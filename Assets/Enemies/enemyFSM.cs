using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class enemyFSM : MonoBehaviour
{
    /**
     * Quando si sta muovendo, ogni tot tempo, può cambiare la direzioni di un angolo minore di 90°. Quando atterra un pezzo di pane, rng per vedere se gli va
     * incontro
     */
    
    private float _speed, _mouthSize, _eatingSpeed, _steeringSpeed, _changeDirectionCd;

    [SerializeField] private float changeDirectionMaxValue=90;
    [SerializeField] private float changeDirectionCd=5f;
    
    private ActionState _state;
    private Vector3 _movingDirection;
    
    // Start is called before the first frame update
    void Start(){
        //_movingDirection = new Vector2(Random.value, Random.value);
        _movingDirection = new Vector2(6, 0);
        StartMovement();
    }

    // Update is called once per frame
    void Update(){
        transform.position += _movingDirection * Time.deltaTime;
        
    }

    private void StartMovement(){
        StartCoroutine(HanldeMovement());
        StartCoroutine(ChangeDirection());
    }

    private IEnumerator HanldeMovement(){
        while (IsMoving())
        {
            transform.position += _movingDirection * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ChangeDirection(){
        while (IsMoving()){
            yield return new WaitForSeconds(changeDirectionCd);
            float rotationAngle = Random.Range(-changeDirectionMaxValue / 2f, changeDirectionMaxValue / 2f);
            _movingDirection = Quaternion.AngleAxis(rotationAngle, Vector3.forward)* _movingDirection;
        }
    }

    private bool IsMoving(){
        if (_state is ActionState.Chasing or ActionState.Roaming or ActionState.BreadSeeking) return true;
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
        BreadSeeking,
        Eating,
        Stealing,
        GettingRobbed
    }
}
