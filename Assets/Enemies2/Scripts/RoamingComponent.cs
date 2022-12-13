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
        private float _chillingTime;

        private LakeShopDescriptionComponent _lakeShopDescriptionComponent;
        private TileGraphComponent _tileGraphComponent;

        private DDelegatedSteering _dDelegatedSteering;
        private SeekBehaviour _seekBehaviour;
        private DragBehaviour _dragBehaviour;


        //this component takes car of the states:
        //CHILLING
        //ROAMING

        private float _minRollModifier = 0.8f;
        private float _maxRollModifier = 1.2f;

        private float _currentChillingTime;
        private bool _chillEnded = false;

        private List<Vector3> _pathRoaming;

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
            float chillingTime, float desiredRoamingDistance)
        {
            _speedRoaming = speedRoaming;
            _accelerationRoaming = accelerationRoaming;
            _decelerationRoaming = decelerationRoaming;
            _steerRoaming = steerRoaming;
            _chillingTime = chillingTime;
            _desiredRoamingDistance = desiredRoamingDistance;
            _lakeShopDescriptionComponent = GameObject.Find("WholeLake").GetComponent<LakeShopDescriptionComponent>();
            _tileGraphComponent = GameObject.Find("TileGraphLake").GetComponent<TileGraphComponent>();
            _dDelegatedSteering = GetComponent<DDelegatedSteering>();
            _seekBehaviour = GetComponent<SeekBehaviour>();
            _dragBehaviour = GetComponent<DragBehaviour>();
            GetComponent<Rigidbody2D>().WakeUp();
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
            //to choose a path, we do something not-so-simple: we want each time to make the duck go in a different direction

            //FIRST: I get the directions in which I can move
            List<DirectionsRoaming> directionPreferences = GetDirectionPreferences();

            //SECOND: I choose a point in which I want to move, preferring the ones in the first directions of the list
            Vector3 dest = GetRoamingGoal(directionPreferences);
            _pathRoaming = _tileGraphComponent.GetPathFromPointToPoint(transform.position, dest, GetComponent<CircleCollider2D>());
            _indexCurrentDestination = -1;
            _currentDestination = _pathRoaming[0];
            _updatedDestination = true;

            //THIRD: I have to follow this path. The path will be followed starting from here, and it will be updated in the stay action
            _seekBehaviour.Destination = _currentDestination;

            //THIRD AND A HALF: I should also set the brakeAt and stopAt of the seekComponent depending on the fact that this is the final
            //destination or not AND depending on the fact that I will get close to it at full speed or not.
            //For the moment, I simply set them to 0
            _seekBehaviour.BrakeAt = 0f;
            _seekBehaviour.StopAt = 0f;
            _seekBehaviour.IsDestinationValid = true;

            Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
            float degrees = Vector2.SignedAngle(Vector2.right - currentPosition, 
                new Vector2(_currentDestination.x, _currentDestination.y) - currentPosition);
            GetComponent<Rigidbody2D>().SetRotation(degrees);

        }

        //enter method for Roaming: set speed and such for the seek component
        public void EnterRoaming_SetSteeringBehaviour()
        {
            _dDelegatedSteering.maxLinearSpeed = _speedRoaming;
            _seekBehaviour.Acceleration = _accelerationRoaming;
            _seekBehaviour.Deceleration = _decelerationRoaming;
            _seekBehaviour.Steer = _steerRoaming;

            _dragBehaviour.linearDrag = 5f;
            _dragBehaviour.angularDrag = 3f;

        }


        private int _indexCurrentDestination = -1;
        private Vector3 _currentDestination;
        private float _tresholdDestinationReached = 1.5f;   //when the enemy duck and the currentDestination have a distance <= this value,
                                                            //the current destination is considered reached
        private bool _updatedDestination = true;
        public void StayRoaming_UpdateDestination()
        {
            //To avoid repeating the same operations over and over, we proceed like this:
            //If the last destination set was reached, we compute the new one.
            //Otherwise, we don't do anything: all the values useful for the movement are already set
            //We assume, to be general, that we have to start by setting the destination
            if(_updatedDestination == false && Vector3.Distance(transform.position, _currentDestination) <= _tresholdDestinationReached)
            {
                _updatedDestination = true;
            }

            //read "_updatedDestination = true" as: "Hey, there is a new destination to reach, so, do all the necessary stuff to reach it!"
            if (_updatedDestination == true)
            {
                _indexCurrentDestination++;
                

                //if the next destination is not the last
                if(_indexCurrentDestination < _pathRoaming.Count - 1)
                {
                    _currentDestination = _pathRoaming[_indexCurrentDestination];
                    _seekBehaviour.Destination = _currentDestination;
                    _updatedDestination = false;
                }

                //each time this stay action gets called, we update the current destination (or not, it depends)
                if (_indexCurrentDestination == _pathRoaming.Count - 1)
                {
                    _currentDestination = _pathRoaming[_indexCurrentDestination];
                    //if this is the last destination, we have to re-set the brakeAt distance and the stopAtDistance.
                    //We start by doing this the easy way: just set the brakeAt distance to the "braking distance"
                    _seekBehaviour.BrakeAt = Mathf.Pow(_dDelegatedSteering.GetMovementStatus().linearSpeed, 2) / (2 * _seekBehaviour.Deceleration) + 0.5f;  //a correction term
                    _seekBehaviour.StopAt = 0.1f;
                    _seekBehaviour.Destination = _currentDestination;
                    _updatedDestination = false;
                }

                //if the last destination has been reached (change state)
                if (_indexCurrentDestination > _pathRoaming.Count - 1)
                {
                    _seekBehaviour.IsDestinationValid = false;


                    
                }
                

                

            }

        }


        //############################################################# TRANSITIONS #############################################################

        public bool GetChillEnded()
        {
            return _chillEnded;
        }





        //############################################################# UTILITIES #############################################################



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
                    break;
                case DirectionsRoaming.Down:
                    l = new List<DirectionsRoaming>();
                    l_left_right = l_left_right.OrderBy(x => Random.Range(0f, 1f)).Take(2).ToList<DirectionsRoaming>();
                    l.AddRange(l_left_right);
                    l.Add(DirectionsRoaming.Up);
                    break;
                case DirectionsRoaming.Left:
                    l = new List<DirectionsRoaming>();
                    l_up_down = l_up_down.OrderBy(x => Random.Range(0f, 1f)).Take(2).ToList<DirectionsRoaming>();
                    l.AddRange(l_up_down);
                    l.Add(DirectionsRoaming.Right);
                    break;
                case DirectionsRoaming.Right:
                    l = new List<DirectionsRoaming>();
                    l_up_down = l_up_down.OrderBy(x => Random.Range(0f, 1f)).Take(2).ToList<DirectionsRoaming>();
                    l.AddRange(l_up_down);
                    l.Add(DirectionsRoaming.Left);
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

            Debug.Log("WHILE in RoamingComponent iniziato");
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
            Debug.Log("WHILE in RoamingComponent finito");

            return chosenPoint;
        }

        private Vector3 GetRandomPointInRectangle(float x0, float y0, float x1, float y1)
        {
            return new Vector3(Random.Range(x0, x1), Random.Range(y0, y1), 0);
        }




    }
}