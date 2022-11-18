using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerMoveSkill : PlayerSkill
    {
        //Move Skill Data
        //------------------------------------------
        [SerializeField] private float _speed = 0.0f;
        //------------------------------------------


        private PlayerController _controller = null;
        private PlayerMoveSkillDescriptionSO _moveDesc = null;


        private Rigidbody2D _rigidBody = null;
        private Vector3 _forwardAxis;
        private Vector3 _rightwardAxis;

        private bool _moveForward = false;
        private float _rotationMovement = 0.0f;

        private float _force = 0.0f;
        private float _overrideSpeed = 0.0f;


        private bool _enableInput = true;



        public override void SetDescription(PlayerSkillDescriptionSO desc)
        {
            _description = desc;
            _moveDesc = (PlayerMoveSkillDescriptionSO)_description;

            _speed = _moveDesc.Speed;
        }

        public void EnableInput(bool enable)
        {
            _enableInput = enable;
        }


        //public void SetOverrideSpeed(float overrideSpeed)
        //{
        //    _overrideSpeed = overrideSpeed;
        //}


        public void Move(float speed/*, bool moveForward*/)
        {
            if (_moveForward)
            {
                var finalDir = _forwardAxis + _rightwardAxis;
                finalDir.Normalize();

                float angle = Mathf.Atan2(-finalDir.x, finalDir.y) * Mathf.Rad2Deg;
                _rotationMovement = angle;
                _force = speed * 1.5f;
                _rigidBody.AddForce(finalDir * _force, ForceMode2D.Force);
                _rigidBody.velocity = Vector2.ClampMagnitude(_rigidBody.velocity, _speed);
            }

            _rigidBody.SetRotation(Quaternion.AngleAxis(_rotationMovement, Vector3.forward));

           // Debug.Log("Current player velocity: " + _rigidBody.velocity);
        }


        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _controller = GetComponent<PlayerController>();

            var duckTypeManager = GameObject.FindObjectOfType<DuckTypeManager>();
        }
            // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (!_enableInput) return;

            //if (_controller.GetState() != PlayerState.Normal &&
            //    _controller.GetState() != PlayerState.Eating) return;


            // PlayerUtility.GetMovementAxis(ref _moveForward, ref _forwardAxis, ref _rightwardAxis);



            _moveForward = false;
            _forwardAxis = new Vector3(0, 0);
            _rightwardAxis = new Vector3(0, 0);

            if (Input.GetKey(KeyCode.W))
            {
                _forwardAxis = new Vector3(0, 1);
                _moveForward = true;
            }
            if (Input.GetKey(KeyCode.A))
            {
                _rightwardAxis = new Vector3(-1, 0);
                _moveForward = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                _forwardAxis = new Vector3(0, -1);
                _moveForward = true;
            }
            if (Input.GetKey(KeyCode.D))
            {
                _rightwardAxis = new Vector3(1, 0);
                _moveForward = true;
            }
        }


        private void FixedUpdate()
        {
            if (_controller.GetState() != PlayerState.Normal) return;

            //PlayerUtility.Move(_speed, _forwardAxis, _rightwardAxis, _rigidBody, _moveForward, ref _rotationMovement);
            Move(_speed/*, _moveForward*/);
        }
    }
}

