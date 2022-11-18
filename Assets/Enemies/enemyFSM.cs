using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        private float _maxSpeed, _mouthSize, _eatingSpeed = 1;

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

        public Bread breadBeingEaten;
        [SerializeField] private GameObject breadTargeted;

        public ActionState State;
        [SerializeField] private Vector3 _movingVector;
        void Start(){
            //collisionManager.InitializeColliders(Species);
            _movingVector = new Vector2(_maxSpeed, 0);
            ChangeState(ActionState.Roaming);
        }

        private void Awake(){
            _maxSpeed = MySpecies.maxSpeed;
            _mouthSize = MySpecies.mouthSize;
            _eatingSpeed = MySpecies.eatingSpeed;
            stealingCd = MySpecies.stealingCd;
        }

        public void ChangeState(ActionState newState){
            Debug.Log("State: " + State + " -> " + newState);
            if (State == ActionState.Roaming){
                movementManager.StopRoaming();
                movementManager.StopMovementRelatedCoroutine(CoroutineType.Idle);
            }

            if (State == ActionState.Eating) StopCoroutine(EatBread(breadBeingEaten));
            if (State == ActionState.MovingToBread){
                breadTargeted = null;
                movementManager.StopMoving();
                movementManager.StopMovementRelatedCoroutine(CoroutineType.SteerForBread);
            }

            if (State == ActionState.Chasing){
            }

            if (State == ActionState.Eating){
                collisionManager.TurnOnColliders();
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
                    movementManager.StartMovement();
                    movementManager.StartMovementRelatedCoroutine(CoroutineType.Moving);
                    break;
                case ActionState.Dashing:
                    State = ActionState.Dashing;
                    break;
                case ActionState.MovingToBread:
                    movementManager.StartMovementRelatedCoroutine(CoroutineType.Moving);
                    State = ActionState.MovingToBread;
                    break;
                case ActionState.Eating:
                    collisionManager.TurnOffColliders();
                    movementManager.StopMoving(); //todo: questo potreebbe creare casini nel caso in cui si imbatta casualmente nel pane
                    breadTargeted = null;
                    State = ActionState.Eating;
                    break;
                case ActionState.Stealing:
                    State = ActionState.Stealing;
                    break;
                case ActionState.GettingRobbed:
                    State = ActionState.GettingRobbed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        public enum ActionState
        {
            Chasing,
            Roaming,
            Dashing,
            MovingToBread,
            Eating,
            Stealing,
            GettingRobbed
        }

        public void TargetBread(GameObject breadGameObject){
            breadTargeted = breadGameObject;
            movementManager.MoveToBread(breadGameObject);
        }

        public void StartEatingBread(GameObject breadGameObject){
            breadBeingEaten = breadGameObject.GetComponent<Bread>();
            ChangeState(ActionState.Eating);
            _eatBreadCoroutine = StartCoroutine(EatBread(breadBeingEaten));
            Destroy(breadGameObject);
        }

        IEnumerator EatBread(Bread bread){
            int i = 0;
            while (bread.BreadPoints > 0){
                bread.BreadPoints--;
                i++;
                yield return new WaitForSeconds(_eatingSpeed);
            }

            ChangeState(ActionState.Roaming);
        }
    }
}
