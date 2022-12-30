using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BreadNamespace;
using Player;

namespace DuckEnemies
{

    public class StealingComponent : MonoBehaviour
    {

        //this component takes car of the states:
        //StealingPassive
        //StealingActive

        private EatingComponent _eatingComponent;

        //private bool _playerStartedStealingMe;

        private Rigidbody2D _rigidbody2D;

        private bool _currentlyBeingRobbed = false;
        private bool _currentlyStealingThePlayer = false;
        private BreadInMouthComponent _breadInMouthComponentAfterStealingPassive;




        private ChasingComponent _chasingComponent;

        //STEALING PASSIVE
        //Since me and the boys (kek) defined some interfaces beforehand (before I had to re-make the enemies), I have to respect them.
        //This is why this method needs to expose some methods that don't seem completely appropriate here



        //############################################################# ACTIONS #############################################################
        public void EnterStealingPassive_ResetVariables()
        {
            _breadInMouthComponentAfterStealingPassive = null;
            _currentlyBeingRobbed = true;
        }

        public void ExitStealingPassive_ResetVariables()
        {
            _breadInMouthComponentAfterStealingPassive = null;
            _currentlyBeingRobbed = false;
        }

        //############################################################# TRANSITIONS #############################################################

        //for going to hubState
        public bool StealingPassive_WasAllFoodStolen()
        {
            if (_currentlyBeingRobbed == true) return false;    //I can't tell if all the food was stolen until the stealingPassive state has terminated
            return _breadInMouthComponentAfterStealingPassive == null;
        }

        public bool StealingPassive_DoIHaveSomeFoodLeft()
        {
            if (_currentlyBeingRobbed == true) return false;    //I can't tell if some food was not stolen until the stealingPassive state has terminated
            return _breadInMouthComponentAfterStealingPassive != null;
        }



        //############################################################# UTILITIES #############################################################
        //################################################ (THE INTERFACES FOR THE PLAYER ;) ) ################################################

        public bool IsEating()
        {
            return _eatingComponent.IsEating();
        }

        public BreadInMouthComponent StartGettingRobbed(Vector3 pos)
        {
            //_playerStartedStealingMe = true;
            _eatingComponent.Disturb();
            transform.position = pos;           //mmmmmmmmmmmmhhhhhh I don't like this so much but for now...
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            return _eatingComponent.GetBreadInMouthComponent();
        }

        //called both after the player steals from the enemy, and the enemy steals from the player
        public void AssignBreadAfterRobbery(BreadInMouthComponent breadInMouthComponentAfterStealingPassive)
        {
            Debug.Log("AAAAA");
            _breadInMouthComponentAfterStealingPassive = breadInMouthComponentAfterStealingPassive;
            if (_breadInMouthComponentAfterStealingPassive)
            {
                _eatingComponent.DirectlySetFoodInMouth(_breadInMouthComponentAfterStealingPassive.gameObject);
            }
            else
            {
                _eatingComponent.DirectlySetFoodInMouth(null);
            }
            _currentlyBeingRobbed = false;
            _currentlyStealingThePlayer = false;
        }





        void Awake()
        {
            //_playerStartedStealingMe = false;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _eatingComponent = GetComponent<EatingComponent>();
            _chasingComponent = GetComponent<ChasingComponent>();
        }








        //TRY STEAL ACTIVE & STEALING ACTIVE

        //############################################################# ACTIONS #############################################################

        public void EnterTryStealActive_StealPlayer()
        {
            _chasingComponent.GetPlayer().gameObject.GetComponent<PlayerStealSkill>().Steal(this);
            Debug.Log("After stealing the player: " + _chasingComponent.GetPlayer().GetState());
        }

        public void EnterStealingActive_ResetVariables()
        {
            Debug.Log("Entered stealingActive");
            _breadInMouthComponentAfterStealingPassive = null;
            _currentlyStealingThePlayer = true;
        }

        public void ExitStealingActive_ResetVariables()
        {
            _breadInMouthComponentAfterStealingPassive = null;
            _currentlyStealingThePlayer = false;
        }




        //############################################################# TRANSITIONS #############################################################

        public bool DoesPlayerHaveFoodInMouth()
        {
            return _chasingComponent.GetPlayer().GetState() == PlayerState.Eating;
        }

        public bool IsPlayerVictimOfStealing()
        {
            return _chasingComponent.GetPlayer().GetState() == PlayerState.GettingRobbed;
        }


        public bool StealingActive_EnemyStoleSomeFood()
        {
            if (_currentlyStealingThePlayer == true) return false;    //I can't tell if some of the food was stolen until the stealingActive state has terminated
            return _breadInMouthComponentAfterStealingPassive != null;
        }

        public bool StealingActive_EnemyDidNotStoleAnyFood()
        {
            if (_currentlyStealingThePlayer == true) return false;    //I can't tell if no food was not stolen until the stealingActive state has terminated
            return _breadInMouthComponentAfterStealingPassive == null;
        }


    }

}