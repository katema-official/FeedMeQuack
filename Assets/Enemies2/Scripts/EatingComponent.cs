using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BreadNamespace;

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

        private bool _notDisturbed;     //if false, it means that this duck is victim of a stealing action performed by the player
        private bool _finishedEating;   //true = there are no more BP to eat, false = there are still BP to eat

        private float _minRollModifier = 0.8f;
        private float _maxRollModifier = 1.2f;
        private bool _digestingEnded = false;
        private float _currentDigestingTime;

        public void Initialize(int mouthSize, float chewingRate, float digestingTime)
        {
            _mouthSize = mouthSize;
            _chewingRate = chewingRate;
            _digestingTime = digestingTime;
        }

        void Awake()
        {
            _identifyFoodComponent = GetComponent<IdentifyFoodComponent>();
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
        public void EnterEating_SetNotDisturbed()
        {
            _notDisturbed = true;   //will tell to the duck if it can keep eating the food or not
            _finishedEating = false;
        }

        //I try to model the eating procedure as a coroutine and not as a stayAction for efficiency reasons
        public void EnterEating_StartEating()
        {
            StartCoroutine(EatingCoroutine());
        }

        private IEnumerator EatingCoroutine()
        {
            yield return new WaitForSeconds(_chewingRate);
            while (_notDisturbed && !_finishedEating)
            {
                int eatenBP;
                bool completelyEaten;
                (eatenBP, completelyEaten) = _myBreadInMouthComponent.SubtractBreadPoints(1);
                Debug.Log("eatenBP = " + eatenBP + ", completelyEaten = " + completelyEaten);
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
            _myBreadInMouthComponent = _myFoodInMouthGO.GetComponent<BreadInMouthComponent>();
        }

    }
}