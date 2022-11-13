using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyFSM : MonoBehaviour
{
    public Species Species;
    [SerializeField] private float _speedPerc, _accelerationTimeSeconds;
    private float _maxSpeed, _mouthSize, _eatingSpeed=1, _steeringSpeed, _idleTime, _idleCD;

    private Coroutine _steeringCoroutineVar, _movingCoroutineVar, _temporaryIdleCoroutine, _accelerateCoroutine,_eatBreadCoroutine, _chasingPlayerCoroutine, _decelerateCoroutine;
    
    [SerializeField] private float steeringAngleMax=45;
    [SerializeField] private float steeringCd;
    [SerializeField] private float innerRadiusCollider, mediumRadiusCollider, outerRadiusCollider;
    [SerializeField] private float stealingCd, stealingPerc;
    [SerializeField] private int percInnerRadius, percMediumRadius, percOuterRadius;

    [SerializeField] private GameObject playerGameObjectToChase;
    private bool _justFinishedEating;
 
    public Bread breadBeingEaten;
    [SerializeField] private GameObject breadTargeted;
    
    private ActionState _state;
    [SerializeField] private Vector3 _movingVector;
    
    // Start is called before the first frame update
    void Start(){
        _movingVector = new Vector2(16, 0);
        ChangeState(ActionState.Roaming);
    }

    private void Awake(){
        _maxSpeed = Species.maxSpeed;
        _accelerationTimeSeconds = Species.accelerationTimeSeconds;
        _mouthSize = Species.mouthSize;
        _eatingSpeed = Species.eatingSpeed;
        _steeringSpeed = Species.steeringSpeed;
        steeringAngleMax = Species.steeringAngleMax;
        steeringCd = Species.steeringCd;
        stealingCd = Species.stealingCd;
        percInnerRadius = Species.percInnerRadius;
        percMediumRadius = Species.percMediumRadius;
        percOuterRadius = Species.percOuterRadius;
        _idleTime = Species.idleTime;
        _idleCD = Species.idleCD;
    }

    // Update is called once per frame
    void Update(){
    }

    private void StartMovement(){
        //cambiare: si muove tot tempo, poi si ferma, poi riprende il movimento
        _movingCoroutineVar= StartCoroutine(MovingCoroutine());
        _steeringCoroutineVar= StartCoroutine(ChangingDirectionCoroutine());
    }

    private IEnumerator MovingCoroutine(){
        _accelerateCoroutine = StartCoroutine(AccelerateCoroutine());
        while (true){
            transform.position += _movingVector * Time.deltaTime * _speedPerc;
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

    private IEnumerator ChangingDirectionCoroutine(){
        while (_state==ActionState.Roaming){
            yield return new WaitForSeconds(steeringCd);
            float rotationAngle = Random.Range(-steeringAngleMax, steeringAngleMax);
            _movingVector = Quaternion.AngleAxis(rotationAngle, Vector3.forward)* _movingVector;
        }
    }

    private bool IsMoving(){
        if (_state is ActionState.Chasing or ActionState.Roaming or ActionState.MovingToBread) return true;
        return false;
    }
    
    //to use to get the angle from a vector2
    private static float Angle(Vector2 vector2)
    {
        return 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * Mathf.Sign(vector2.x));
    }
    
    private enum ActionState
    {
        Chasing,
        Roaming,
        Dashing,
        MovingToBread,
        Eating,
        Stealing,
        GettingRobbed
    }

    private void ChangeState(ActionState newState){
        Debug.Log("State: "+_state+" -> " +newState);
        if (_state == ActionState.Roaming){
            StopRoaming();
            StopCoroutine(_temporaryIdleCoroutine);
        }
        if(_state == ActionState.Eating) StopCoroutine(EatBread(breadBeingEaten));
        if(_state == ActionState.MovingToBread) //StopCoroutine(_movingCoroutineVar); provo a cambiare questo con stopRoaming
            StopMoving();
        if(_state == ActionState.Chasing) //StopCoroutine(_chasingPlayerCoroutine);
        if (_state == ActionState.Eating){ //doing this resets the trigger "on enter" method, so it can see again pieces of bread alredy seen in the past
            GameObject collider2D = GetComponent<Collider2D>().gameObject;
            collider2D.SetActive(false);
            collider2D.SetActive(true);
        }
        switch (newState)
        {
            case ActionState.Chasing:
                ChasePlayer(playerGameObjectToChase);
                _movingCoroutineVar = StartCoroutine(MovingCoroutine());
                _state = ActionState.Chasing;
                break;
            case ActionState.Roaming:
                _state = ActionState.Roaming;
                _temporaryIdleCoroutine = StartCoroutine(TemporaryIdleCoroutine());
                StartMovement();
                break;
            case ActionState.Dashing:
                _state = ActionState.Dashing;
                break;
            case ActionState.MovingToBread:
                _movingCoroutineVar = StartCoroutine(MovingCoroutine());
                _state = ActionState.MovingToBread;
                break;
            case ActionState.Eating:
                StopMoving(); //todo: questo potreebbe creare casini nel caso in cui si imbatta casualmente nel pane
                _state = ActionState.Eating;
                break;
            case ActionState.Stealing:
                _state = ActionState.Stealing;
                break;
            case ActionState.GettingRobbed:
                _state = ActionState.GettingRobbed;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private IEnumerator TemporaryIdleCoroutine(){
        while (_state==ActionState.Roaming){
            yield return new WaitForSeconds(_idleCD);
            //StopCoroutine(_movingCoroutineVar);
            StopRoaming();
            yield return new WaitForSeconds(_idleTime);
            StartMovement();
        }
        yield return null;
    }

    private void ChasePlayer(GameObject playerGameObject){
        _chasingPlayerCoroutine = StartCoroutine(ChasePlayerCoroutine(playerGameObject));
    }

    private IEnumerator ChasePlayerCoroutine(GameObject playerGameObject){
        PlayerDuck playerDuck = playerGameObject.GetComponent<PlayerDuck>();
        while (_state==ActionState.Chasing && Distance(playerGameObject)<outerRadiusCollider && playerDuck.FoodInMouth>0){
            _movingVector = Direction(playerGameObject);
            yield return null;
        }
    }

    private void StopRoaming(){
        StopMoving();
        StopCoroutine(_steeringCoroutineVar);
    }

    private void StopMoving(){
        _decelerateCoroutine = StartCoroutine(DecelerateCoroutine());
        //StopCoroutine(_movingCoroutineVar);
    }

    private IEnumerator DecelerateCoroutine(){
        while (_speedPerc>0){
            _speedPerc -= 0.01f;
            yield return new WaitForSeconds(_accelerationTimeSeconds/100);
        }
        StopCoroutine(_movingCoroutineVar);
        yield return null;
    }

    public void StartStealingPassive(){
        //todo: capire cosa bisogna fare
        ChangeState(ActionState.GettingRobbed);
    }

    public void StartStealingActive(GameObject playerDuckGameObject){
        playerGameObjectToChase = playerDuckGameObject;
        ChangeState(ActionState.Chasing);
    }

    private void OnTriggerEnter2D(Collider2D collider2D){
        if (collider2D.gameObject.GetComponent<PlayerDuck>() != null) CheckStealingOptions(collider2D.gameObject);
        TriggerActionBread(collider2D);
    }

    private void CheckStealingOptions(GameObject playerDuckGameObject){
        PlayerDuck playerDuck = playerDuckGameObject.GetComponent<PlayerDuck>();
        if (playerDuck.FoodInMouth > 0 && stealingCd == 0){
            float distance = Distance(playerDuckGameObject);
            float rand = Random.value;
            if (distance < innerRadiusCollider && rand< percInnerRadius/100.0f){
                StartStealingActive(playerDuckGameObject);
            }
            else if (distance < mediumRadiusCollider && rand < percMediumRadius/100.0f){
                StartStealingActive(playerDuckGameObject);
            }
        }
    }

    private void TriggerActionBread(Collider2D collider2D){
        if (_state == ActionState.Eating) return;
        GameObject collidedWith = collider2D.gameObject;
        float distance = Distance(collidedWith);
        float rand = Random.value;
        if (collidedWith.GetComponent<Bread>()){
            if (breadTargeted != null && distance > Distance(collidedWith))
                return; //alredy pursuing a closer piece of bread
            //check se è bread
            if (distance < innerRadiusCollider && (rand < (float) percInnerRadius / 100.0))
                TargetBread(collidedWith);
            else if (distance < mediumRadiusCollider && (rand < (float) percMediumRadius / 100.0))
                TargetBread(collidedWith);
            else if (distance < outerRadiusCollider && (rand < (float) percOuterRadius / 100.0))
                TargetBread(collidedWith);
        }
    }

    private void TargetBread(GameObject breadGameObject){
        breadTargeted = breadGameObject;
        MoveToBread(breadTargeted);
    }

    private void MoveToBread(GameObject breadGameObject){

        Vector2 direction = Direction(breadGameObject);
        
        Vector2 normalizedVector = NormalizeToMaxSpeed(direction);
        _movingVector = normalizedVector;
        //todo fare in modo che ruoti gradualemente piuttosto che di colpo
        
        if(_state!=ActionState.MovingToBread)  ChangeState(ActionState.MovingToBread);
        //magari mettere nel switch case un metodo per fare roteare gradualmente verso la direzione del bread
    }

    private Vector2 Direction(GameObject destinationGameObject){
        var destination = destinationGameObject.transform.position;
        var currentPosition = transform.position;
        return new Vector2(destination.x - currentPosition.x,destination.y - currentPosition.y);
    }

    public void StartEatingBread(GameObject breadGameObject){
        breadBeingEaten = breadGameObject.GetComponent<Bread>();
        ChangeState(ActionState.Eating);
        _eatBreadCoroutine =StartCoroutine(EatBread(breadBeingEaten));
        Destroy(breadGameObject);
    }

    IEnumerator EatBread(Bread bread){
        int i = 0;
        while (bread.BreadPoints>0){
            bread.BreadPoints--;
            i++;
            yield return new WaitForSeconds(_eatingSpeed);
        }
        ChangeState(ActionState.Roaming);
    }
    
    private Vector2 NormalizeToMaxSpeed(Vector2 vectorToNormalize){
        //rigidbody.velocity = rigidbody.velocity.normalized * currentSpeed; potrebbe andare bene
        float x1 = vectorToNormalize.x, y1 = vectorToNormalize.y;
        float normalizationFactor = _maxSpeed/math.sqrt(x1 * x1 + y1 * y1);
        Vector2 normalizedVector = new Vector2(x1 *normalizationFactor, y1 *normalizationFactor);
        return normalizedVector;
    }
    
    private Vector2 NormalizeToSpeed(Vector2 vectorToNormalize){
        //rigidbody.velocity = rigidbody.velocity.normalized * currentSpeed; potrebbe andare bene
        float x1 = vectorToNormalize.x, y1 = vectorToNormalize.y;
        float normalizationFactor = _maxSpeed/math.sqrt(x1 * x1 + y1 * y1);
        Vector2 normalizedVector = new Vector2(x1 *normalizationFactor, y1 *normalizationFactor);
        return normalizedVector;
    }

    private float Distance(GameObject destinationGameObject){
        Vector3 dest = destinationGameObject.transform.position;
        return math.distance(dest, transform.position);
    }
}
