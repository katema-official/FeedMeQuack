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

    public int BreadDigested = 30;

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
            GameObject[] breads = GameObject.FindGameObjectsWithTag("FoodInWater");
            float minDist = 10000000f;
            GameObject BreadInWaterClosest = null;
            for(int i = 0; i < breads.Length; i++)
            {
                float dist = Vector3.Distance(breads[i].transform.position, transform.position);
                if (dist <= 3f)
                {
                    if(dist <= minDist)
                    {
                        minDist = dist;
                        BreadInWaterClosest = breads[i];
                    }
                }
            }

            if(BreadInWaterClosest != null && BreadInMouth == null)
            {
                BreadInMouth = BreadInWaterClosest.GetComponent<BreadNamespace.BreadInWaterComponent>().GenerateNewBreadInMouth(2);
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if(BreadInMouth != null)
            {
                bool destroyed;
                int eaten;
                (eaten, destroyed) = BreadInMouth.GetComponent<BreadNamespace.BreadInMouthComponent>().SubtractBreadPoints(1);

                BreadDigested += eaten;

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
