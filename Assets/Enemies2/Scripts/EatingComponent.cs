using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BreadNamespace;
using TMPro;
using Music;

namespace DuckEnemies
{
    public class EatingComponent : MonoBehaviour
    {
        //I would like this component to take care of three states, since they are conceptually tied one another:
        //Bite
        //Eating
        //Digesting


        private IdentifyFoodComponent _identifyFoodComponent;

        [SerializeField] private GameObject _myFoodInMouthGO = null;
        [SerializeField] private BreadInMouthComponent _myBreadInMouthComponent = null;

        private int _mouthSize;
        private float _chewingRate;
        private float _digestingTime;

        private bool _notDisturbed = true;     //if false, it means that this duck is victim of a stealing action performed by the player
        private bool _finishedEating = true;   //true = there are no more BP to eat, false = there are still BP to eat

        private float _minRollModifier = 0.8f;
        private float _maxRollModifier = 1.2f;
        private bool _digestingEnded = false;
        private float _currentDigestingTime;

        [SerializeField] private GameObject _eatingStatusPrefab;
        private GameObject _eatingStatusGO;
        private AnimalSoundController _animalSoundController;

        public void Initialize(int mouthSize, float chewingRate, float digestingTime)
        {
            _mouthSize = mouthSize;
            _chewingRate = chewingRate;
            _digestingTime = digestingTime;
        }

        void Awake()
        {
            _identifyFoodComponent = GetComponent<IdentifyFoodComponent>();
            _animalSoundController = GetComponent<AnimalSoundController>();
        }





        //############################################################# ACTIONS #############################################################

        //BITE
        //these two enter actions are performed one after another
        public void EnterBite_CleanVariables()
        {
            _myFoodInMouthGO = null;
            _myBreadInMouthComponent = null;
        }

        public void EnterBite_BiteBreadInWater()
        {
            //last check: is the bread I was pointing at still there?
            if (_identifyFoodComponent.IsThereAnObjectiveFood())
            {
                //if so, bite it
                _myFoodInMouthGO = _identifyFoodComponent.GetObjectiveFood().GetComponent<BreadInWaterComponent>().GenerateNewBreadInMouth(_mouthSize);
                _myBreadInMouthComponent = _myFoodInMouthGO.GetComponent<BreadInMouthComponent>();
            }
        }


        //EATING
        //(the bread to eat was already fixed in the Bite state!)
        public void EnterEating_ResetValues()
        {
            _notDisturbed = true;   //will tell to the duck if it can keep eating the food or not
            _finishedEating = false;
        }

        public void EnterEating_SpawnEatingStatus()
        {
            _eatingStatusGO = Instantiate(_eatingStatusPrefab);
            _eatingStatusGO.transform.SetParent(transform);
            _eatingStatusGO.transform.localPosition = new Vector3(0f, 2.25f, 0);
            _eatingStatusGO.transform.localScale = new Vector3(0.35f, 0.35f, 0);
            _eatingStatusGO.transform.Find("ValueText").GetComponent<TextMeshPro>().text = "" + _myBreadInMouthComponent.GetBreadPoints();
        }

        //I try to model the eating procedure as a coroutine and not as a stayAction for efficiency reasons
        public void EnterEating_StartEating()
        {
            StartCoroutine(EatingCoroutine());
        }

        public void EnterEating_PlaySound()
        {
            _animalSoundController.SetIsEnemy(true);
            _animalSoundController.Eat();
        }

        private IEnumerator EatingCoroutine()
        {
            yield return new WaitForSeconds(_chewingRate);
            while (_notDisturbed && !_finishedEating)
            {
                int eatenBP;
                bool completelyEaten;
                (eatenBP, completelyEaten) = _myBreadInMouthComponent.SubtractBreadPoints(1);
                //Debug.Log("eatenBP = " + eatenBP + ", completelyEaten = " + completelyEaten);
                _eatingStatusGO.transform.Find("ValueText").GetComponent<TextMeshPro>().text = "" + _myBreadInMouthComponent.GetBreadPoints();
                if (!completelyEaten)
                {
                    yield return new WaitForSeconds(_chewingRate);
                }
                else
                {
                    _finishedEating = true;
                }
            }
            yield return null;
        }

        public void ExitEating_DestoryEatingStatus()
        {
            Destroy(_eatingStatusGO);
        }

        public void ExitEating_ResetValues()
        {
            _notDisturbed = true;   //will tell to the duck if it can keep eating the food or not
            _finishedEating = false;
        }

        public void ExitEating_StopSound()
        {
            _animalSoundController.UnEat();
        }


        //DIGESTING
        public void EnterDigesting_ChooseDigestingTime()
        {
            _digestingEnded = false;
            _currentDigestingTime = _digestingTime * Random.Range(_minRollModifier, _maxRollModifier);
            StartCoroutine(DigestCoroutine());
        }

        private IEnumerator DigestCoroutine()
        {
            yield return new WaitForSeconds(_currentDigestingTime);
            _digestingEnded = true;
        }

        public void ExitDigesting()
        {
            _digestingEnded = false;
        }




        //############################################################# TRANSITIONS #############################################################

        //BITE
        public bool CanBiteTheFood()
        {
            return _myBreadInMouthComponent != null;
        }


        //EATING
        public bool DidIFinishEating()
        {
            return _finishedEating;
        }

        public bool WasIDisturbed()
        {
            return !_notDisturbed;
        }


        //DIGESTING
        public bool GetDigestingEnded()
        {
            return _digestingEnded;
        }



        //############################################################# UTILITIES #############################################################

        //useful for telling to the eating coroutine to stop eating as the duck is now the victim of a stealing attempt from the player
        public void Disturb()
        {
            _notDisturbed = false;
        }


        //Useful for when the duck exited from stealingPassive and still has some bread
        public void DirectlySetFoodInMouth(GameObject foodGO)
        {
            _myFoodInMouthGO = foodGO;
            //In the future, I should check if this food is of kind "bread". But for now, the only food is indeed the bread.
            if (foodGO != null)
            {
                _myBreadInMouthComponent = _myFoodInMouthGO.GetComponent<BreadInMouthComponent>();
            }
            else
            {
                _myBreadInMouthComponent = null;
            }
        }

        //actually this method could be used both as an utility and a transition, I need to think about it
        //(on a second thought: no, it's just an utility, because its opposite is actually a transition method)
        public bool IsEating()
        {
            return !DidIFinishEating() && _myFoodInMouthGO != null;
        }

        public BreadInMouthComponent GetBreadInMouthComponent()
        {
            return _myBreadInMouthComponent;
        }

    }
}