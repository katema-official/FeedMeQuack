using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BreadNamespace
{
    public class ThrowBreadComponent2 : MonoBehaviour
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
        private GameObject _airSprite;
        private GameObject _shadow;

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

        public LevelStageNamespace.EnumsDungeon.BreadType dimension;

        //the LakeDescriptionComponent assigns to this variable the prefab of the bread that needs to be rendered when thrown, and to be spawned successively
        public GameObject BreadToSpawnPrefab;

        void Awake()
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
            _airSprite = transform.Find("AirSprite").gameObject;
            _shadow = transform.Find("Shadow").gameObject;

            // Set the initial velocity.
            initialVelocity = Random.Range(MinInitialVelocity, MaxInitialVelocity);
            velocity = initialVelocity;

            
            sleep = false;
            thrown = true;
        }

        void Update()
        {

            if (sleep)
            {
                return;
            }

            if (thrown == true)
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
            float tf = (initialVelocity) / (gravity);
            var yMax = _yInit + initialVelocity * tf - (0.5f) * gravity * Mathf.Pow(tf, 2);

            while (yMax < _yEnd)
            {
                //Debug.LogFormat("NOOO yMax = {0}, _yEnd = {1}. First should be greater than second.", yMax, _yEnd);
                initialVelocity += 5f;
                tf = (initialVelocity) / (gravity);
                yMax = _yInit + initialVelocity * tf - (0.5f) * gravity * Mathf.Pow(tf, 2);
            }

            velocity = initialVelocity;
            tf += Mathf.Sqrt((2 * Mathf.Abs(yMax - _yEnd)) / gravity);

            _shadow.GetComponent<Rigidbody2D>().velocity = new Vector2((_xDistance / tf), (_yDistance / tf));
            _airSprite.GetComponent<Rigidbody2D>().velocity = new Vector2((_xDistance / tf), velocity);


        }

        private void throwBread()
        {
            velocity -= gravity * Time.deltaTime;

            _airSprite.GetComponent<Rigidbody2D>().velocity = new Vector2(_airSprite.GetComponent<Rigidbody2D>().velocity.x, velocity);


            if (_airSprite.transform.position.y <= _shadow.transform.position.y)
            {
                sleep = true;

                //the breadThrown object can be destroyed, and the actual bread can be instantiated
                GameObject b = Instantiate(BreadToSpawnPrefab, _shadow.transform.position, Quaternion.identity);
                b.GetComponent<BreadComponent>().InitializedBread(dimension);
                Destroy(gameObject);
            }
        }

    }
}