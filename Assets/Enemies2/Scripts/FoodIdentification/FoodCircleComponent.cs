using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuckEnemies
{
    public class FoodCircleComponent : MonoBehaviour
    {

        //this component just signals to the IdentifyFoodComponent if a piece of bread has been identified

        [SerializeField] private int ID = 0;
        private IdentifyFoodComponent _identifyFoodComponent;
        private float _radius;

        void Awake()
        {
            _identifyFoodComponent = transform.parent.GetComponent<IdentifyFoodComponent>();
            
        }

        void Start()
        {
            _radius = GetComponent<CircleCollider2D>().radius;
            Debug.Log("RADIUS = " + _radius);
        }

        /*private void OnTriggerStay2D(Collider2D collision)
        {
            if(collision.gameObject.tag == "FoodInWater")
            {
                _identifyFoodComponent.NotifyFoodIdentified(collision.gameObject, ID);
            }
        }*/


        private void OnTriggerEnter2D(Collider2D collision)
        {
            CheckBread(collision);
        }

        private void CheckBread(Collider2D collision)
        {
            if (collision.CompareTag("FoodInWater"))
            {
                //Debug.Log("ID = " + ID + ", COLLISION: " + collision.gameObject.name);
                _identifyFoodComponent.NotifyFoodIdentified(collision.gameObject, ID);
            }
        }

        public void NotifyBreadNear(Collider2D collision)
        {
            //Debug.Log("ID = " + ID + ", DISTANCE: " + Vector2.Distance(transform.position, collision.gameObject.transform.position) + ", RADIUS = " + _radius);
            if (Vector2.Distance(transform.position, collision.gameObject.transform.position) > _radius + 0.5f) return;
            CheckBread(collision);
        }

    }
}