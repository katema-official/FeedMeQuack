using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class MovementManager : MonoBehaviour
    {

        [SerializeField] private EnemyFSM enemyFsm;

        [SerializeField] private float _speedPerc = 0;

        private float _maxSpeed,
            _accelerationTimeSeconds,
            _decelerationTimeSeconds,
            _idleTime,
            _movementAtMaxSpeedDuration,
            _outerRadius,
            _chillingTime,
            _steeringValue;

        private Coroutine _movingCoroutineVar,
            _temporaryIdleCoroutine,
            _accelerateCoroutine,
            _decelerateCoroutine,
            _steerForBreadCoroutine,
            _chasingPlayerCoroutine,
            _chillingCoroutine,
            _recoveryCoroutine;

        [SerializeField] private Vector3 _movingVector;

        private GameObject _enemyGameObject, _breadTargeted, _parentGameObject;

        private LevelStageNamespace.LakeShopDescriptionComponent _lakeShopDescriptionComponent;


        private void Awake(){
            float rng1 = Random.Range(0.75f, 1.0f);
            float rng2 = Random.Range(0.9f, 1.0f);
            _parentGameObject = gameObject.transform.parent.gameObject;
            Species species = enemyFsm.MySpecies;
            _maxSpeed = species.maxSpeed * rng1;
            _accelerationTimeSeconds = species.accelerationTimeSeconds;
            _decelerationTimeSeconds = species.decelerationTimeSeconds;
            _idleTime = species.idleTime;
            _movementAtMaxSpeedDuration = species.movementAtMaxSpeedDuration;
            _outerRadius = species.outerRadiusCollider;
            _chillingTime = species.chillingTime * rng2;
            _steeringValue = species.steeringValue;
            _lakeShopDescriptionComponent = GameObject.Find("WholeLake").GetComponent<LevelStageNamespace.LakeShopDescriptionComponent>();
        }


        public void StartMovement(){
            ChangeDirection();
            _movingCoroutineVar = StartCoroutine(MovingCoroutine());
        }

        private IEnumerator MovingCoroutine(){
            _accelerateCoroutine = StartCoroutine(AccelerateCoroutine());
            while (true){
                _parentGameObject.transform.position += _movingVector * Time.deltaTime * _speedPerc;
                yield return null;
            }
        }

        private IEnumerator AccelerateCoroutine(){
            if (_decelerateCoroutine != null){
                StopCoroutine(_decelerateCoroutine);
                StopCoroutine(_movingCoroutineVar);
                //todo: check se rompe qualcosa, in teoria dovrebbe fixare tipo tutto. In alternativa, fare in modo che la decelerate corotine chiami un metodo che aspetta decelerateTime e poi
                //blocchi la moving coroutine, piuttosto che farlo alla fine della coroutine stessa, dato che potrebbe venire fermata a metà esecuzione e creare problemi.
            }

            while (_speedPerc < 1){
                _speedPerc += 0.01f;
                yield return new WaitForSeconds(_accelerationTimeSeconds / 100);
            }

            yield return null;
        }

        private void ChangeDirection(){
            bool wouldHitBorder = true;
            while (wouldHitBorder){
                _movingVector = new Vector2(_maxSpeed, 0);
                float rng = Random.Range(0, 8);
                _movingVector = Quaternion.AngleAxis(rng * 45, Vector3.forward) * _movingVector;
                wouldHitBorder = CheckIfMovementWouldHitBorder();
            }
        }

        private bool CheckIfMovementWouldHitBorder(){
            //return false;
            //float distanceToTravel = _maxSpeed * _movementDuration * _movMultiplier;
            //Vector3 finalDestination = _parentGameObject.transform.position + (_movingVector * _movementDuration * _movMultiplier);//directionToEvaluate * distanceToTravel;
            Vector3 movementVectorAtMaxSpeed = _movingVector * _movementAtMaxSpeedDuration;
            Vector3 finalDestination = _parentGameObject.transform.position + movementVectorAtMaxSpeed * 1.1f;

            return !_lakeShopDescriptionComponent.Contains(finalDestination);   //return
            //return false;
        }


        private IEnumerator TemporaryIdleCoroutine(){
            while (enemyFsm.State == EnemyFSM.ActionState.Roaming){
                StartMovement();
                yield return new WaitForSeconds(_accelerationTimeSeconds +_movementAtMaxSpeedDuration);
                StopRoaming();
                yield return new WaitForSeconds(_idleTime + _decelerationTimeSeconds);
            }

            yield return null;
        }

        public void StopRoaming(){
            StopMoving();
        }

        public void StopMoving(){
            _decelerateCoroutine = StartCoroutine(DecelerateCoroutine());
        }

        private IEnumerator DecelerateCoroutine(){
            //todo: problema, non stoppo la coroutine del movimento quando passo da roaming a moving to Bread
            StopCoroutine(_accelerateCoroutine);
            while (_speedPerc > 0){
                _speedPerc -= 0.01f;
                yield return new WaitForSeconds(_decelerationTimeSeconds / 100);
            }

            StopCoroutine(_movingCoroutineVar);
            yield return null;
        }

        public void MoveToBread(GameObject breadGameObject){
            _breadTargeted = breadGameObject;
            _steerForBreadCoroutine = StartCoroutine(SteerForBreadCoroutine());
            if (enemyFsm.State != EnemyFSM.ActionState.MovingToBread)
                enemyFsm.ChangeState(EnemyFSM.ActionState.MovingToBread);
        }

        private IEnumerator SteerForBreadCoroutine(){
            float startTime = Time.time;
            while (_breadTargeted != null){
                //todo: vedere che succede nel momento in cui il pane che punto viene mangiato da un altro
                Vector2 vecToAdd = Direction(_breadTargeted);
                float delta = Time.time - startTime;
                _movingVector = AddForceToMovingVector(vecToAdd, delta);
                yield return new WaitForSeconds(0.001f);
            }

            if (_breadTargeted == null && !enemyFsm.IsEating()){
                enemyFsm.ChangeState(EnemyFSM.ActionState.Chilling);
            }

            yield return null;
        }

        private Vector2 NormalizeToMaxSpeed(Vector2 vectorToNormalize){
            float x1 = vectorToNormalize.x, y1 = vectorToNormalize.y;
            float normalizationFactor = _maxSpeed / math.sqrt(x1 * x1 + y1 * y1);
            Vector2 normalizedVector = new Vector2(x1 * normalizationFactor, y1 * normalizationFactor);
            return normalizedVector;
        }

        private Vector2 AddForceToMovingVector(Vector2 vecToAdd, float deltaT){
            vecToAdd = NormalizeToMaxSpeed(vecToAdd);
            float timeAdj = deltaT / _steeringValue;
            Vector2 weakerVector = new Vector2(vecToAdd.x * timeAdj, vecToAdd.y * timeAdj);
            Vector2 newVector = _movingVector * _speedPerc + (Vector3) weakerVector;
            return NormalizeToMaxSpeed(newVector);
        }

        private float Distance(GameObject destinationGameObject){
            Vector3 dest = destinationGameObject.transform.position;
            return math.distance(dest, transform.parent.position);
        }

        private Vector2 Direction(GameObject destinationGameObject){
            var destination = destinationGameObject.transform.position;
            var currentPosition = _parentGameObject.transform.position;
            return new Vector2(destination.x - currentPosition.x, destination.y - currentPosition.y);
        }

        public void StartMovementRelatedCoroutine(CoroutineType coroutineType){
            switch (coroutineType){
                case CoroutineType.Moving:
                    _movingCoroutineVar = StartCoroutine(MovingCoroutine());
                    break;
                case CoroutineType.Idle:
                    _temporaryIdleCoroutine = StartCoroutine(TemporaryIdleCoroutine());
                    break;
                case CoroutineType.Accelerate:
                    break;
                case CoroutineType.Decelerate:
                    break;
                case CoroutineType.Eat:
                    break;
                case CoroutineType.Chase:
                    break;
                case CoroutineType.GoToBread:

                    break;
                case  CoroutineType.Recovery:
                    _recoveryCoroutine = StartCoroutine(RecoveryFromStandingStillCoroutine());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(coroutineType), coroutineType, null);
            }
        }

        public void StopMovementRelatedCoroutine(CoroutineType coroutineType){
            switch (coroutineType){
                case CoroutineType.Moving:
                    StopCoroutine(_movingCoroutineVar);
                    break;
                case CoroutineType.Idle:
                    StopCoroutine(_temporaryIdleCoroutine);
                    break;
                case CoroutineType.Accelerate:

                    break;
                case CoroutineType.Decelerate:

                    break;
                case CoroutineType.Eat:

                    break;
                case CoroutineType.Chase:

                    break;
                case CoroutineType.SteerForBread:
                    StopCoroutine(_steerForBreadCoroutine);
                    break;
                case CoroutineType.Recovery:
                    StopCoroutine(_recoveryCoroutine);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(coroutineType), coroutineType, null);
            }
        }

        private IEnumerator ChasePlayerCoroutine(GameObject playerGameObject){
            PlayerDuck playerDuck = playerGameObject.GetComponent<PlayerDuck>();
            while (enemyFsm.State == EnemyFSM.ActionState.Chasing && playerDuck.FoodInMouth > 0){
                _movingVector = Direction(playerGameObject);
                yield return null;
            }
        }

        public void ChasePlayer(GameObject playerGameObjectToChase){
            StartMovementRelatedCoroutine(CoroutineType.Chase);
            _movingCoroutineVar = StartCoroutine(MovingCoroutine());
            _chasingPlayerCoroutine = StartCoroutine(ChasePlayerCoroutine(playerGameObjectToChase));
        }

        public void StartChilling(){
            _chillingCoroutine = StartCoroutine(ChillingCoroutine());
        }

        private IEnumerator ChillingCoroutine(){
            float rng = Random.Range(0.75f, 1.25f);
            yield return new WaitForSeconds(_chillingTime* rng);
            if (enemyFsm.State == EnemyFSM.ActionState.Chilling){
                enemyFsm.ChangeState(EnemyFSM.ActionState.Roaming);
            }

            yield return null;
        }

        public void StopChilling(){
            StopCoroutine(_chillingCoroutine);
        }

        private IEnumerator RecoveryFromStandingStillCoroutine(){
            Vector3 oldPosition = _parentGameObject.transform.position;
            EnemyFSM.ActionState state = enemyFsm.State;
            while (true){
                if (state == EnemyFSM.ActionState.MovingToBread){
                    yield return new WaitForSeconds(1);
                    Vector3 newPosition = _parentGameObject.transform.position;
                    if (newPosition != oldPosition) oldPosition = newPosition;
                    else{
                        enemyFsm.ChangeState(EnemyFSM.ActionState.Roaming);
                    }
                }
            }
            yield return null;
        }

        public void GoTo(Vector3 positionToBeIn){
            _movingVector =NormalizeToMaxSpeed( positionToBeIn - _parentGameObject.transform.position);
            _movingCoroutineVar = StartCoroutine(MovingCoroutine());
            while (_parentGameObject.transform.position!= positionToBeIn){
                
            }
            StopCoroutine(_movingCoroutineVar);
            //_decelerateCoroutine = StartCoroutine(DecelerateCoroutine());
        }
    }
}
