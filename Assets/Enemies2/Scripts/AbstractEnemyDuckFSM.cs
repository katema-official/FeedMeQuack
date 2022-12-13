using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BreadNamespace;
using FSMNamespace;


namespace DuckEnemies
{

    public class AbstractEnemyDuckFSM : MonoBehaviour
    {
        //First of all, let's define the characteristics of a duck enemy, at an abstract level
        //NB: some values might be, in the end, the same. We nevertheless declare them as separate to be general

        //First, we define a set of variables whose value can be set by the developer. Then we define other variables/data structures
        //that will be used at runtime

        //####################################### ROAMING AND SEEKING (SPEED, ACCELERATION, DECELERATION, STEER...) #######################################

        [SerializeField] protected float _speedRoaming;                        //max speed, acceleration, deceleration and steer 
        [SerializeField] protected float _accelerationRoaming;                 //at which the duck moves when roaming
        [SerializeField] protected float _decelerationRoaming;
        [SerializeField] protected float _steerRoaming;
        [SerializeField] protected float _chillingTime;

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



        [SerializeField] protected EnemyDuckSO _myEnemyDuckDescription;



        //these sets save up the bread that was refused. Each bread gameobject is recorded as it InstanceID (GetInstanceID())
        protected HashSet<int> _refusedBreadsCircle1;
        protected HashSet<int> _refusedBreadsCircle2;
        protected HashSet<int> _refusedBreadsCircle3;

        protected GameObject _breadInWaterObjectiveGO;
        protected BreadInMouthComponent _breadInMouthBeingEaten;

        protected EnemyDuckFSMEnumState.State _state;

        protected FSM _fsm;
        protected float _reactionTimeFSM = 0.1f;


        //the other components of this gameobject (we can't have all the code in one place!)
        protected RoamingComponent _roamingComponent;



        void Awake()
        {
            _refusedBreadsCircle1 = new HashSet<int>();
            _refusedBreadsCircle2 = new HashSet<int>();
            _refusedBreadsCircle3 = new HashSet<int>();
            _breadInWaterObjectiveGO = null;
            _breadInMouthBeingEaten = null;
            _state = EnemyDuckFSMEnumState.State.HubState;

            _roamingComponent = GetComponent<RoamingComponent>();
            _roamingComponent.Initialize(_speedRoaming, _accelerationRoaming, _decelerationRoaming, _steerRoaming, _chillingTime);



            //Initialization of the FSM

            //FIRST: define each state with its actions: enter action, stay actions and exit actions
            FSMState hubState = new FSMState();
            hubState.enterActions.Add(EnterHubState_CleanVariables);

            FSMState chilling = new FSMState();
            chilling.enterActions.Add(_roamingComponent.EnterChilling_ChooseChillingTime);
            chilling.exitActions.Add(_roamingComponent.ExitChilling);

            FSMState roaming = new FSMState();




            //SECOND: define the transition between states

            //actually, this is the last transition for hubState. If it isn't possible to go in any other state, go in this
            FSMTransition hubState_to_chilling = new FSMTransition(GoToChill, new FSMAction[] { () => _state = EnemyDuckFSMEnumState.State.Chilling });
            hubState.AddTransition(hubState_to_chilling, chilling);

            FSMTransition chilling_to_roaming = new FSMTransition(_roamingComponent.GetChillEnded, new FSMAction[] { () => _state = EnemyDuckFSMEnumState.State.Roaming });
            chilling.AddTransition(chilling_to_roaming, roaming);




            _fsm = new FSM(hubState);
            StartCoroutine(RunFSM());


        }







        //############################################################# ACTIONS #############################################################

        //Enter method for HubState. It's used to be sure that every variable is in the state it should be
        protected void EnterHubState_CleanVariables()
        {
            _breadInWaterObjectiveGO = null;
            _breadInMouthBeingEaten = null;
        }



        //############################################################# TRANSITIONS #############################################################

        protected bool GoToChill()
        {
            return true;
        }









        //#######################################################################################################################################
        //########################################### METHODS USED JUST TO UPDATE THE _STATE VARIABLE ###########################################
        //#######################################################################################################################################
        //E invece no perch� ho imparato a usare le lambda HAHAHAHA







        protected IEnumerator RunFSM()
        {
            while (true)
            {
                _fsm.Update();
                yield return new WaitForSeconds(_reactionTimeFSM);
                Debug.Log("_state = " + _state);
            }
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