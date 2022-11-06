using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigidBody = null;
    private Camera _camera = null;
    private Transform _mouth = null;

    private bool _moveForward = false;
    private float _speed = 0.0f;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _camera = transform.parent.GetComponentInChildren<Camera>();
        _mouth = transform.Find("Mouth");
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
