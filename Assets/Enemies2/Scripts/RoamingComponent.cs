using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphLakeNamespace;
using System.Linq;
using LevelStageNamespace;
using SteeringBehaviourNamespace;

namespace DuckEnemies
{
    public class RoamingComponent : MonoBehaviour
    {

        private float _speedRoaming;                        //max speed, acceleration, deceleration and steer 
        private float _accelerationRoaming;                 //at which the duck moves when roaming
        private float _decelerationRoaming;
        private float _steerRoaming;
        private float _stopAtRoaming;
        private float _chillingTime;

        private Vector3 _finalDestinationRoaming;

        private LakeShopDescriptionComponent _lakeShopDescriptionComponent;
        private TileGraphComponent _tileGraphComponent;

        private MovementSeekComponent _movementSeekComponent;
        private Animator _animator;


        //this component takes car of the states:
        //CHILLING
        //ROAMING

        private float _minRollModifier = 0.8f;
        private float _maxRollModifier = 1.2f;

        private float _currentChillingTime;
        private bool _chillEnded = false;

        private List<Vector3> _pathRoaming;

        private bool _destinationRoamingReached = false;    //used to go from roaming to hubState

        private enum DirectionsRoaming
        {
            Up,
            Down,
            Left,
            Right,
            None
        }
        private DirectionsRoaming _lastDirectionFollowed = DirectionsRoaming.None;
        private float _desiredRoamingDistance;

        public void Initialize(float speedRoaming, float accelerationRoaming, float decelerationRoaming, float steerRoaming, 
            float chillingTime, float desiredRoamingDistance, float roamingStopAt)
        {
            _speedRoaming = speedRoaming;
            _accelerationRoaming = accelerationRoaming;
            _decelerationRoaming = decelerationRoaming;
            _steerRoaming = steerRoaming;
            _chillingTime = chillingTime;
            _desiredRoamingDistance = desiredRoamingDistance;
            _stopAtRoaming = roamingStopAt;
            _lakeShopDescriptionComponent = GameObject.Find("WholeLake").GetComponent<LakeShopDescriptionComponent>();
            _tileGraphComponent = GameObject.Find("TileGraphLake").GetComponent<TileGraphComponent>();
            _movementSeekComponent = GetComponent<MovementSeekComponent>();
            _animator = transform.Find("Sprite").GetComponent<Animator>();
            _destinationRoamingReached = false;
        }










        //############################################################# ACTIONS #############################################################

        //enter method for Chilling: choose a chilling time
        public void EnterChilling_ChooseChillingTime()
        {
            _chillEnded = false;
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
            //to choose a path, we do something not-so-simple: we want each time to make the duck go in a different direction

            //FIRST: I get the directions in which I can move
            List<DirectionsRoaming> directionPreferences = GetDirectionPreferences();

            //SECOND: I choose a point in which I want to move, preferring the ones in the first directions of the list
            _finalDestinationRoaming = GetRoamingGoal(directionPreferences);
            _pathRoaming = _tileGraphComponent.GetPathFromPointToPoint(transform.position, _finalDestinationRoaming, GetComponent<CircleCollider2D>());
            _indexCurrentDestination = 0;
            _currentDestination = _pathRoaming[0];

            //THIRD: I have to follow this path. The path will be followed starting from here, and it will be updated in the stay action
            _movementSeekComponent.SetCurrentAndFinalDestination(_currentDestination, _pathRoaming[_pathRoaming.Count - 1]);



        }

        //enter method for Roaming: set speed and such for the seek component
        public void EnterRoaming_SetSteeringBehaviour()
        {
            _movementSeekComponent.MaxSpeed = _speedRoaming;
            _movementSeekComponent.MaxSteer = _steerRoaming;
            _movementSeekComponent.Acceleration = _accelerationRoaming;
            _movementSeekComponent.Deceleration = _decelerationRoaming;

            _movementSeekComponent.StopAt = _stopAtRoaming;

            _movementSeekComponent.SetAcceleration_Increment();

            _movementSeekComponent.StartMoving();// IsDestinationValid = true;

            _destinationRoamingReached = false;

            //FOURTH: the duck might be disturbed during its traveling. If that happens, it can get stuck. To avoid this, we run a procedure that
            //checks if the duck arrived at each intermediate (and final) destination in an expected time. If it didn't, we recompute the path.
            _checkIfPathIsFollowedCoroutine = StartCoroutine(CheckIfPathIsFollowed());

        }


