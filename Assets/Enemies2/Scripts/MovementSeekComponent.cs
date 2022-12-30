using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviourNamespace
{
    public class MovementSeekComponent : MonoBehaviour
    {
        //this component takes care of moving the enemy duck towards a specific destination, moving along
        //a certain number of intermediate destinations. Basically it shouldn't be used alone, but should be
        //exploited by other components (that possibly represent/take care of specific states of the enemy FSM)
        //to move the enemy duck around (see Roaming, FoodSeeking...)
        public Vector2 FinalDestination;
        public Vector2 CurrentDestination;
        public bool IsDestinationValid = false;
        public bool HasStartedDecelerating = false;
        private float _currentRotation = 0.0f;

        public float MaxSpeed = 0.0f;
        public float MaxSteer = 0.0f;
        public float Acceleration = 0.0f;
        public float Deceleration = 0.0f;
        private float _currentAcceleration = 1.0f;

        public float StopAt = 0.0f;

        private Vector2 _directionToMoveNormalized;
        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private Vector2 _directionToBrake;
        private bool _stopForReal;

        [SerializeField] private float _initialSteer = 0f;




        private float _xComponentSpeed;
        private float _yComponentSpeed;
        //where the duck actually moves
        private void FixedUpdate()
        {
            //if (IsDestinationValid || HasStartedDecelerating...

            if (IsDestinationValid)
            {

                //then, I apply a force in that direction
                //float force = _currentAcceleration * MaxSteer;
                if (_rigidbody2D.velocity.magnitude <= MaxSpeed)
                {
                    _rigidbody2D.AddForce(_directionToMoveNormalized * _currentAcceleration * MaxSteer, ForceMode2D.Force);
                }
                _rigidbody2D.velocity = Vector2.ClampMagnitude(_rigidbody2D.velocity, MaxSpeed);

                //if (Vector2.Distance...


            }


            if(HasStartedDecelerating)
            {
                if (!_stopForReal)
                {
                    _rigidbody2D.AddForce(_directionToBrake * Deceleration, ForceMode2D.Force);
                }
                else
                {
                    HasStartedDecelerating = false;
                    _rigidbody2D.velocity = new Vector2(0, 0);
                }
            }

            /*if (HasStartedDecelerating)
            {

                

                if(_rigidbody2D.velocity.x * _xComponentSpeed > 0 && _rigidbody2D.velocity.y * _yComponentSpeed > 0)
                {

                    //_directionToBrake = -_rigidbody2D.velocity.normalized;  //_directionToMoveNormalized;

                    //float forceBreak = Deceleration;
                    _rigidbody2D.AddForce(_directionToBrake * Deceleration, ForceMode2D.Force);
                    //Debug.LogFormat("SPEED X AND Y: {0} AND {1}", _rigidbody2D.velocity.x, _rigidbody2D.velocity.y);
                    if(_rigidbody2D.velocity.x * _xComponentSpeed < 0 && _rigidbody2D.velocity.y * _yComponentSpeed < 0) //oppure se ti stai muovendo di pochissimo (non viene mai chiamato hahahaha lol kek)
                    {
                        //Debug.Log("STOP");  //non viene mai chiamato
                        HasStartedDecelerating = false;
                        _rigidbody2D.velocity = new Vector2(0, 0);
                        
                    }
                }
                else
                {
                    //Debug.Log("BASTA");
                    HasStartedDecelerating = false;
                    _rigidbody2D.velocity = new Vector2(0, 0);
                }
                   
            }*/

        }


        private void LateUpdate()
        {

            if (IsDestinationValid || HasStartedDecelerating)
            {
                //first of all, i retrieve the direction in which I need to move
                _directionToMoveNormalized = (CurrentDestination - (new Vector2(transform.position.x, transform.position.y)));
                _directionToMoveNormalized.Normalize();
            }

            if (IsDestinationValid)
            {
                float angle = Mathf.Atan2(-_directionToMoveNormalized.x, _directionToMoveNormalized.y) * Mathf.Rad2Deg;
                _currentRotation = angle;
                SetRotation();
                if (Vector2.Distance(transform.position, FinalDestination) <= StopAt)
                {
                    StopMoving();
                }
            }

            if (HasStartedDecelerating)
            {
                _stopForReal = _rigidbody2D.velocity.x * _xComponentSpeed < 0 && _rigidbody2D.velocity.y * _yComponentSpeed < 0;
            }

        }




        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = transform.Find("Sprite").GetComponent<Animator>();
            _currentRotation = Random.Range(-180f, 180f);
            //Debug.Log("rotation = " + _currentRotation);
            SetRotation();    
        }




        //set the acceleration to max immediately
        public void SetAcceleration_Max()
        {
            _currentAcceleration = Acceleration;
        }

        //set the acceleration to a low value that increases over time
        public void SetAcceleration_Increment()
        {
            _currentAcceleration = 1f;
            StartCoroutine(BringCurrentAccelerationToMax());
        }

        private IEnumerator BringCurrentAccelerationToMax()
        {
            float waitTime = 0.2f;
            float delta = 1f;
            while(_currentAcceleration < Acceleration)
            {
                _currentAcceleration += delta;
                yield return new WaitForSeconds(waitTime);
            }
            yield return null;
        }




        //method to call when all the data necessary to go to a destination have been fixed
        public void StartMoving()
        {
            _stopForReal = false;
            IsDestinationValid = true;
            HasStartedDecelerating = false;
        }

        //method to call when we don't want the duck to move anymore
        public void StopMoving()
        {
            _directionToBrake = -_rigidbody2D.velocity.normalized;
            IsDestinationValid = false;
            HasStartedDecelerating = true;
            SetCurrentVelocityComponents();
            //HasStartedDecelerating = false;   //I probably don't need this. If it was far from the destination, this is already false. If I want it to stop moving because
            //the destination was reached, I still want this duck to decelerate
        }

        //It's a bit drastic, but can be useful every time we don't want this component to take care anymore of the movement of the duck (as for example in the Dashing)
        public void CompletelyStopMoving()
        {
            IsDestinationValid = false;
            HasStartedDecelerating = false;
            _rigidbody2D.velocity = Vector2.zero;
        }

        public void SetCurrentVelocityComponents()
        {
            _xComponentSpeed = _rigidbody2D.velocity.x;
            _yComponentSpeed = _rigidbody2D.velocity.y;
        }

        public void SetCurrentAndFinalDestination(Vector3 current, Vector3 final)
        {
            CurrentDestination = current;
            FinalDestination = final;
        }

        public void SetCurrentDestination(Vector3 current)
        {
            CurrentDestination = current;
        }



        

        public void SetRotation()
        {
            float angle = _currentRotation;

            //up
            if (angle >= 0 && angle < 22.5)
            {
                _animator.SetFloat("Blend", 0.1428571f);
            }
            //up-left
            else if (angle >= 22.5f && angle < 77.5f)
            {
                _animator.SetFloat("Blend", 0.2857143f);
            }
            //left
            else if (angle >= 77.5f && angle < 112.5)
            {
                _animator.SetFloat("Blend", 0.4285714f);
            }
            //down-left
            else if (angle >= 112.5 && angle < 157.5)
            {
                _animator.SetFloat("Blend", 0.5714286f);
            }
            //down
            else if (angle >= 157.5 && angle < 180)
            {
                _animator.SetFloat("Blend", 0.7142857f);
            }


            //down
            else if (angle >= -180 && angle < -157.5)
            {
                _animator.SetFloat("Blend", 0.7142857f);
            }
            //down-right
            else if (angle >= -157.5 && angle < -112.5)
            {
                _animator.SetFloat("Blend", 0.8571429f);
            }
            //right
            else if (angle >= -112.5 && angle < -77.5f)
            {
                _animator.SetFloat("Blend", 1f);
            }
            //up-right
            else if (angle >= -77.5f && angle < -22.5f)
            {
                _animator.SetFloat("Blend", 0);
            }
            //up
            else if (angle >= -22.5 && angle < 0)
            {
                _animator.SetFloat("Blend", 0.1428571f);
            }
        }

        public float GetRotation()
        {
            return _currentRotation;
        }


    }
}