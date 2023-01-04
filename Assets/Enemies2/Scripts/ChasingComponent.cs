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

        private float _stopAtPlayer = 5f;   //minimum distance between enemy and player such that the player is considered reached



        private Coroutine _findPathCoroutine;

        private bool _keepChasing;
        private List<Vector3> _pathChasing;
        private Vector3 _finalDestinationChasing;
        private Vector3 _currentDestination;
        private int _indexCurrentDestination;
        [SerializeField] private float _tresholdCurrentDestinationReached = 1.5f;

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

            _keepChasing = false;
            _indexCurrentDestination = 0;

        }

        //############################################################# ACTIONS #############################################################
        public void EnterChasing_StartPathFinderCoroutine()
        {
            ComputePath();
            _keepChasing = true;
            _findPathCoroutine = StartCoroutine(FindPath());
        }

        public void EnterChasing_SetSteeringBehaviour()
        {
            _movementSeekComponent.MaxSpeed = _speedChasing;
            _movementSeekComponent.MaxSteer = _steerChasing;
            _movementSeekComponent.Acceleration = _accelerationChasing;
            _movementSeekComponent.Deceleration = _decelerationChasing;

            _movementSeekComponent.StopAt = _stopAtPlayer;

            _movementSeekComponent.SetAcceleration_Increment();

            _movementSeekComponent.StartMoving();
        }



        public void StayChasing_UpdateDestination()
        {
            //if (_indexCurrentDestination == _pathChasing.Count - 1)
            //{
                //when the distance between me and the final destination is lower than StopAt, i stop accelerating in that direction
            if (Vector2.Distance(transform.position, GetPlayer().gameObject.transform.position) <= _movementSeekComponent.StopAt)
            {
                    _keepChasing = false;
                    //Debug.Log("REACHED");
            }
            else
            {
                //Debug.Log("Not reached! distance is " + Vector2.Distance(transform.position, GetPlayer().gameObject.transform.position));
            }
            //}


            if (Vector2.Distance(transform.position, _currentDestination) <= _tresholdCurrentDestinationReached &&
                _indexCurrentDestination <= _pathChasing.Count - 1)
            {
                _indexCurrentDestination += 1;
                _currentDestination = _pathChasing[_indexCurrentDestination];
                _movementSeekComponent.SetCurrentDestination(_currentDestination);
            }
            return;
        }



        public void ExitChasing_DeletePath()
        {
            _movementSeekComponent.StopMoving();
        }

        public void ExitCoroutine_StopPathFinderCoroutine()
        {
            StopCoroutine(_findPathCoroutine);
            _keepChasing = false;
        }


        //to reset the will of the duck to steal the player
        public void ResetStealingCooldown()
        {
            _actualStealCooldown = _wantsToStealCooldown;
            _identifyPlayerComponent.ForgetAboutPlayer();
            StartCoroutine(DecreaseCooldown());

        }

        private IEnumerator DecreaseCooldown()
        {
            while(_actualStealCooldown > 0)
            {
                _actualStealCooldown -= 1f;
                yield return new WaitForSeconds(1f);
            }
            yield return null;
        }
        


        //############################################################# TRANSITIONS #############################################################
        public bool DecidedToSteal()
        {
            return _identifyPlayerComponent.IsThereAnObjectivePlayer() && 
                _playerController.GetState() == PlayerState.Eating &&   //TODO: could also be the state "Carrying"
                _actualStealCooldown == 0f && 
                _identifyPlayerComponent.GetU() < _stealTriggerProbability;
        }

        public bool MustStopChasing()
        {
            return _playerController.GetState() != PlayerState.Eating || _actualStealCooldown > 0f;
        }

        public bool PlayerReached()
        {
            return _keepChasing == false;
        }



        //############################################################# UTILITIES #############################################################

        //coroutine that computes the path to follow to get to the player
        private float _updateTime = 0.3f;     //every _updateTime the path will be recomputed
        private IEnumerator FindPath()
        {
            while(_keepChasing)
            {
                yield return new WaitForSeconds(_updateTime);
                ComputePath();

            }
            yield return null;
        }

        private void ComputePath()
        {
            _finalDestinationChasing = _identifyPlayerComponent.GetObjectivePlayer().transform.position;
            _pathChasing = _tileGraphComponent.GetPathFromPointToPoint(transform.position, _finalDestinationChasing, GetComponent<CircleCollider2D>());
            _indexCurrentDestination = 0;

            _tileGraphComponent.DrawPath(_pathChasing, Color.black);

            //the player moves, so we have to implement a sort of chasing component.
            //1) Compute current distance between enemy and player
            float distanceEnemyPlayer = 0f;
            for (int i = 0; i < _pathChasing.Count - 1; i++)
            {
                distanceEnemyPlayer += Vector2.Distance(_pathChasing[i], _pathChasing[i + 1]);
            }

            distanceEnemyPlayer = Mathf.Max(distanceEnemyPlayer, 2f);

            //2) Compute the time to get there (at max speed, for simplicity)
            float timeToGetThere = distanceEnemyPlayer / _speedChasing;

            //3) Assuming that the player keeps moving in that direction at that speed, compute where it would end up
            float plusDistance = _playerController.GetComponent<Rigidbody2D>().velocity.magnitude * timeToGetThere;
            Vector3 arrival = new Vector2(_pathChasing[_pathChasing.Count - 1].x, _pathChasing[_pathChasing.Count - 1].y) +
                _playerController.GetComponent<Rigidbody2D>().velocity.normalized * plusDistance;

            //4) Add this new point to the end of the path to follow (shouldn't be done like this, but it's the fastest and easiset way)
            _pathChasing.Add(arrival);

            _tileGraphComponent.DrawPath(_pathChasing, Color.green);

            _currentDestination = _pathChasing[0];
            _movementSeekComponent.SetCurrentAndFinalDestination(_currentDestination, _pathChasing[_pathChasing.Count - 1]);
            _movementSeekComponent.StartMoving();
        }




        public PlayerController GetPlayer()
        {
            return _playerController;
        }

        public float GetActualStealCooldown()
        {
            return _actualStealCooldown;
        }

        public void NotifyPlayerJustRobbed()
        {
            bool callCoroutine = false;
            if(_actualStealCooldown == 0) callCoroutine = true;
            _actualStealCooldown += 2f;
            if (callCoroutine) StartCoroutine(DecreaseCooldown());
        }


    }
}