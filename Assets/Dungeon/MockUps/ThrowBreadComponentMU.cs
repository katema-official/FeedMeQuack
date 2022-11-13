using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBreadComponentMU : MonoBehaviour
{
    //code based on:
    //https://forum.unity.com/threads/simulate-gravity-in-a-top-down-2d-rpg.293712/


    // The fake gravity pulling this object down along the y-axis.
    [SerializeField] public float gravity = 100f;

    // The velocity we're moving at. Only for the y-axis.
    [SerializeField] private float velocity;

    // Our initial velocity up along the y-axis.
    float initialVelocity;
    [SerializeField] public float MinInitialVelocity = 75f;
    [SerializeField] public float MaxInitialVelocity = 125f;

    // Our sprite which is a child of this gameobject. This is the the thing
    // the player sees so this is the object we manipulate with our fake gravity.
    Transform sprite;

    // A flag we set to true when our sprite has landed on our fake ground plane.
    [SerializeField] private bool sleep = true;

    // A place for us to store the angularVelocity for the rigidbody2D. We don't want
    // to rotate the entire rigidbody because that would offset the shadow sprite which
    // follows the ground plane so we rotate the sprite by this velocity instead.
    float angularVelocity;

    // The amount we place the sprite above the game object to simulate it flying over
    // the ground. The shadow follows the gameobject in a straight line so when we launch
    // the sprite in an arc it looks like it's flying even though everything is still
    // entirely flat.
    [SerializeField] private float _height;

    private bool thrown = false;



    private float _xInit;
    private float _yInit;
    private float _xEnd;
    private float _yEnd;
    private float _xDistance;
    private float _yDistance;

    void Start()
    {

        //choose the initial position
        GameObject grass = GameObject.Find("Terrain");
        //float width_grass = grass.GetComponent<SpriteRenderer>().sprite.rect.width;
        //float height_grass = grass.GetComponent<SpriteRenderer>().sprite.rect.height;
        float widthGrass = grass.transform.localScale.x;
        float heightGrass = grass.transform.localScale.y;
        float xCenterGrass = grass.transform.position.x;
        float yCenterGrass = grass.transform.position.y;
        float offsetXTerrain = 10;
        float offsetYTerrain = 8;
        int left_right___or___above_below = Random.Range(0, 2);
        _xInit = Random.Range(0 - offsetXTerrain, widthGrass + offsetXTerrain + 1);
        _yInit = Random.Range(0 - offsetYTerrain, heightGrass + offsetYTerrain + 1);

        if(left_right___or___above_below == 0)
        {
            _yInit = Random.Range(yCenterGrass - (heightGrass / 2) - offsetYTerrain, yCenterGrass + heightGrass / 2 + offsetYTerrain + 1);
            int coin = Random.Range(0, 2);
            if(coin == 0)
            {
                _xInit = Random.Range(xCenterGrass - (widthGrass / 2) - offsetXTerrain, xCenterGrass - (widthGrass / 2));
            }
            else
            {
                _xInit = Random.Range(xCenterGrass + 1 + (widthGrass / 2), xCenterGrass + (widthGrass / 2) + offsetXTerrain);
            }
        }
        else
        {
            _xInit = Random.Range(xCenterGrass - (widthGrass / 2) - offsetXTerrain, xCenterGrass + (widthGrass / 2) + offsetXTerrain + 1);
            int coin = Random.Range(0, 2);
            if (coin == 0)
            {
                _yInit = Random.Range(yCenterGrass - (heightGrass / 2) - offsetYTerrain, yCenterGrass - (heightGrass / 2));
            }
            else
            {
                _yInit = Random.Range(yCenterGrass + 1 + (heightGrass / 2), yCenterGrass + (heightGrass / 2) + offsetYTerrain);
            }
        }

        //_xInit = 0 + ((float) sign) * (width_grass / 2) + sign * (Random.Range(1, offset_x));
        //_yInit = 0 + sign * (height_grass / 2) + sign * (Random.Range(1, offset_y));
        Debug.LogFormat("{0} e {1}", widthGrass, heightGrass);

        GameObject lake = GameObject.Find("Water");
        float widthLake = lake.transform.localScale.x;
        float heightLake = lake.transform.localScale.y;
        float xCenterLake = lake.transform.position.x;
        float yCenterLake = lake.transform.position.y;
        float offsetXLake = 10f;
        float offsetYLake = 8f;

        bool generatedEnd = false;
        while (!generatedEnd)
        {
            _xEnd = Random.Range((xCenterGrass - widthLake / 2) + offsetXLake, (xCenterGrass + widthLake / 2) - offsetXLake);
            _yEnd = Random.Range((yCenterGrass - heightLake / 2) + offsetYLake, (yCenterGrass + heightLake / 2) - offsetYLake);

            float one = Mathf.Pow((2 * _xEnd) / widthLake, 2);
            float two = Mathf.Pow((2 * _yEnd) / heightLake, 2);

            float distanceFromCenter = Mathf.Sqrt(one + two);

            if(distanceFromCenter <= 1)
            {
                generatedEnd = true;
            }
        }


        _xDistance = _xEnd - _xInit;
        _yDistance = _yEnd - _yInit;

        transform.position = new Vector3(_xInit, _yInit, 0);



        // Find the sprite so we can position it in Update().
        sprite = gameObject.transform.Find("Sprite");

        // Set the initial velocity.
        initialVelocity = Random.Range(MinInitialVelocity, MaxInitialVelocity);
        velocity = initialVelocity;

        // Get the angluar velocity and zero it for the rigidbody2D.
        angularVelocity = GetComponent<Rigidbody2D>().angularVelocity;
        GetComponent<Rigidbody2D>().angularVelocity = 0f;
        Debug.Log("Height = " + _height);
        sleep = false;
        thrown = true;
    }

    void Update()
    {

        if (sleep)
        {
            return;
        }

        if(thrown == true)
        {
            thrown = false;
            initializeBreadThrow();
        }

        throwBread();


        

        
    }

    private void initializeBreadThrow()
    {
        initialVelocity = Random.Range(MinInitialVelocity, MaxInitialVelocity);
        velocity = initialVelocity;
        //sleep = false;
        float tf = 2f * (initialVelocity) / (gravity); // * Time.deltaTime);
        Debug.Log("tempo stimato = " + tf);
        IEnumerator coroutine = WaitAndPrint(tf);

        GetComponent<Rigidbody2D>().velocity = new Vector2((_xDistance / tf), (_yDistance / tf));


        StartCoroutine(coroutine);

        
    }

    private void throwBread()
    {
        // Calculate the new height.
        velocity -= gravity * Time.deltaTime;
        _height += velocity * Time.deltaTime;

        // Set the local position of the sprite to be the height.
        // Because of the initial upwards velocity it will start going up
        // and the gravity will pull it down again so it follows an arc.
        // The gameobject the sprite is attached to will move in a straight line however
        // and so will the attached shadow sprite so we'll get the illusion of 3D physics.
        sprite.localPosition = new Vector3(0, _height, 0);

        // Rotate the sprite.
        sprite.Rotate(0, 0, angularVelocity * Time.deltaTime);

        // If the height is 0 we've landed and we stop moving.
        // The rigidbody2D's velocity is what moves us in a straight line across the ground,
        // we're only faking the vertical part.
        if (_height <= 0)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            sprite.localPosition = new Vector3(0, 0, 0);
            sleep = true;
        }
    }







    private IEnumerator WaitAndPrint(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("WaitAndPrint " + waitTime);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }




}
