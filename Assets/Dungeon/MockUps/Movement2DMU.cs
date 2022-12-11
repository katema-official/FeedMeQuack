using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2DMU : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private Rigidbody2D _rb;
    [SerializeField] private float _degrees = 1f;
    [SerializeField] private float vX;
    [SerializeField] private float vY;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _rb.velocity = new Vector2(vX, vY);
            //vX += 0.01f;
            //vY += 0.11f;

            float angleInDegrees = Vector2.SignedAngle(Vector2.right, _rb.velocity);    //This 4 lines work perfectly!
            if (angleInDegrees < 0)
            {
                angleInDegrees = 360f + angleInDegrees;
            }

            if (_rb.velocity != Vector2.zero) _rb.rotation = Vector2.SignedAngle(Vector2.right, _rb.velocity);
            Debug.Log("ROtation= " + angleInDegrees);
            
        }
    }
}
