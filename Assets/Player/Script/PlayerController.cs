using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigidBody = null;
    private Camera _camera = null;
    private Transform _mouth = null;

    private bool _moveForward = false;
    private float _rotationMovement = 0.0f;
    private float _rotationSpeed = 90.0f;

    private float _speed = 0.0f;
    private PlayerState _state = PlayerState.Normal;


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
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
        _rotationMovement = 0;

        if (Input.GetKey(KeyCode.W))
        {
            _moveForward = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _rotationMovement = _rotationSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _rotationMovement = -_rotationSpeed;
        }
    }


    private void FixedUpdate()
    {
        
    }
}