        private int _indexCurrentDestination = 0;
        private Vector3 _currentDestination;
        [SerializeField] private float _tresholdCurrentDestinationReached = 1.5f;   //when the enemy duck and the currentDestination (that is NOT the final one) have
                                                                   //a distance <= this value, then the current destination is considered reached


        public void StayRoaming_UpdateDestination()
        {
            if(_indexCurrentDestination == _pathRoaming.Count - 1)
            {
                //when the distance between me and the final destination is lower than StopAt, i stop accelerating in that direction
                if(Vector2.Distance(transform.position, _currentDestination) <= _movementSeekComponent.StopAt)
                {
                    _destinationRoamingReached = true;
                }
                return;
            }


            if(Vector2.Distance(transform.position, _currentDestination) <= _tresholdCurrentDestinationReached)
            {
                _indexCurrentDestination += 1;
                _currentDestination = _pathRoaming[_indexCurrentDestination];
                _movementSeekComponent.SetCurrentDestination(_currentDestination);

                //the very moment we reach the first destination, we set the steering to its maximum value (TODO: should not be necessary anymore, delete this)
                //_movementSeekComponent.MaxSteer = _steerRoaming;
            }
            return;

        }

        public void ExitRoaming_StopCoroutine()
        {
            StopCoroutine(_checkIfPathIsFollowedCoroutine);
            _checkIfPathIsFollowedCoroutine = null;

            _movementSeekComponent.StopMoving();
        }


        //############################################################# TRANSITIONS #############################################################

        public bool GetChillEnded()
        {
            return _chillEnded;
        }

        public bool DestinationReachedRoaming() 
        {
            return _destinationRoamingReached;
        }



        //############################################################# UTILITIES #############################################################

        private Coroutine _checkIfPathIsFollowedCoroutine;
        private Vector3 _currentDestinationToReach;
        private IEnumerator CheckIfPathIsFollowed()
        {
            //the idea of this procedure is:
            //I want to make sure that the duck reaches its destination. To be sure about that, I do the following:
            //-Given its current destination, and assuming the duck moves at max speed (to be more fair we should take into account also acceleration, but for
            //now we'll just stick with this simplyfying assumption), we can compute the expected time needed by the duck to reach the current destination.
            //For this amount of time, we sleep.
            //-Once we wake up, we check: did the duck reach that destination or not? (can be checked controlling if the current destination has changed or not)
            //If yes, phew, we are happy, but we must repeat this kind of check for each current destination until the final one has been reached.
            //If no, argh, the duck was disturbed somehow and may be stuck. In that case, we have to compute a new path from the current position to the final
            //destination, and update all the data interested in the path.

            while (!DestinationReachedRoaming()) {
                _currentDestinationToReach = _currentDestination;
                float expectedTime = (Vector2.Distance(transform.position, _currentDestinationToReach)) / _speedRoaming;
                expectedTime *= 1.2f;       //we give a bit of tolerance, we don't want to be extremely precise
                yield return new WaitForSeconds(expectedTime);

                if (!DestinationReachedRoaming())
                {
                    if (_currentDestinationToReach == _currentDestination)
                    {
                        //the duck was disturbed
                        _pathRoaming = _tileGraphComponent.GetPathFromPointToPoint(transform.position, _finalDestinationRoaming, GetComponent<CircleCollider2D>());
                        _indexCurrentDestination = 0;
                        _currentDestination = _pathRoaming[0];
                        _movementSeekComponent.SetCurrentAndFinalDestination(_currentDestination, _pathRoaming[_pathRoaming.Count - 1]);
                        //Debug.Log("RECOMPUTED");
                    }
                    else
                    {
                        //the duck was not disturbed
                        //Debug.Log("NOT RECOMPUTED :)");
                    }
                }

            }
            yield return null;
        }



