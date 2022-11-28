using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using BreadNamespace;
using Enemies;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class EnemyFSM : MonoBehaviour
    {
        public Species MySpecies;
        [SerializeField] private CollisionManager collisionManager;
        [SerializeField] private MovementManager movementManager;
        [SerializeField] private EatingManager eatingManager;

        private Coroutine _movingCoroutineVar,
            _temporaryIdleCoroutine,
            _accelerateCoroutine,
            _eatBreadCoroutine,
            _chasingPlayerCoroutine,
            _decelerateCoroutine,
            _steerForBreadCoroutine;

        [SerializeField] private float stealingCd, stealingPerc;

        [SerializeField] private GameObject playerGameObjectToChase;
        private bool _justFinishedEating;

        public ActionState State=ActionState.Init;




        private CollectBreadScript _collectBreadScript;

        void Start(){
            ChangeState(ActionState.Roaming);
        }

        private void Awake(){
            stealingCd = MySpecies.stealingCd;
            _collectBreadScript = gameObject.GetComponentInChildren<CollectBreadScript>();
        }

        public void ChangeState(ActionState newState){
            Debug.Log("State: " + State + " -> " + newState);
            if (State == ActionState.Roaming){
                movementManager.StopRoaming();
                movementManager.StopMovementRelatedCoroutine(CoroutineType.Idle);
            }
            
            if (State == ActionState.MovingToBread){
                if(newState!=ActionState.Eating) movementManager.StopMoving(); //coroutine already stopped in the switch case below
                movementManager.StopMovementRelatedCoroutine(CoroutineType.SteerForBread);
                movementManager.StopMovementRelatedCoroutine(CoroutineType.Recovery);
            }

            if (State == ActionState.Chasing){
            }

            if (State == ActionState.Eating){
                if (newState == ActionState.GettingRobbed){
                    eatingManager.GetRobbed();
                }
                else{
                    collisionManager.ResetBreadTarget();
                    collisionManager.TurnOffColliders();
                    collisionManager.TurnOnColliders();
                    _collectBreadScript.ResetCollider();
                }
            }

            if (State == ActionState.Chilling){
                movementManager.StopChilling();
            }

            switch (newState){
                case ActionState.Chasing:
                    State = ActionState.Chasing;
                    movementManager.ChasePlayer(playerGameObjectToChase);
                    movementManager.StartMovementRelatedCoroutine(CoroutineType.Chase);
                    break;
                case ActionState.Roaming:
                    State = ActionState.Roaming;
                    movementManager.StartMovementRelatedCoroutine(CoroutineType.Idle);
                    break;
                case ActionState.Dashing:
                    State = ActionState.Dashing;
                    break;
                case ActionState.MovingToBread:
                    //todo: capita che entra in sto stato e poi si bugga
                    if(State==ActionState.Roaming) movementManager.StopMovementRelatedCoroutine(CoroutineType.Moving);
                    movementManager.StartMovementRelatedCoroutine(CoroutineType.Moving);
                    State = ActionState.MovingToBread;
                    movementManager.StartMovementRelatedCoroutine(CoroutineType.Recovery);
                    break;
                case ActionState.Eating:
                    //if(State!=ActionState.MovingToBread) 
                    movementManager.StopMoving(); //todo: questo potreebbe creare casini nel caso in cui si imbatta casualmente nel pane
                    State = ActionState.Eating;
                    break;
                case ActionState.Stealing:
                    State = ActionState.Stealing;
                    break;
                case ActionState.GettingRobbed:
                    State = ActionState.GettingRobbed;
                    break;
                case ActionState.Chilling:
                    State = ActionState.Chilling;
                    movementManager.StartChilling();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        public enum ActionState
        {
            Init,
            Chasing,
            Roaming,
            Dashing,
            MovingToBread,
            Eating,
            Stealing,
            GettingRobbed,
            Chilling //after eating food
        }

        public void TargetBread(GameObject breadGameObject){
            movementManager.MoveToBread(breadGameObject);
        }

        public void StartEatingBread(GameObject breadGameObject){
            BreadNamespace.BreadInWaterComponent breadInWaterComponent = breadGameObject.GetComponent<BreadNamespace.BreadInWaterComponent>();
            BreadNamespace.BreadInMouthComponent breadAboutToBeEaten = breadInWaterComponent.GenerateNewBreadInMouth(MySpecies.mouthSize).GetComponent<BreadNamespace.BreadInMouthComponent>();
            eatingManager.StartEatingBread(breadAboutToBeEaten);
            ChangeState(ActionState.Eating);
        }
    
        public void LevelFinished(){
            //todo: vola fuori dal lago e poi distruggo il game object
        }

        public bool IsEating(){
            return eatingManager.IsEating();
        }

        public BreadInMouthComponent StartGettingRobbed(Vector3 positionToBeIn){
            ChangeState(ActionState.GettingRobbed);
            movementManager.GoTo(positionToBeIn);
            return eatingManager.BreadInMouth;
        }

        public void AssignBreadAfterRobbery(BreadInMouthComponent newBread){
            eatingManager.StartEatingAfterRobbery(newBread);
        }
    }
}
