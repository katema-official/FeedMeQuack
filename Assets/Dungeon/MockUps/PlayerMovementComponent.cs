using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementComponent : MonoBehaviour
{
    Rigidbody2D body;

    float horizontal;
    float vertical;

    public float runSpeed = 20.0f;

    public GameObject BreadInMouth = null;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject[] breads = GameObject.FindGameObjectsWithTag("Bread");
            float minDist = 10000000f;
            BreadInMouth = null;
            for(int i = 0; i < breads.Length; i++)
            {
                float dist = Vector3.Distance(breads[i].transform.position, transform.position);
                if (dist <= 3f)
                {
                    if(dist <= minDist)
                    {
                        minDist = dist;
                        BreadInMouth = breads[i];
                    }
                }
            }

            if(BreadInMouth != null)
            {
                BreadInMouth.GetComponent<BreadNamespace.BreadComponent>().StartedToBeEaten(this.gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if(BreadInMouth != null)
            {
                bool destroyed;
                int eaten;
                (destroyed, eaten) = BreadInMouth.GetComponent<BreadNamespace.BreadComponent>().SubtractBreadPoints(1);

                if (destroyed)
                {
                    BreadInMouth = null;
                }
            }
        }


    }

    private void FixedUpdate()
    {
        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }
}
