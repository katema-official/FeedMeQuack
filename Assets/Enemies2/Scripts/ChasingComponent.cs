using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using GraphLakeNamespace;
using SteeringBehaviourNamespace;

namespace DuckEnemies {
    public class ChasingComponent : MonoBehaviour
    {


        //CHASING
        private float _stealTriggerProbability;
        private float _speedChasing;
        private float _accelerationChasing;
        private float _decelerationChasing;
        private float _steerChasing;
        private float _wantsToStealCooldown;
        private float _actualStealCooldown = 0f;

        private IdentifyPlayerComponent _identifyPlayerComponent;
        private TileGraphComponent _tileGraphComponent;
        private MovementSeekComponent _movementSeekComponent;
        private PlayerController _playerController;

        private float _stopAtPlayer = 3f;   //minimum distance between enemy and player such that the player is considered reached

        private Coroutine _findPathCoroutine;

        public void Initialize(float stealTriggerProbability, float speed, float acceleration, float deceleration, float steer, float cooldown)
        {
            _stealTriggerProbability = stealTriggerProbability;
            _speedChasing = speed;
            _accelerationChasing = acceleration;
            _decelerationChasing = deceleration;
            _steerChasing = steer;
            _wantsToStealCooldown = cooldown;
            _actualStealCooldown = 0f;

            _identifyPlayerComponent = GetComponent<IdentifyPlayerComponent>();
            _tileGraphComponent = GameObject.Find("TileGraphLake").GetComponent<TileGraphComponent>();
            _movementSeekComponent = GetComponent<MovementSeekComponent>();
            _playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        }

        //############################################################# ACTIONS #############################################################
        public void EnterChasing_StartPathFinderCoroutine()
        {

        }

        public void ExitCoroutine_StopPathFinderCoroutine()
        {

        }


        //############################################################# TRANSITIONS #############################################################
        public bool DecidedToSteal()
        {
            return _identifyPlayerComponent.IsThereAnObjectivePlayer() && 
                _playerController.GetState() == PlayerState.Eating &&   //TODO: could also be the state "Carrying"
                _actualStealCooldown == 0f && 
                _identifyPlayerComponent.GetU() < _stealTriggerProbability;
        }



        //############################################################# UTILITIES #############################################################

        private IEnumerator FindPathCoroutine()
        {

            yield return null;
        }




        //CHASING (should be seen as part of the whole "stealing active" part of the enemy FSM)
    }
}