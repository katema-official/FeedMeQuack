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
        private Camera _camera = null;

        private Rigidbody2D _rigidBody = null;
        private Vector3 _forwardAxis;
        private Vector3 _rightwardAxis;
        private Vector3 _finalDir;


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

        public Vector3 GetDirection()
        {
            return _finalDir;
        }

        //public void SetOverrideSpeed(float overrideSpeed)
        //{
        //    _overrideSpeed = overrideSpeed;
        //}


        public void Move(float speed, bool moveForward = false)
        {
            if (_moveForward || moveForward)
            {
                if (_enableInput)
                { 
                    _finalDir = _forwardAxis + _rightwardAxis;
                    _finalDir.Normalize();
                }

                float angle = Mathf.Atan2(-_finalDir.x, _finalDir.y) * Mathf.Rad2Deg;
                _rotationMovement = angle;
                _force = speed * 1.5f;
                _rigidBody.AddForce(_finalDir * _force, ForceMode2D.Force);
                _rigidBody.velocity = Vector2.ClampMagnitude(_rigidBody.velocity, speed);
            }

            _rigidBody.SetRotation(Quaternion.AngleAxis(_rotationMovement, Vector3.forward));

            MoveCamera();
           // Debug.Log("Current player velocity: " + _rigidBody.velocity);
        }

        private void MoveCamera()
        {
            _camera.transform.position = _rigidBody.position;
            var cameraBounds = CameraUtility.OrthographicBounds(_camera);
            Bounds lakeBounds;

            if (_controller.GetLake())
                lakeBounds = _controller.GetLake().getBounds();
            else
                lakeBounds = new Bounds();

            Vector3 newCamPos = transform.position;

            if (cameraBounds.min.x < lakeBounds.min.x) newCamPos.x += lakeBounds.min.x - cameraBounds.min.x;
            if (cameraBounds.max.x > lakeBounds.max.x) newCamPos.x -= cameraBounds.max.x - lakeBounds.max.x;
            if (cameraBounds.min.y < lakeBounds.min.y) newCamPos.y += lakeBounds.min.y - cameraBounds.min.y;
            if (cameraBounds.max.y > lakeBounds.max.y) newCamPos.y -= cameraBounds.max.y - lakeBounds.max.y;

            _camera.transform.position = newCamPos;
        }
        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _controller = GetComponent<PlayerController>();
            _camera = transform.parent.GetComponentInChildren<Camera>();

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

