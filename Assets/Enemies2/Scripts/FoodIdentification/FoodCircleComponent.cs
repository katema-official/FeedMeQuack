using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuckEnemies
{
    public class FoodCircleComponent : MonoBehaviour
    {

        //this component just signals to the IdentifyFoodComponent if a piece of bread has been identified

        [SerializeField] private int ID = 0;
        IdentifyFoodComponent _identifyFoodComponent;

        void Awake()
        {
            _identifyFoodComponent = transform.parent.GetComponent<IdentifyFoodComponent>();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if(collision.gameObject.tag == "FoodInWater")
            {
                _identifyFoodComponent.NotifyFoodIdentified(collision.gameObject, ID);
            }
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
}