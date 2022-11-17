using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D _rigidBody = null;
        private Camera _camera = null;
        private Transform _mouth = null;

        private LakeController _currentLake = null;

        private Vector3 _forwardAxis;
        private Vector3 _rightwardAxis;

        private bool _moveForward = false;
        private float _rotationMovement = 0.0f;
      //  private float _rotationSpeed = 90.0f;

        private float _force = 0.0f;
        private PlayerState _state = PlayerState.Normal;



        private PlayerDuckDescriptionSO _description = null;
        //Derived from DuckDescription
        //---------------------------------
        [SerializeField] private float _speed = 0.0f;
        //[SerializeField] private float _eatingSpeed = 0.0f;
        //[SerializeField] private float _chewingRate = 0.0f;
        //[SerializeField] private int _mouthSize = 0;
        //---------------------------------

        public void Move(float speed, float rotationMovement, bool moveForward)
        {
            if (moveForward)
            {   
                var finalDir = _forwardAxis + _rightwardAxis;
                finalDir.Normalize();
           
                float angle = Mathf.Atan2(-finalDir.x, finalDir.y) * Mathf.Rad2Deg;
                _rotationMovement = angle;
                _force = _speed * 1.5f;
                _rigidBody.AddForce(finalDir * _force, ForceMode2D.Force);
                _rigidBody.velocity = Vector2.ClampMagnitude(_rigidBody.velocity, _speed);
            }

            _rigidBody.SetRotation(Quaternion.AngleAxis(_rotationMovement, Vector3.forward));

             Debug.Log("Current player velocity: " + _rigidBody.velocity);
        }

        private void MoveCamera()
        {
            _camera.transform.position = _rigidBody.position;
            var cameraBounds = CameraUtility.OrthographicBounds(_camera);
            Bounds lakeBounds;

            if (_currentLake)
                lakeBounds = _currentLake.getBounds();
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
            if (_rigidBody)
            {
                _rigidBody.gravityScale = 0f;
                _rigidBody.freezeRotation = true;
            }

            var duckTypeManager = GameObject.FindObjectOfType<DuckTypeManager>();
            _description = duckTypeManager.getTypeFromName("Mallard");

            _speed = _description.Speed;
            //_eatingSpeed = _description.EatingSpeed;
            //_chewingRate = _description.ChewingRate;
            //_mouthSize = _description.MouthSize;        
        




            _force = _speed * 1.5f;
            _camera = transform.parent.GetComponentInChildren<Camera>();
            _mouth = transform.Find("Mouth");
        }

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
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
            if (_state == PlayerState.Normal)  Move(_speed, _rotationMovement, _moveForward);
           // MoveCamera();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var lakeController = collision.gameObject.GetComponentInChildren<LakeController>();
            if (lakeController)  _currentLake = lakeController;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var lakeController = collision.gameObject.GetComponentInChildren<LakeController>();
            if (lakeController)  _currentLake = null;
        }
    }
}
