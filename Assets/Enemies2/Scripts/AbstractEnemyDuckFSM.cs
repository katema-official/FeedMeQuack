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
        [SerializeField] protected float _desiredRoamingDistance;
        [SerializeField] protected float _stopAtRoaming;

        [SerializeField] protected float _speedFoodSeeking;                        //max speed, acceleration, deceleration and steer
        [SerializeField] protected float _accelerationFoodSeeking;                 //at which the duck moves when going after a piece 
        [SerializeField] protected float _decelerationFoodSeeking;                 //of bread
        [SerializeField] protected float _steerFoodSeeking;
        [SerializeField] protected float _stopAtFoodSeeking;

        //####################################### CIRCLES THAT DEFINE WHAT THE DUCK SEES #######################################

        [SerializeField] protected float _circle1FoodRadius;                  //the radius of the three circles that define if a enemy
        [SerializeField] protected float _circle2FoodRadius;                  //duck saw a piece of bread
        [SerializeField] protected float _circle3FoodRadius;
        [SerializeField] [Range(0.0f, 1.0f)] protected float _circle1FoodProbability;  //probabilities that a duck sees a piece of bread
        [SerializeField] [Range(0.0f, 1.0f)] protected float _circle2FoodProbability;  //when it spawns inside a particular circle
        [SerializeField] [Range(0.0f, 1.0f)] protected float _circle3FoodProbability;

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
        [SerializeField] protected float _digestingTime;


        [SerializeField] protected EnemyDuckSO _myEnemyDuckDescription;




        protected EnemyDuckFSMEnumState.State _state;

        protected FSM _fsm;
        protected float _reactionTimeFSM = 0.05f;


        //the other components of this gameobject (we can't have all the code in one place!)
        protected RoamingComponent _roamingComponent;
        protected IdentifyFoodComponent _identifyFoodComponent;
        protected FoodSeekingComponent _foodSeekingComponent;
        protected DashingComponent _dashingComponent;
        protected EatingComponent _eatingComponent;



        void Awake()
        {
            //Initialization of internal data structures and variables
            _state = EnemyDuckFSMEnumState.State.HubState;


            _speedRoaming = _myEnemyDuckDescription.SpeedRoaming;
            _accelerationRoaming = _myEnemyDuckDescription.AccelerationRoaming;
            _decelerationRoaming = _myEnemyDuckDescription.DecelerationRoaming;
            _steerRoaming = _myEnemyDuckDescription.SteerRoaming;
            _chillingTime = _myEnemyDuckDescription.ChillingTime;
            _desiredRoamingDistance = _myEnemyDuckDescription.DesiredRoamingDistance;
            _stopAtRoaming = _myEnemyDuckDescription.StopAtRoaming;

            _speedFoodSeeking = _myEnemyDuckDescription.SpeedFoodSeeking;
            _accelerationFoodSeeking = _myEnemyDuckDescription.AccelerationFoodSeeking;
            _decelerationFoodSeeking = _myEnemyDuckDescription.DecelerationFoodSeeking;
            _steerFoodSeeking = _myEnemyDuckDescription.SteerFoodSeeking;
            _stopAtFoodSeeking = _myEnemyDuckDescription.StopAtFoodSeeking;

            _circle1FoodRadius = _myEnemyDuckDescription.Circle1FoodRadius;
            _circle2FoodRadius = _myEnemyDuckDescription.Circle2FoodRadius;
            _circle3FoodRadius = _myEnemyDuckDescription.Circle3FoodRadius;
            _circle1FoodProbability = _myEnemyDuckDescription.Circle1FoodProbability;
            _circle2FoodProbability = _myEnemyDuckDescription.Circle2FoodProbability;
            _circle3FoodProbability = _myEnemyDuckDescription.Circle3FoodProbability;

            _circle1PlayerRadius = _myEnemyDuckDescription.Circle1PlayerRadius;
            _circle2PlayerRadius = _myEnemyDuckDescription.Circle2PlayerRadius;
            _circle3PlayerRadius = _myEnemyDuckDescription.Circle3PlayerRadius;
            _circle1PlayerProbability = _myEnemyDuckDescription.Circle1PlayerProbability;
            _circle2PlayerProbability = _myEnemyDuckDescription.Circle2PlayerProbability;
            _circle3PlayerProbability = _myEnemyDuckDescription.Circle3PlayerProbability;

            _dashTriggerProbability = _myEnemyDuckDescription.DashTriggerProbability;
            _stealTriggerProbability = _myEnemyDuckDescription.StealTriggerProbability;

            _speedDash = _myEnemyDuckDescription.SpeedDash;
            _accelerationDash = _myEnemyDuckDescription.AccelerationDash;
            _decelerationDash = _myEnemyDuckDescription.DecelerationDash;
            _steerDash = _myEnemyDuckDescription.SteerDash;

            _speedChasing = _myEnemyDuckDescription.SpeedChasing;
            _accelerationChasing = _myEnemyDuckDescription.AccelerationChasing;
            _decelerationChasing = _myEnemyDuckDescription.DecelerationChasing;
            _steerChasing = _myEnemyDuckDescription.SteerChasing;

            _mouthSize = _myEnemyDuckDescription.MouthSize;
            _chewingRate = _myEnemyDuckDescription.ChewingRate;
            _digestingTime = _myEnemyDuckDescription.DigestingTime;


            _roamingComponent = GetComponent<RoamingComponent>();
            _roamingComponent.Initialize(_speedRoaming, _accelerationRoaming, _decelerationRoaming, _steerRoaming, _chillingTime, _desiredRoamingDistance, _stopAtRoaming);
            _identifyFoodComponent = GetComponent<IdentifyFoodComponent>();
            _identifyFoodComponent.Initialize(_circle1FoodRadius, _circle2FoodRadius, _circle3FoodRadius, _circle1FoodProbability, _circle2FoodProbability, _circle3FoodProbability);
            _foodSeekingComponent = GetComponent<FoodSeekingComponent>();
            _foodSeekingComponent.Initialize(_speedFoodSeeking, _accelerationFoodSeeking, _decelerationFoodSeeking, _steerFoodSeeking, _stopAtFoodSeeking);
            _dashingComponent = GetComponent<DashingComponent>();
            _dashingComponent.Initialize(_dashTriggerProbability);
            _eatingComponent = GetComponent<EatingComponent>();
            _eatingComponent.Initialize(_mouthSize, _chewingRate, _digestingTime);


            //Initialization of the FSM

            //FIRST: define each state with its actions: enter action, stay actions and exit actions
            FSMState hubState = new FSMState();
            hubState.enterActions.Add(EnterHubState_CleanVariables);

            FSMState chilling = new FSMState();
            chilling.enterActions.Add(_roamingComponent.EnterChilling_ChooseChillingTime);
            chilling.exitActions.Add(_roamingComponent.ExitChilling);

            FSMState roaming = new FSMState();
            roaming.enterActions.Add(_roamingComponent.EnterRoaming_ChooseRandomPath);
            roaming.enterActions.Add(_roamingComponent.EnterRoaming_SetSteeringBehaviour);
            roaming.stayActions.Add(_roamingComponent.StayRoaming_UpdateDestination);

            FSMState foodSeen = new FSMState();
            //Idk rn if there will be actions to perform when a piece of bread has been identified

            FSMState dashing = new FSMState();
            //Here I'll define the actions to perform when I'll have the dashing for real

            FSMState foodSeeking = new FSMState();
            foodSeeking.enterActions.Add(_foodSeekingComponent.EnterFoodSeeking_FindPath);
            foodSeeking.enterActions.Add(_foodSeekingComponent.EnterFoodSeeking_SetSteeringBehaviour);
            foodSeeking.enterActions.Add(_foodSeekingComponent.EnterFoodSeeking_SetFoodID);
            foodSeeking.stayActions.Add(_foodSeekingComponent.StayFoodSeeking_UpdateDestination);
            foodSeeking.exitActions.Add(_foodSeekingComponent.ExitFoodSeeking_DeletePath);
            foodSeeking.exitActions.Add(_foodSeekingComponent.ExitFoodSeeking_ResetFoodID);

            FSMState bite = new FSMState();
            bite.enterActions.Add(_eatingComponent.EnterBite_CleanVariables);
            bite.enterActions.Add(_eatingComponent.EnterBite_BiteBreadInWater);
            bite.exitActions.Add(_identifyFoodComponent.ForgetAboutAllFood);

            FSMState eating = new FSMState();
            eating.enterActions.Add(_eatingComponent.EnterEating_SetNotDisturbed);
            eating.enterActions.Add(_eatingComponent.EnterEating_StartEating);

            FSMState digesting = new FSMState();
            digesting.enterActions.Add(_eatingComponent.EnterDigesting_ChooseDigestingTime);
            digesting.exitActions.Add(_eatingComponent.ExitDigesting);
            




            //SECOND: define the transition between states
            FSMTransition chilling_to_roaming = new FSMTransition(_roamingComponent.GetChillEnded, 
                new FSMAction[] { () => _state = EnemyDuckFSMEnumState.State.Roaming });


            FSMTransition roaming_to_hubState = new FSMTransition(_roamingComponent.DestinationReachedRoaming, 
                new FSMAction[] { () => _state = EnemyDuckFSMEnumState.State.HubState });
            

            FSMTransition x_to_foodSeen = new FSMTransition(_identifyFoodComponent.IsThereAnObjectiveFood, 
                new FSMAction[] { () => _state = EnemyDuckFSMEnumState.State.FoodSeen });
            

            FSMTransition foodSeen_to_dashing = new FSMTransition(_dashingComponent.WantsToDashTowardsObjectiveFood, 
                new FSMAction[] { () => _state = EnemyDuckFSMEnumState.State.Dashing });
            FSMTransition foodSeen_to_foodSeeking = new FSMTransition(_identifyFoodComponent.IsThereAnObjectiveFood, 
                new FSMAction[] { () => _state = EnemyDuckFSMEnumState.State.FoodSeeking });


            FSMTransition dashing_to_bite = new FSMTransition(_dashingComponent.IsDestinationReached,
                new FSMAction[] { () => _state = EnemyDuckFSMEnumState.State.Bite });


            FSMTransition foodSeeking_to_hubState = new FSMTransition(() => !_identifyFoodComponent.IsThereAnObjectiveFood() || _foodSeekingComponent.HasFoodDisappeared(), //To check whether it's null or, if it's not, has changed (otherwise it wouldn't recompute the path)
                new FSMAction[] { () => _state = EnemyDuckFSMEnumState.State.HubState });
            FSMTransition foodSeeking_to_bite = new FSMTransition(_foodSeekingComponent.DestinationReachedFoodSeeking,
                new FSMAction[] { () => _state = EnemyDuckFSMEnumState.State.Bite });


            FSMTransition bite_to_hubState = new FSMTransition(() => !_eatingComponent.CanBiteTheFood(),
                new FSMAction[] { () => _state = EnemyDuckFSMEnumState.State.HubState });
            FSMTransition bite_to_eating = new FSMTransition(_eatingComponent.CanBiteTheFood,
                new FSMAction[] { () => _state = EnemyDuckFSMEnumState.State.Eating });


            FSMTransition eating_to_digesting = new FSMTransition(_eatingComponent.DidIFinishEating,
                new FSMAction[] { () => _state = EnemyDuckFSMEnumState.State.Digesting });
            //transition to stealingPassive


            FSMTransition digesting_to_hubState = new FSMTransition(_eatingComponent.GetDigestingEnded,
                new FSMAction[] { () => _state = EnemyDuckFSMEnumState.State.HubState });


            //actually, this is the last transition for hubState. If it isn't possible to go in any other state, go in this
            FSMTransition hubState_to_chilling = new FSMTransition(GoToChill, 
                new FSMAction[] { () => _state = EnemyDuckFSMEnumState.State.Chilling });
            

            hubState.AddTransition(x_to_foodSeen, foodSeen);
            //chasing transition
            hubState.AddTransition(hubState_to_chilling, chilling);

            chilling.AddTransition(x_to_foodSeen, foodSeen);
            chilling.AddTransition(chilling_to_roaming, roaming);

            roaming.AddTransition(x_to_foodSeen, foodSeen);
            //chasing transition
            roaming.AddTransition(roaming_to_hubState, hubState);

            foodSeen.AddTransition(foodSeen_to_dashing, dashing);
            foodSeen.AddTransition(foodSeen_to_foodSeeking, foodSeeking);

            dashing.AddTransition(dashing_to_bite, bite);

            foodSeeking.AddTransition(foodSeeking_to_hubState, hubState);
            foodSeeking.AddTransition(foodSeeking_to_bite, bite);

            bite.AddTransition(bite_to_eating, eating);
            bite.AddTransition(bite_to_hubState, hubState);

            eating.AddTransition(eating_to_digesting, digesting);
            //transition to stealingPassive

            digesting.AddTransition(digesting_to_hubState, hubState);


            _fsm = new FSM(hubState);
            StartCoroutine(RunFSM());


        }







        //############################################################# ACTIONS #############################################################

        //Enter method for HubState. It's used to be sure that every variable is in the state it should be
        protected void EnterHubState_CleanVariables()
        {
            //lol i moved everything away
        }



        //############################################################# TRANSITIONS #############################################################

        protected bool GoToChill()
        {
            return true;
        }









        //#######################################################################################################################################
        //########################################### METHODS USED JUST TO UPDATE THE _STATE VARIABLE ###########################################
        //#######################################################################################################################################
        //E invece no perché ho imparato a usare le lambda HAHAHAHA







        protected IEnumerator RunFSM()
        {
            while (true)
            {
                _fsm.Update();
                yield return new WaitForSeconds(_reactionTimeFSM);
                Debug.Log("_state = " + _state);
            }
        }






    }
}