using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class enemyFSM : MonoBehaviour
{
    /**
     * Quando si sta muovendo, ogni tot tempo, può cambiare la direzioni di un angolo minore di 90°. Quando atterra un pezzo di pane, rng per vedere se gli va
     * incontro
     */
    
    private float _speed=6.0f, _mouthSize, _eatingSpeed, _steeringSpeed, _changeDirectionCd;

    private int numColl = 0;
    
    [SerializeField] private float steeringSpeedMaxValue=90;
    [SerializeField] private float steeringCd=5f;
    [SerializeField] private float innerRadiusCollider, mediumRadiusCollider, outerRadiusCollider;
    [SerializeField] private int percInnerRadius, percMediumRadius, percOuterRadius;
 
    public GameObject breadBeingEaten;
    [SerializeField] private GameObject breadTargeted;
    
    private ActionState _state;
    private Vector3 _movingVector;
    
    // Start is called before the first frame update
    void Start(){
        _movingVector = new Vector2(6, 0);
        ChangeState(ActionState.Roaming);
    }

    // Update is called once per frame
    void Update(){
        transform.position += _movingVector * Time.deltaTime;
        
    }

    private void StartMovement(){
        StartCoroutine(MovingCoroutine());
        StartCoroutine(ChangingDirectionCoroutine());
    }

    private IEnumerator MovingCoroutine(){
        while (IsMoving())
        {
            transform.position += _movingVector * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ChangingDirectionCoroutine(){
        while (IsMoving()){
            yield return new WaitForSeconds(steeringCd);
            float rotationAngle = Random.Range(-steeringSpeedMaxValue / 2f, steeringSpeedMaxValue / 2f);
            _movingVector = Quaternion.AngleAxis(rotationAngle, Vector3.forward)* _movingVector;
        }
    }

    private void MoveToPoint(Vector2 pointToMoveTo){
        
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
        if (_state == ActionState.Roaming) StopRoaming();
        if(_state == ActionState.Eating) StopCoroutine(EatBread(breadBeingEaten));
        if(_state == ActionState.MovingToBread) StopCoroutine(MovingCoroutine());
        switch (newState)
        {
            case ActionState.Chasing:
                _state = ActionState.Chasing;
                break;
            case ActionState.Roaming:
                _state = ActionState.Roaming;
                StartMovement();
                break;
            case ActionState.Dashing:
                _state = ActionState.Dashing;
                break;
            case ActionState.MovingToBread:
                break;
            case ActionState.Eating:
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

    private void StopRoaming(){
        StartCoroutine(MovingCoroutine());
        StopCoroutine(ChangingDirectionCoroutine());
    }

    public void StartStealingPassive(){
        //todo: capire cosa bisogna fare
        ChangeState(ActionState.GettingRobbed);
    }

    public void StartStealingActive(){
        //todo: capire che fare
        ChangeState(ActionState.Stealing);
    }

    private void OnTriggerEnter2D(Collider2D collider2D){ //forse devo fare collide piuttosto che trigger
        numColl++;
        GameObject collidedWith = collider2D.gameObject;
        float distance = math.distance(collidedWith.transform.position, transform.position);
        float rand = Random.value;
        if (collidedWith.GetComponent<Bread>()){
            if(breadTargeted!=null && distance>math.distance(breadTargeted.transform.position, transform.position))
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

    private void OnCollisionEnter2D(Collision2D col){//todo: assummo che questo funzioni solo al contatto
        StartEatingBread(col.gameObject);
    }

    private void TargetBread(GameObject breadGameObject){
        numColl++;
        Debug.Log("Cambiato target "+ numColl);
        breadTargeted = breadGameObject;
        MoveToBread(breadTargeted);
    }

    private void MoveToBread(GameObject breadGameObject){
        var position = breadGameObject.transform.position;
        var currentPosition = transform.position;
        Vector2 direction = new Vector2(position.x - currentPosition.x,position.y - currentPosition.y);
        
        /*
        float angle = Angle(direction);
        Vector3 v1 = Quaternion.Euler(0,angle,0)*Vector3.forward;
        _movingVector=v1*_speed;*/
        Vector2 normalizedVector = NormalizeToMaxSpeed(direction);
        _movingVector = normalizedVector;
        StartCoroutine(MovingCoroutine()); //todo fare in modo che ruoti gradualemente piuttosto che di colpo
        
        ChangeState(ActionState.MovingToBread);//magari mettere nel switch case un metodo per fare roteare gradualmente verso la direzione del bread
    }

    public void StartEatingBread(GameObject breadGameObject){
        Debug.Log("SI MANGIA!");
        breadBeingEaten = breadGameObject;
        ChangeState(ActionState.Eating);
        StartCoroutine(EatBread(breadGameObject));
    }

    IEnumerator EatBread(GameObject breadGameObject){
        Bread bread = breadGameObject.GetComponent<Bread>();
        while (bread.BreadPoints>0){
            bread.BreadPoints--;
            yield return new WaitForSeconds(_eatingSpeed);
        }
        Destroy(breadGameObject);
        ChangeState(ActionState.Roaming);
    }

    string PrintVector(Vector2 vector){
        return "x: "+ vector.x+"    y: "+vector.y;
    }

    private Vector2 NormalizeToMaxSpeed(Vector2 vectorToNormalize){
        float x1 = vectorToNormalize.x, y1 = vectorToNormalize.y;
        float normalizationFactor = _speed/math.sqrt(x1 * x1 + y1 * y1);
        Vector2 normalizedVector = new Vector2(x1 *normalizationFactor, y1 *normalizationFactor);
        return normalizedVector;
    }
}
