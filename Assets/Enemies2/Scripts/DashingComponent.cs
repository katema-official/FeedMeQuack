using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SteeringBehaviourNamespace;
using LevelStageNamespace;
using Music;

namespace DuckEnemies
{
    public class DashingComponent : MonoBehaviour
    {

        private float _speedDashing;
        private float _accelerationDashing;
        private float _decelerationDashing;
        private float _steerDashing;
        private float _distanceToDash;

        private float _stopAtDashing = 3f;      //this won't be given by the outside, since the duck won't simply stop when close enough to the food,
                                                //but also when in water.


        private IdentifyFoodComponent _identifyFoodComponent;
        private MovementSeekComponent _movementSeekComponent;
        private LakeShopDescriptionComponent _lakeShopDescriptionComponent;
        private AnimalSoundController _animalSoundController;
        private Rigidbody2D _rigidbody2D;
        private float _dashTriggerProbability;
        private bool _destinationReached;



        private GameObject _foodObjective;  //You know what? At first I wanted to save just a Vector3. But with this, isn't it more scalable?
                                            //I mean, if tomorrow we decide to move the bread, we can simply read the value of the transform
                                            //of this object
        private Vector3 _foodObjectivePosition;     //...but just to make sure, I'll save it here




        private GameObject _obstaclesGO;
        private List<GameObject> _obstaclesList;


        public void Initialize(float dashTriggerProbability, float speed, float acceleration, float deceleration, float steer, float distanceToDash)
        {
            _dashTriggerProbability = dashTriggerProbability;
            _speedDashing = speed;
            _accelerationDashing = acceleration;
            _decelerationDashing = deceleration;
            _steerDashing = steer;
            _distanceToDash = distanceToDash;

            _destinationReached = true;

        }




        //############################################################# ACTIONS #############################################################

        public void EnterDashing_SaveDestination()
        {
            _foodObjective = _identifyFoodComponent.GetObjectiveFood();
            _foodObjectivePosition = _foodObjective.transform.position;
            _destinationReached = false;
            _currentRotation = _movementSeekComponent.GetRotation();
            SetAcceleration_Increment();

            _movementSeekComponent.CompletelyStopMoving();
        }

        public void EnterDashing_DisableCollisionsWithObstacles()
        {
            foreach(GameObject obstacle in _obstaclesList)
            {
                Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), obstacle.GetComponent<CompositeCollider2D>(), true);
            }
        }

        public void EnterDashing_PlaySound()
        {
            _animalSoundController.Fly();
        }

        public void ExitDashing_EnableCollisionsWithObstacles()
        {
            foreach (GameObject obstacle in _obstaclesList)
            {
                Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), obstacle.GetComponent<CompositeCollider2D>(), false);
            }
            _destinationReached = true;
        }

        public void ExitDashing_StopSound()
        {
            _animalSoundController.UnFly();
        }



        //############################################################# TRANSITIONS #############################################################

        public bool WantsToDashTowardsObjectiveFood()
        {
            if (_dashTriggerProbability == 0f) return false;

            return _identifyFoodComponent.IsThereAnObjectiveFood() && 
                (Random.Range(0f, 1f) <= _dashTriggerProbability) && 
                (Vector2.Distance(transform.position, _identifyFoodComponent.GetObjectiveFood().transform.position) >= _distanceToDash);
        }

        //needs to be changed in the future
        public bool IsDestinationReached()
        {
            return _destinationReached;
        }





        //############################################################# UTILITIES #############################################################
        void Awake()
        {
            _identifyFoodComponent = GetComponent<IdentifyFoodComponent>();
            _obstaclesList = new List<GameObject>();
        }

        // Start is called before the first frame update
        void Start()
        {
            _obstaclesGO = GameObject.Find("Obstacles").transform.GetChild(0).gameObject;
            _movementSeekComponent = GetComponent<MovementSeekComponent>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _lakeShopDescriptionComponent = GameObject.Find("WholeLake").GetComponent<LakeShopDescriptionComponent>();

            GetAllObstaclesGameObjects(_obstaclesGO, _obstaclesList);
            StartCoroutine(UpdateObstacles());

            _animalSoundController = GetComponent<AnimalSoundController>();
        }

        //I'm so sorry but rn I don't want to do anything difficult to do something so simple
        private IEnumerator UpdateObstacles()
        {
            yield return new WaitForSeconds(0.05f);
            _obstaclesList.RemoveAll(x => !x.activeSelf);
            //In this way I only work with active obstacles
            yield return null;
        }

        private void GetAllObstaclesGameObjects(GameObject obstacleGO, List<GameObject> ret)
        {
            if(obstacleGO.transform.childCount == 0)
            {
                ret.Add(obstacleGO);
            }

            foreach(Transform child in obstacleGO.transform)
            {
                GetAllObstaclesGameObjects(child.gameObject, ret);
            }
        }


        //#################################################################### THE ACTUAL DASHING ####################################################################

        //I'd like to use the MovementSeekComponent, but this is a different logic unfortunately. So, I'll write the code here,
        //even though it takes inspiration from MovementSeekComponent.

        //STRAIGHT FROM MOVEMENT SEEK COMPONENT BABY
        private float _currentAcceleration = 0f;
        //set the acceleration to max immediately
        public void SetAcceleration_Max()
        {
            _currentAcceleration = _accelerationDashing;
        }

        //set the acceleration to a low value that increases over time
        public void SetAcceleration_Increment()
        {
            _currentAcceleration = 1f;
            StartCoroutine(BringCurrentAccelerationToMax());
        }

        private IEnumerator BringCurrentAccelerationToMax()
        {
            float waitTime = 0.2f;
            float delta = 1f;
            while (_currentAcceleration < _accelerationDashing)
            {
                _currentAcceleration += delta;
                yield return new WaitForSeconds(waitTime);
            }
            yield return null;
        }

        private Vector2 _directionToMoveNormalized;
        private float _currentRotation;
        private void FixedUpdate()
        {
            if(IsDestinationReached() == false)
            {
                _directionToMoveNormalized = (new Vector2(_foodObjectivePosition.x, _foodObjectivePosition.y) - (new Vector2(transform.position.x, transform.position.y)));
                _directionToMoveNormalized.Normalize();

                float angle = Mathf.Atan2(-_directionToMoveNormalized.x, _directionToMoveNormalized.y) * Mathf.Rad2Deg;
                _currentRotation = angle;

                float force = _currentAcceleration * _steerDashing;
                if (_rigidbody2D.velocity.magnitude <= _speedDashing)
                {
                    _rigidbody2D.AddForce(_directionToMoveNormalized * force, ForceMode2D.Force);
                }
                _rigidbody2D.velocity = Vector2.ClampMagnitude(_rigidbody2D.velocity, _speedDashing);



                //When do I stop? When I have reached my objective and I am above water, so I can land easily
                if(Vector2.Distance(_foodObjectivePosition, transform.position) <= _stopAtDashing &&
                    _lakeShopDescriptionComponent.Contains(transform.position))
                {
                    _destinationReached = true;
                    _movementSeekComponent.Deceleration = _decelerationDashing;
                    _movementSeekComponent.StopMoving();
                }


            }
        }



    }
}
