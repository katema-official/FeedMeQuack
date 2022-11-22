using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public static class PlayerUtility
    {
        public static void GetMovementAxis(ref bool moveForward,ref Vector3 forwardAxis, ref Vector3 rightwardAxis)
        {
            moveForward = false;
            forwardAxis = new Vector3(0, 0);
            rightwardAxis = new Vector3(0, 0);

            if (Input.GetKey(KeyCode.W))
            {
                forwardAxis = new Vector3(0, 1);
                moveForward = true;
            }
            if (Input.GetKey(KeyCode.A))
            {
                rightwardAxis = new Vector3(-1, 0);
                moveForward = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                forwardAxis = new Vector3(0, -1);
                moveForward = true;
            }
            if (Input.GetKey(KeyCode.D))
            {
                rightwardAxis = new Vector3(1, 0);
                moveForward = true;
            }
        }
        public static void Move(float speed, Vector3 forwardAxis, Vector3 rightwardAxis, Rigidbody2D rigidBody, bool moveForward, ref float rotationMovement)
        {
            if (moveForward)
            {
                var finalDir = forwardAxis + rightwardAxis;
                finalDir.Normalize();

                float angle = Mathf.Atan2(-finalDir.x, finalDir.y) * Mathf.Rad2Deg;
                rotationMovement = angle;

                var force = speed * 1.5f;
                rigidBody.AddForce(finalDir * force, ForceMode2D.Force);
                rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, speed);
            }

            rigidBody.SetRotation(Quaternion.AngleAxis(rotationMovement, Vector3.forward));

            Debug.Log("Current player velocity: " + rigidBody.velocity);
        }
    }
}