        private List<DirectionsRoaming> GetDirectionPreferences()
        {
            List<DirectionsRoaming> l;
            List<DirectionsRoaming> l_up_down = new List<DirectionsRoaming>() { DirectionsRoaming.Up, DirectionsRoaming.Down };
            List<DirectionsRoaming> l_left_right = new List<DirectionsRoaming>() { DirectionsRoaming.Left, DirectionsRoaming.Right };
            switch (_lastDirectionFollowed)
            {
                case DirectionsRoaming.None:
                    l = new List<DirectionsRoaming>() { DirectionsRoaming.Up, DirectionsRoaming.Down, DirectionsRoaming.Left, DirectionsRoaming.Right };
                    l = l.OrderBy(x => Random.Range(0f, 1f)).Take(4).ToList<DirectionsRoaming>();
                    break;
                case DirectionsRoaming.Up:
                    l = new List<DirectionsRoaming>();
                    l_left_right = l_left_right.OrderBy(x => Random.Range(0f, 1f)).Take(2).ToList<DirectionsRoaming>();
                    l.AddRange(l_left_right);
                    l.Add(DirectionsRoaming.Down);
                    l.Add(DirectionsRoaming.Up);
                    break;
                case DirectionsRoaming.Down:
                    l = new List<DirectionsRoaming>();
                    l_left_right = l_left_right.OrderBy(x => Random.Range(0f, 1f)).Take(2).ToList<DirectionsRoaming>();
                    l.AddRange(l_left_right);
                    l.Add(DirectionsRoaming.Up);
                    l.Add(DirectionsRoaming.Down);
                    break;
                case DirectionsRoaming.Left:
                    l = new List<DirectionsRoaming>();
                    l_up_down = l_up_down.OrderBy(x => Random.Range(0f, 1f)).Take(2).ToList<DirectionsRoaming>();
                    l.AddRange(l_up_down);
                    l.Add(DirectionsRoaming.Right);
                    l.Add(DirectionsRoaming.Left);
                    break;
                case DirectionsRoaming.Right:
                    l = new List<DirectionsRoaming>();
                    l_up_down = l_up_down.OrderBy(x => Random.Range(0f, 1f)).Take(2).ToList<DirectionsRoaming>();
                    l.AddRange(l_up_down);
                    l.Add(DirectionsRoaming.Left);
                    l.Add(DirectionsRoaming.Right);
                    break;
                default:
                    l = null;
                    break;
            }
            return l;
        }

        private Vector3 GetRoamingGoal(List<DirectionsRoaming> preferences)
        {
            bool chosen = false;
            Vector3 chosenPoint = Vector3.zero;
            int trialsPerDirection = 10;    //How many times should I try to take a point in a certain direction before giving up and going to the enxt direction?
            int directionIndex = 0;
            int nDirections = preferences.Count;

            //these values can absolutely change of course, but for now they will suffice
            float widthRectangle = _desiredRoamingDistance * (3f / 2f);
            float heightRectangle = _decelerationRoaming / 2f;

            //Debug.Log("WHILE in RoamingComponent iniziato");
            while (!chosen)
            {
                float x0, x1, y0, y1;
                for(int i = 0; i < trialsPerDirection; i++)
                {
                    switch (preferences[directionIndex])
                    {
                        case DirectionsRoaming.Up:
                            x0 = transform.position.x - widthRectangle / 2f;
                            x1 = transform.position.x + widthRectangle / 2f;
                            y0 = transform.position.y + _desiredRoamingDistance - heightRectangle / 2f;
                            y1 = transform.position.y + _desiredRoamingDistance + heightRectangle / 2f;
                            break;
                        case DirectionsRoaming.Down:
                            x0 = transform.position.x - widthRectangle / 2f;
                            x1 = transform.position.x + widthRectangle / 2f;
                            y0 = transform.position.y - _desiredRoamingDistance - heightRectangle / 2f;
                            y1 = transform.position.y - _desiredRoamingDistance + heightRectangle / 2f;
                            break;
                        case DirectionsRoaming.Left:
                            x0 = transform.position.x - _desiredRoamingDistance - heightRectangle / 2f;
                            x1 = transform.position.x - _desiredRoamingDistance + heightRectangle / 2f;
                            y0 = transform.position.y - widthRectangle / 2f;
                            y1 = transform.position.y + widthRectangle / 2f;
                            break;
                        case DirectionsRoaming.Right:
                            x0 = transform.position.x + _desiredRoamingDistance - heightRectangle / 2f;
                            x1 = transform.position.x + _desiredRoamingDistance + heightRectangle / 2f;
                            y0 = transform.position.y - widthRectangle / 2f;
                            y1 = transform.position.y + widthRectangle / 2f;
                            break;
                        default:
                            x0 = 0;
                            x1 = 0;
                            y0 = 0;
                            y1 = 0;
                            break;

                    }
                    Vector3 tentativePoint = GetRandomPointInRectangle(x0, y0, x1, y1);
                    if (_lakeShopDescriptionComponent.Contains(tentativePoint))
                    {
                        chosenPoint = tentativePoint;
                        chosen = true;
                        break;
                    }
                }
                directionIndex = (directionIndex + 1) % nDirections;
            }
            //Debug.Log("WHILE in RoamingComponent finito");

            return chosenPoint;
        }

        private Vector3 GetRandomPointInRectangle(float x0, float y0, float x1, float y1)
        {
            return new Vector3(Random.Range(x0, x1), Random.Range(y0, y1), 0);
        }




    }
}