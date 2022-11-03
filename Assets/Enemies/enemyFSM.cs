using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class enemyFSM : MonoBehaviour
{
    /**
     * Quando si sta muovendo, ogni tot tempo, può cambiare la direzioni di un angolo minore di 90°. Quando atterra un pezzo di pane, rng per vedere se gli va
     * incontro
     */
    
    private float _speed, _mouthSize, _eatingSpeed, _steeringSpeed, _changeDirectionCd;

    [SerializeField] private float steeringSpeedMaxValue=90;
    [SerializeField] private float steeringCd=5f;

    private GameObject _breadBeingEaten;
    
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
        if(_state == ActionState.Eating) StopCoroutine(EatBread(_breadBeingEaten));
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

    private void MoveToBread(GameObject breadGameObject){
        var position = breadGameObject.transform.position;
        Vector2 breadPosition=new Vector2(position.x,position.y);
        var currentPosition = transform.position;
        Vector2 direction = new Vector2(breadPosition.x - currentPosition.x,breadPosition.y - currentPosition.y);
        //_movingVector = direction;

        float angle = Angle(direction);
        
        Vector3 v1 = Quaternion.Euler(0,angle,0)*Vector3.forward;
        // correct speed (same as last line in Downstream's example):
        _movingVector=v1*_speed;
        StartCoroutine(MovingCoroutine()); //todo fare in modo che ruoti gradualemente piuttosto che di colpo
        
        ChangeState(ActionState.MovingToBread);//magari mettere nel switch case un metodo per fare roteare gradualmente verso la direzione del bread
    }

    public void NoticeBread(GameObject breadGameObject){
        MoveToBread(breadGameObject);
    }

    public void StartEatingBread(GameObject breadGameObject){
        _breadBeingEaten = breadGameObject;
        ChangeState(ActionState.Eating);
        EatBread(breadGameObject);
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

    public void StartStealingPassive(){
        //todo: capire cosa bisogna fare
        ChangeState(ActionState.GettingRobbed);
    }

    public void StartStealingActive(){
        //todo: capire che fare
        ChangeState(ActionState.Stealing);
    }
}
