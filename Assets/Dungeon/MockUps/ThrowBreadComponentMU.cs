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


    [SerializeField] public GameObject BreadToSpawnPrefab;


    private float _xInit;
    private float _yInit;
    private float _xEnd;
    private float _yEnd;
    private float _xDistance;
    private float _yDistance;

    void Start()
    {
        LevelStageNamespace.LakeDescriptionComponent wholeLakeComponent = GameObject.Find("WholeLake").GetComponent<LevelStageNamespace.LakeDescriptionComponent>();

        //choose the initial position
        (_xInit, _yInit) = wholeLakeComponent.GeneratePointOutsideLake();

        //choose the landing point
        (_xEnd, _yEnd) = wholeLakeComponent.GeneratePointInsideLake();

        


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
        float tf = 2f * (initialVelocity) / (gravity);
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

            //the breadThrown object can be destroyed, and the actual bread can be instantiated
            Instantiate(BreadToSpawnPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }







    private IEnumerator WaitAndPrint(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("WaitAndPrint " + waitTime);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }




}
