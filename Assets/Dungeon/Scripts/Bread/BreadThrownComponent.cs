using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BreadNamespace
{
    public class BreadThrownComponent : MonoBehaviour
    {
        //code based on:
        //https://forum.unity.com/threads/simulate-gravity-in-a-top-down-2d-rpg.293712/


        // The fake gravity pulling this object down along the y-axis.
        [SerializeField] public float _gravity = 100f;

        // The velocity we're moving at. Only for the y-axis.
        [SerializeField] private float _velocity;

        // Our initial velocity up along the y-axis.
        float initialVelocity;
        [SerializeField] public float MinInitialVelocity = 75f;
        [SerializeField] public float MaxInitialVelocity = 125f;

        // Our sprite which is a child of this gameobject. This is the the thing
        // the player sees so this is the object we manipulate with our fake gravity.
        private GameObject _airSprite;
        private GameObject _shadow;

        // A flag we set to true when our sprite has landed on our fake ground plane.
        [SerializeField] private bool _sleep = true;

        // A place for us to store the angularVelocity for the rigidbody2D. We don't want
        // to rotate the entire rigidbody because that would offset the shadow sprite which
        // follows the ground plane so we rotate the sprite by this velocity instead.
        float angularVelocity;

        private int _breadPoints = 0; //if it's zero, it means that the actual value of bread points will be decided when the BreadInWater is created.
        //Otherwise, this is the number of bread points that the new BreadInWater will have (this is the case if the player spits the bread)



        private float _xInit;
        private float _yInit;
        private float _xEnd;
        private float _yEnd;
        private float _xDistance;
        private float _yDistance;

        public LevelStageNamespace.LakeDescriptionComponent wholeLakeComponent;

        //the LakeDescriptionComponent assigns to this variable the prefab of the bread that needs to be rendered when thrown, and to be spawned successively
        public GameObject BreadToSpawnPrefab;

        private LevelStageNamespace.EnumsDungeon.BreadType _dimension;

        void Awake()
        {
            wholeLakeComponent = GameObject.Find("WholeLake").GetComponent<LevelStageNamespace.LakeDescriptionComponent>();

            
            //Find the sprite so we can position it in Update().
            _airSprite = transform.Find("AirSprite").gameObject;
            _shadow = transform.Find("Shadow").gameObject;

            
            _sleep = true;
        }

        void Update()
        {

            if (_sleep)
            {
                return;
            }

            /*if (thrown == true)
            {
                thrown = false;
                initializeBreadThrow();
            }*/

            throwBread();
        }












        public void InitializeBreadThrownFromDuck(LevelStageNamespace.EnumsDungeon.BreadType dimension, int breadPoints, float xInit, float yInit, float xEnd, float yEnd,
                                                    float minInitialVelocity = 75f, float maxInitialVelocity = 125f, float gravity = 100f)
        {
            _dimension = dimension;
            _breadPoints = breadPoints;

            _xInit = xInit;
            _yInit = yInit;
            _xEnd = xEnd;
            _yEnd = yEnd;
            MinInitialVelocity = minInitialVelocity;
            MaxInitialVelocity = maxInitialVelocity;
            _gravity = gravity;
            

            InitializeBreadThrown();
        }


        //to call after a ThrowBread gameObject has been created when the bread is thrown from outside lake
        public void InitializeBreadThrownFromPeople(LevelStageNamespace.EnumsDungeon.BreadType dimension, float minInitialVelocity = 75f, float maxInitialVelocity = 125f, float gravity = 100f)
        {
            _dimension = dimension;

            //choose the initial position
            (_xInit, _yInit) = wholeLakeComponent.GeneratePointOutsideLake();

            //choose the landing point
            (_xEnd, _yEnd) = wholeLakeComponent.GeneratePointInsideLake();

            MinInitialVelocity = minInitialVelocity;
            MaxInitialVelocity = maxInitialVelocity;
            _gravity = gravity;
            

            InitializeBreadThrown();
            

        }


        private void InitializeBreadThrown()
        {
            Sprite sprite = null;
            switch (_dimension)
            {
                case LevelStageNamespace.EnumsDungeon.BreadType.Small:
                    sprite = BreadToSpawnPrefab.GetComponent<BreadInWaterComponent>().GetBreadSmallSprite();
                    break;
                case LevelStageNamespace.EnumsDungeon.BreadType.Medium:
                    sprite = BreadToSpawnPrefab.GetComponent<BreadInWaterComponent>().GetBreadMediumSprite();
                    break;
                case LevelStageNamespace.EnumsDungeon.BreadType.Large:
                    sprite = BreadToSpawnPrefab.GetComponent<BreadInWaterComponent>().GetBreadLargeSprite();
                    break;
            }
            _airSprite.GetComponent<SpriteRenderer>().sprite = sprite;

            Vector2 spriteSize = sprite.rect.size;                      //to remove
            float xPixelSprite = 100f;                                  //to remove
            float yPixelSprite = 100f;                                  //to remove
            float amountToDivideX = spriteSize.x / xPixelSprite;        //to remove
            float amountToDivideY = spriteSize.y / yPixelSprite;        //to remove
            _airSprite.transform.localScale = new Vector3(_airSprite.transform.localScale.x / amountToDivideX, _airSprite.transform.localScale.y / amountToDivideY, 0);      //to remove


            _xDistance = _xEnd - _xInit;
            _yDistance = _yEnd - _yInit;

            transform.position = new Vector3(_xInit, _yInit, 0);


            initialVelocity = Random.Range(MinInitialVelocity, MaxInitialVelocity);
            _velocity = initialVelocity;
            float tf = (initialVelocity) / (_gravity);
            var yMax = _yInit + initialVelocity * tf - (0.5f) * _gravity * Mathf.Pow(tf, 2);

            while (yMax < _yEnd)
            {
                //Debug.LogFormat("NOOO yMax = {0}, _yEnd = {1}. First should be greater than second.", yMax, _yEnd);
                initialVelocity += 5f;
                tf = (initialVelocity) / (_gravity);
                yMax = _yInit + initialVelocity * tf - (0.5f) * _gravity * Mathf.Pow(tf, 2);
            }

            _velocity = initialVelocity;
            tf += Mathf.Sqrt((2 * Mathf.Abs(yMax - _yEnd)) / _gravity);

            _shadow.GetComponent<Rigidbody2D>().velocity = new Vector2((_xDistance / tf), (_yDistance / tf));
            _airSprite.GetComponent<Rigidbody2D>().velocity = new Vector2((_xDistance / tf), _velocity);

            _sleep = false;
        }











        private void throwBread()
        {
            _velocity -= _gravity * Time.deltaTime;

            _airSprite.GetComponent<Rigidbody2D>().velocity = new Vector2(_airSprite.GetComponent<Rigidbody2D>().velocity.x, _velocity);


            if (_airSprite.transform.position.y <= _shadow.transform.position.y)
            {
                _sleep = true;

                //the breadThrown object can be destroyed, and the actual bread can be instantiated
                GameObject b = Instantiate(BreadToSpawnPrefab, _shadow.transform.position, Quaternion.identity);
                b.GetComponent<BreadInWaterComponent>().InitializeBread(_dimension, _breadPoints);
                Destroy(gameObject);
            }

        }

    }
}