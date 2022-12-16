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

        private GameObject _myBreadInMouthGO = null;
        private BreadInMouthComponent _myBreadInMouthComponent = null;

        private int _mouthSize;
        private float _chewingRate;
        private float _digestingTime;


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
            _myBreadInMouthGO = null;
            _myBreadInMouthComponent = null;
        }

        public void EnterBite_BiteBreadInWater()
        {
            //last check: is the bread I was pointing at still there?
            if (_identifyFoodComponent.IsThereAnObjectiveFood())
            {
                //if so, bite it
                _myBreadInMouthGO = _identifyFoodComponent.GetObjectiveFood().GetComponent<BreadInWaterComponent>().GenerateNewBreadInMouth(_mouthSize);
                _myBreadInMouthComponent = _myBreadInMouthGO.GetComponent<BreadInMouthComponent>();
            }
        }







        //############################################################# TRANSITIONS #############################################################

        //BITE
        public bool CanBiteTheFood()
        {
            return _myBreadInMouthComponent != null;
        }





        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}