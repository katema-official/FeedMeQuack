using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphLakeNamespace;

namespace DuckEnemies
{
    public class RoamingComponent : MonoBehaviour
    {

        private float _speedRoaming;                        //max speed, acceleration, deceleration and steer 
        private float _accelerationRoaming;                 //at which the duck moves when roaming
        private float _decelerationRoaming;
        private float _steerRoaming;
        private float _chillingTime;

        private TileGraphComponent _tileGraphComponent;


        //this component takes car of the states:
        //CHILLING
        //ROAMING

        private float _minRollModifier = 0.8f;
        private float _maxRollModifier = 1.2f;

        private float _currentChillingTime;
        private bool _chillEnded = false;

        public void Initialize(float speedRoaming, float accelerationRoaming, float decelerationRoaming, float steerRoaming, float chillingTime)
        {
            _speedRoaming = speedRoaming;
            _accelerationRoaming = accelerationRoaming;
            _decelerationRoaming = decelerationRoaming;
            _steerRoaming = steerRoaming;
            _chillingTime = chillingTime;
            _tileGraphComponent = GameObject.Find("TileGraphLake").GetComponent<TileGraphComponent>();
        }










        //############################################################# ACTIONS #############################################################

        //enter method for Chilling: choose a chilling time
        public void EnterChilling_ChooseChillingTime()
        {
            Debug.Log("Chilling time = " + _chillingTime);
            _currentChillingTime = _chillingTime * Random.Range(_minRollModifier, _maxRollModifier);
            StartCoroutine(ChillCoroutine());
        }

        private IEnumerator ChillCoroutine()
        {
            yield return new WaitForSeconds(_currentChillingTime);
            _chillEnded = true;
        }

        //exit method for Chilling: change _chillEnded
        public void ExitChilling()
        {
            _chillEnded = false;
        }



        //enter method for Roaming: choose a path to follow
        public void EnterRoaming_ChooseRandomPath()
        {
            //per domani: come scegli dove andare? a caso dentro un cerchio? o qualcosa di un po' più complesso?
            //e come fai a essere sicuro che a una certa distanza troverai una casella libera? Perché se ne scegli una completamente a caso,
            //può fare schifo e potresti muoverti di poco.
        }


        //############################################################# TRANSITIONS #############################################################

        public bool GetChillEnded()
        {
            return _chillEnded;
        }







    }
}