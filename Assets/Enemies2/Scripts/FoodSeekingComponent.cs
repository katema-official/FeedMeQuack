using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelStageNamespace;
using SteeringBehaviourNamespace;
using GraphLakeNamespace;

namespace DuckEnemies
{
    public class FoodSeekingComponent : MonoBehaviour
    {

        private float _speedFoodSeeking;                        //max speed, acceleration, deceleration and steer 
        private float _accelerationFoodSeeking;                 //at which the duck moves when roaming
        private float _decelerationFoodSeeking;
        private float _steerFoodSeeking;
        private float _stopAtFoodSeeking;

        private TileGraphComponent _tileGraphComponent;
        private MovementSeekComponent _movementSeekComponent;
        private IdentifyFoodComponent _identifyFoodComponent;

        private bool _destinationFoodSeekingReached;
        private List<Vector3> _pathFoodSeeking;

        private int _IDOfThisFood;

        public void Initialize(float speed, float acceleration, float deceleration, float steer, float stopAt)
        {
            _speedFoodSeeking = speed;
            _accelerationFoodSeeking = acceleration;
            _decelerationFoodSeeking = deceleration;
            _steerFoodSeeking = steer;
            _stopAtFoodSeeking = stopAt;

            _tileGraphComponent = GameObject.Find("TileGraphLake").GetComponent<TileGraphComponent>();
            _movementSeekComponent = GetComponent<MovementSeekComponent>();
            _identifyFoodComponent = GetComponent<IdentifyFoodComponent>();
        }


        private int _indexCurrentDestination = 0;
        private Vector3 _currentDestination;
        [SerializeField] private float _tresholdCurrentDestinationReached = 1.5f;


        //############################################################# ACTIONS #############################################################

        //when i get into the BreadSeeking state, I have to find the path that takes me from my current position to the actual bread
        public void EnterFoodSeeking_FindPath()
        {
            Vector3 goal = _identifyFoodComponent.GetObjectiveFood().transform.position;
            _pathFoodSeeking = _tileGraphComponent.GetPathFromPointToPoint(transform.position, goal, GetComponent<CircleCollider2D>());

            _indexCurrentDestination = 0;
            _currentDestination = _pathFoodSeeking[0];

            _movementSeekComponent.CurrentDestination = _currentDestination;
            _movementSeekComponent.FinalDestination = _pathFoodSeeking[_pathFoodSeeking.Count - 1];
        }

        public void EnterFoodSeeking_SetSteeringBehaviour()
        {
            _movementSeekComponent.MaxSpeed = _speedFoodSeeking;
            _movementSeekComponent.MaxSteer = _steerFoodSeeking;
            _movementSeekComponent.Acceleration = _accelerationFoodSeeking;
            _movementSeekComponent.Deceleration = _decelerationFoodSeeking;

            _movementSeekComponent.StopAt = _stopAtFoodSeeking;

            _movementSeekComponent.SetAcceleration_Increment();

            _movementSeekComponent.StartMoving();

            _destinationFoodSeekingReached = false;
        }

        public void EnterFoodSeeking_SetFoodID()
        {
            _IDOfThisFood = _identifyFoodComponent.GetObjectiveFood().GetInstanceID();
        }

        public void StayFoodSeeking_UpdateDestination()
        {
            if (_indexCurrentDestination == _pathFoodSeeking.Count - 1)
            {
                //when the distance between me and the final destination is lower than StopAt, i stop accelerating in that direction
                if (Vector2.Distance(transform.position, _currentDestination) <= _movementSeekComponent.StopAt)
                {
                    _destinationFoodSeekingReached = true;
                }
                return;
            }


            if (Vector2.Distance(transform.position, _currentDestination) <= _tresholdCurrentDestinationReached)
            {
                _indexCurrentDestination += 1;
                _currentDestination = _pathFoodSeeking[_indexCurrentDestination];
                _movementSeekComponent.CurrentDestination = _currentDestination;
            }
            return;
        }

        public void ExitFoodSeeking_DeletePath()
        {
            _movementSeekComponent.StopMoving();
        }

        public void ExitFoodSeeking_ResetFoodID()
        {
            _IDOfThisFood = -1;
        }


        //############################################################# TRANSITIONS #############################################################

        public bool DestinationReachedFoodSeeking()
        {
            return _destinationFoodSeekingReached;
        }

        //condition used to check whether the food this component was after is still there or has changed/is not the same
        public bool HasFoodDisappeared()
        {
            Debug.Log("Food disappeared? " + (_identifyFoodComponent.GetObjectiveFood().GetInstanceID() != _IDOfThisFood));
            return _identifyFoodComponent.GetObjectiveFood().GetInstanceID() != _IDOfThisFood;
        }

    }
}