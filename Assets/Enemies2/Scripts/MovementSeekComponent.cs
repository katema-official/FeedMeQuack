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

        public float BrakeAt = 0.0f;
        public float StopAt = 0.0f;

        private Vector2 _directionToMoveNormalized;
        private Rigidbody2D _rigidbody2D;

        [SerializeField] private float _initialSteer = 3f;




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

                //if we are near the destination, we can stop accelerating
                /*if (CurrentDestination == FinalDestination)
                {
                    if (Vector2.Distance(transform.position, FinalDestination) <= StopAt)
                    {
                        return;
                    }
                }*/

                float angle = Mathf.Atan2(-_directionToMoveNormalized.x, _directionToMoveNormalized.y) * Mathf.Rad2Deg;
                _currentRotation = angle;
                //then, I apply a force in that direction
                float force = Acceleration * MaxSteer;
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
                    Debug.Log("X AND Y AT THE END: " + _xComponentSpeed + ", " + _yComponentSpeed);
                }


                    

                /*
                    if(Vector2.Distance(transform.position, FinalDestination) <= StopAt &&
                        _rigidbody2D.velocity.x * _xComponentSpeed > 0 && _rigidbody2D.velocity.y * _yComponentSpeed > 0)
                    {
                        
                        Vector2 _directionToBrake = -_directionToMoveNormalized;

                        float forceBreak = Deceleration * MaxSteer; // (_rigidbody2D.mass * Mathf.Pow(_rigidbody2D.velocity.magnitude,2)) / (2 * StopAt);
                        _rigidbody2D.AddForce(_directionToBrake * forceBreak, ForceMode2D.Force);



                        

                        
                    }
                    else
                    {
                        float angle = Mathf.Atan2(-_directionToMoveNormalized.x, _directionToMoveNormalized.y) * Mathf.Rad2Deg;
                        _currentRotation = angle;
                        //then, I apply a force in that direction
                        float force = Acceleration * MaxSteer;
                        if (_rigidbody2D.velocity.magnitude <= MaxSpeed)
                        {
                            _rigidbody2D.AddForce(_directionToMoveNormalized * force, ForceMode2D.Force);
                        }
                        _rigidbody2D.velocity = Vector2.ClampMagnitude(_rigidbody2D.velocity, MaxSpeed);
                    }*/
                

            }

            if (HasStartedDecelerating)
            {
                if(_rigidbody2D.velocity.x * _xComponentSpeed > 0 && _rigidbody2D.velocity.y * _yComponentSpeed > 0)
                {

                    Vector2 _directionToBrake = -_rigidbody2D.velocity.normalized;  //_directionToMoveNormalized;

                    float forceBreak = Deceleration; //* MaxSteer; // (_rigidbody2D.mass * Mathf.Pow(_rigidbody2D.velocity.magnitude,2)) / (2 * StopAt);
                    _rigidbody2D.AddForce(_directionToBrake * forceBreak, ForceMode2D.Force);
                    Debug.LogFormat("SPEED X AND Y: {0} AND {1}", _rigidbody2D.velocity.x, _rigidbody2D.velocity.y);
                    if(_rigidbody2D.velocity.x * _xComponentSpeed < 0 && _rigidbody2D.velocity.y * _yComponentSpeed < 0)
                    {
                        _rigidbody2D.velocity = new Vector2(0, 0);
                        Debug.Log("STOP");
                        HasStartedDecelerating = false;
                    }
                }
                
                
            }

            /*if (CurrentDestination == FinalDestination)
            {
                StartedDecelerating(_directionToMoveNormalized);
                float force = Acceleration * MaxSteer;
                //if I'm close enough to the destination, I decelerate, by applying a force in the opposite direction of moving.
                //I keep applying this force until I've reached a certain (very low) speed, such that I will stop in a short amount of time
                if (Vector2.Distance(transform.position, FinalDestination) <= BrakeAt)
                {
                    
                    
                    Vector2 _directionToBrake = -_directionToMoveNormalized;

                    if (Vector2.Dot(_rigidbody2D.velocity, _directionToMoveNormalized) > 0)// && Vector2.Dot(_accelerationVector, _directionToBrake) < 0)
                    {
                        float forceBreak = Deceleration * MaxSteer;
                        _rigidbody2D.AddForce(_directionToBrake * force, ForceMode2D.Force);
                    }
                }
            }*/

        }

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            
        }

        public void SetInitialSteer()
        {
            MaxSteer = _initialSteer;
        }

        //method to call when all the data necessary to go to a destination have been fixed
        public void StartMoving()
        {
            IsDestinationValid = true;
            HasStartedDecelerating = false;
        }


        private Vector2 _accelerationVector;

        private void StartedDecelerating(Vector2 dir)
        {
            if (!HasStartedDecelerating)
            {
                HasStartedDecelerating = true;
                _accelerationVector = dir;
            }
        }

    }
}