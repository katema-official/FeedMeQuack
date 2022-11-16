using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovementManager : MonoBehaviour
{

    [SerializeField] private EnemyFSM enemyFsm;

    [SerializeField] private float _speedPerc;
    
    private float _maxSpeed, _accelerationTimeSeconds, _idleTime, _idleCD, _deactivationRange;
    
    private Coroutine _movingCoroutineVar, _temporaryIdleCoroutine, _accelerateCoroutine, _decelerateCoroutine, _steerForBreadCoroutine, _chasingPlayerCoroutine;
    
    private Vector3 _movingVector;

    private GameObject _enemyGameObject,_breadTargeted, _parentGameObject;


    private void Awake(){
        _parentGameObject = gameObject.transform.parent.gameObject;
        Species species = enemyFsm.Species;
        _maxSpeed = species.maxSpeed;
        _accelerationTimeSeconds = species.accelerationTimeSeconds;
        _idleTime = species.idleTime;
        _idleCD = species.idleCD;
        _deactivationRange = species.outerRadiusCollider * 1.5f;
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
        if(_decelerateCoroutine!=null) StopCoroutine(_decelerateCoroutine);
        _speedPerc = 0;
        while (_speedPerc<1){
            _speedPerc += 0.01f;
            yield return new WaitForSeconds(_accelerationTimeSeconds/100);
        }

        yield return null;
    }

    private void ChangeDirection(){
        float rng = Random.Range(0,8);
        _movingVector= new Vector2( _maxSpeed, 0);
        _movingVector = Quaternion.AngleAxis(rng*45, Vector3.forward)* _movingVector;
    }
    
    

    private IEnumerator TemporaryIdleCoroutine(){
        while (enemyFsm.State== EnemyFSM.ActionState.Roaming){
            yield return new WaitForSeconds(_idleCD);
            StopRoaming();
            yield return new WaitForSeconds(_idleTime);
            StartMovement();
            yield return null;
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
        while (_speedPerc>0){
            _speedPerc -= 0.01f;
            yield return new WaitForSeconds(_accelerationTimeSeconds/100);
        }
        StopCoroutine(_movingCoroutineVar);
        yield return null;
    }

    public void MoveToBread(GameObject breadGameObject){
        _breadTargeted = breadGameObject;
        _steerForBreadCoroutine = StartCoroutine(SteerForBreadCoroutine());
        _speedPerc = 1;
        if(enemyFsm.State!=EnemyFSM.ActionState.MovingToBread)  enemyFsm.ChangeState(EnemyFSM.ActionState.MovingToBread);
    }

    private IEnumerator SteerForBreadCoroutine(){
        float startTime = Time.time;
        while (_breadTargeted!=null && Distance(_breadTargeted)>0 ){ //todo: vedere che succede nel momento in cui il pane che punto viene mangiato da un altro
            Vector2 vecToAdd = Direction(_breadTargeted);
            float delta = Time.time - startTime;
            _movingVector = AddForceToMovingVector(vecToAdd, delta);
            yield return new WaitForSeconds(0.001f);
            /*if (_breadTargeted!=null && Distance(_breadTargeted) > outerRadiusCollider){
                Debug.Log("dist targeted: "+ Distance(_breadTargeted));
                _breadTargeted = null;
                enemyFsm.ChangeState(EnemyFSM.ActionState.Roaming);
                //todo: reset dei collider
            }*/
        }

        yield return null;
    }

    private Vector2 NormalizeToMaxSpeed(Vector2 vectorToNormalize){
        float x1 = vectorToNormalize.x, y1 = vectorToNormalize.y;
        float normalizationFactor = _maxSpeed/math.sqrt(x1 * x1 + y1 * y1);
        Vector2 normalizedVector = new Vector2(x1 *normalizationFactor, y1 *normalizationFactor);
        return normalizedVector;
    }

    private Vector2 AddForceToMovingVector(Vector2 vecToAdd, float deltaT){
        vecToAdd = NormalizeToMaxSpeed(vecToAdd);
        float timeAdj = deltaT /2000;
        Vector2 weakerVector = new Vector2(vecToAdd.x*timeAdj, vecToAdd.y* timeAdj);
        Vector2 newVector = _movingVector + (Vector3) weakerVector;
        return NormalizeToMaxSpeed(newVector);
    }

    private float Distance(GameObject destinationGameObject){
        Vector3 dest = destinationGameObject.transform.position;
        return math.distance(dest, transform.position);
    }

    private Vector2 Direction(GameObject destinationGameObject){
        var destination = destinationGameObject.transform.position;
        var currentPosition = _parentGameObject.transform.position;
        return new Vector2(destination.x - currentPosition.x,destination.y - currentPosition.y);
    }

    public void StartMovementRelatedCoroutine(CoroutineType coroutineType){
        switch (coroutineType){
            case CoroutineType.Moving:
                _movingCoroutineVar= StartCoroutine(MovingCoroutine());
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
            default:
                throw new ArgumentOutOfRangeException(nameof(coroutineType), coroutineType, null);
        }
    }
    
    public void StopMovementRelatedCoroutine(CoroutineType coroutineType){
        switch (coroutineType){
            case CoroutineType.Moving:
                
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
            default:
                throw new ArgumentOutOfRangeException(nameof(coroutineType), coroutineType, null);
        }
    }

    private IEnumerator ChasePlayerCoroutine(GameObject playerGameObject){
        PlayerDuck playerDuck = playerGameObject.GetComponent<PlayerDuck>();
        while (enemyFsm.State == EnemyFSM.ActionState.Chasing && Distance(playerGameObject) < _deactivationRange && playerDuck.FoodInMouth > 0){
            _movingVector = Direction(playerGameObject);
            yield return null;
        }
    }

    public void ChasePlayer(GameObject playerGameObjectToChase){
        StartMovementRelatedCoroutine(CoroutineType.Chase);
        _movingCoroutineVar = StartCoroutine(MovingCoroutine());
        _chasingPlayerCoroutine = StartCoroutine(ChasePlayerCoroutine(playerGameObjectToChase));
    }
}
