using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BreadNamespace;


namespace DuckEnemies
{

    public class AbstractEnemyDuckFSM : MonoBehaviour
    {
        //First of all, let's define the characteristics of a duck enemy, at an abstract level
        //NB: some values might be, in the end, the same. We nevertheless declare them as separate to be general

        //First, we define a set of variables whose value can be set by the developer. Then we define other variables/data structures
        //that will be used at runtime

        //####################################### SPEED, ACCELERATION, DECELERATION AND STEER #######################################

        [SerializeField] protected float _speedRoaming;                        //max speed, acceleration, deceleration and steer 
        [SerializeField] protected float _accelerationRoaming;                 //at which the duck moves when roaming
        [SerializeField] protected float _decelerationRoaming;
        [SerializeField] protected float _steerRoaming;

        [SerializeField] protected float _speedFoodSeeking;                        //max speed, acceleration, deceleration and steer
        [SerializeField] protected float _accelerationFoodSeeking;                 //at which the duck moves when going after a piece 
        [SerializeField] protected float _decelerationFoodSeeking;                 //of bread
        [SerializeField] protected float _steerFoodSeeking;

        //####################################### CIRCLES THAT DEFINE WHAT THE DUCK SEES #######################################

        [SerializeField] protected float _circle1BreadRadius;                  //the radius of the three circles that define if a enemy
        [SerializeField] protected float _circle2BreadRadius;                  //duck saw a piece of bread
        [SerializeField] protected float _circle3BreadRadius;
        [SerializeField] [Range(0.0f, 1.0f)] protected float _circle1BreadProbability;  //probabilities that a duck sees a piece of bread
        [SerializeField] [Range(0.0f, 1.0f)] protected float _circle2BreadProbability;  //when it spawns inside a particular circle
        [SerializeField] [Range(0.0f, 1.0f)] protected float _circle3BreadProbability;

        [SerializeField] protected float _circle1PlayerRadius;                  //the radius of the three circles that define if a enemy
        [SerializeField] protected float _circle2PlayerRadius;                  //duck saw the player eating a piece of bread
        [SerializeField] protected float _circle3PlayerRadius;
        [SerializeField] [Range(0.0f, 1.0f)] protected float _circle1PlayerProbability;  //probabilities that a duck sees the player when it is
        [SerializeField] [Range(0.0f, 1.0f)] protected float _circle2PlayerProbability;  //eating a piece of bread
        [SerializeField] [Range(0.0f, 1.0f)] protected float _circle3PlayerProbability;

        //####################################### PROBABILITY OF TRIGGERING ABILITIES AND GENERAL ABILITIES MANAGEMENT #######################################

        [SerializeField] [Range(0.0f, 1.0f)] protected float _dashTriggerProbability;   //probability of triggering the dash ability (when possible according to the FSM)
        [SerializeField] [Range(0.0f, 1.0f)] protected float _stealTriggerProbability;  //same as above, but for the stealing ability

        [SerializeField] protected float _speedDash;                           //max speed, acceleration, deceleration and steer
        [SerializeField] protected float _accelerationDash;                    //at which the duck moves when dashing
        [SerializeField] protected float _decelerationDash;
        [SerializeField] protected float _steerDash;

        [SerializeField] protected float _speedChasing;                        //max speed, acceleration, deceleration and steer
        [SerializeField] protected float _accelerationChasing;                 //at which the duck moves when going after the player 
        [SerializeField] protected float _decelerationChasing;                 //(because it wants to steal it)
        [SerializeField] protected float _steerChasing;

        //####################################### EATING MANAGEMENT #######################################

        [SerializeField] protected int _mouthSize;                             //how many BreadPoints this duck can have at once in its mouth
        [SerializeField] protected float _chewingRate;                         //measures in how many seconds (or fractions of seconds) a BreadPoint is consumed





        //these sets save up the bread that was refused. Each bread gameobject is recorded as it InstanceID (GetInstanceID())
        protected HashSet<int> _refusedBreadsCircle1;
        protected HashSet<int> _refusedBreadsCircle2;
        protected HashSet<int> _refusedBreadsCircle3;

        protected GameObject _breadInWaterObjectiveGO;
        protected BreadInMouthComponent _breadInMouthBeingEaten;

        protected EnemyDuckFSMEnumState.State _state;



        void Awake()
        {
            _refusedBreadsCircle1 = new HashSet<int>();
            _refusedBreadsCircle2 = new HashSet<int>();
            _refusedBreadsCircle3 = new HashSet<int>();
            _breadInWaterObjectiveGO = null;
            _breadInMouthBeingEaten = null;
            _state = EnemyDuckFSMEnumState.State.HubState;





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