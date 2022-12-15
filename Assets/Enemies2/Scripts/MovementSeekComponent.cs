using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviourNamespace
{
    public class MovementSeekComponent : MonoBehaviour
    {
        //this component takes care of moving the enemy duck towards a specific destination, moving along
        //a certain number of intermediate destinations
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

        [SerializeField] private float _initialSteer = 0f;




        private float _xComponentSpeed;
        private float _yComponentSpeed;
        //where the duck actually moves
        private void FixedUpdate()
        {
            //first of all, i retrieve the direction in which I need to move
            _directionToMoveNormalized = (CurrentDestination - (new Vector2(transform.position.x, transform.position.y)));
            _directionToMoveNormalized.Normalize();

            if (IsDestinationValid)
            {

                float angle = Mathf.Atan2(-_directionToMoveNormalized.x, _directionToMoveNormalized.y) * Mathf.Rad2Deg;
                _currentRotation = angle;
                //then, I apply a force in that direction
                float force = _currentAcceleration * MaxSteer;
                if (_rigidbody2D.velocity.magnitude <= MaxSpeed)
                {
                    _rigidbody2D.AddForce(_directionToMoveNormalized * force, ForceMode2D.Force);
                }
                _rigidbody2D.velocity = Vector2.ClampMagnitude(_rigidbody2D.velocity, MaxSpeed);
                if (Vector2.Distance(transform.position, FinalDestination) <= StopAt)
                {
                    IsDestinationValid = false;
                    HasStartedDecelerating = true;
                    _xComponentSpeed = _rigidbody2D.velocity.x;
                    _yComponentSpeed = _rigidbody2D.velocity.y;
                    //Debug.Log("X AND Y AT THE END: " + _xComponentSpeed + ", " + _yComponentSpeed);
                }
                

            }

            if (HasStartedDecelerating)
            {
                if(_rigidbody2D.velocity.x * _xComponentSpeed > 0 && _rigidbody2D.velocity.y * _yComponentSpeed > 0)
                {

                    Vector2 _directionToBrake = -_rigidbody2D.velocity.normalized;  //_directionToMoveNormalized;

                    float forceBreak = Deceleration; //* MaxSteer; // (_rigidbody2D.mass * Mathf.Pow(_rigidbody2D.velocity.magnitude,2)) / (2 * StopAt);
                    _rigidbody2D.AddForce(_directionToBrake * forceBreak, ForceMode2D.Force);
                    //Debug.LogFormat("SPEED X AND Y: {0} AND {1}", _rigidbody2D.velocity.x, _rigidbody2D.velocity.y);
                    if(_rigidbody2D.velocity.x * _xComponentSpeed < 0 && _rigidbody2D.velocity.y * _yComponentSpeed < 0)
                    {
                        _rigidbody2D.velocity = new Vector2(0, 0);
                        //Debug.Log("STOP");
                        HasStartedDecelerating = false;
                    }
                }
                   
            }

        }

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = transform.Find("Sprite").GetComponent<Animator>();
            _currentRotation = Random.Range(-180f, 180f);
            Debug.Log("rotation = " + _currentRotation);
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
            IsDestinationValid = true;
            HasStartedDecelerating = false;
        }

        //method to call when we don't want the duck to move anymore
        public void StopMoving()
        {
            IsDestinationValid = false;
            HasStartedDecelerating = false;
        }

        private void Update()
        {
            SetRotation();
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


    }
}