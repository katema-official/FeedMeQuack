using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BreadNamespace;

namespace DuckEnemies
{
    public class IdentifyFoodComponent : MonoBehaviour
    {

        private float _circle1BreadRadius;                  //the radius of the three circles that define if a enemy
        private float _circle2BreadRadius;                  //duck saw a piece of bread
        private float _circle3BreadRadius;
        private float _circle1BreadProbability;  //probabilities that a duck sees a piece of bread
        private float _circle2BreadProbability;  //when it spawns inside a particular circle
        private float _circle3BreadProbability;

        //these sets save up the bread that was refused. Each bread gameobject is recorded as it InstanceID (GetInstanceID())
        protected HashSet<int> _refusedBreadsCircle1;
        protected HashSet<int> _refusedBreadsCircle2;
        protected HashSet<int> _refusedBreadsCircle3;
        protected GameObject _breadInWaterObjectiveGO;

        FoodCircleComponent _foodCircleComponent1;
        FoodCircleComponent _foodCircleComponent2;
        FoodCircleComponent _foodCircleComponent3;

        public void Initialize(float radius1, float radius2, float radius3, float prob1, float prob2, float prob3)
        {
            _circle1BreadRadius = radius1;
            _circle2BreadRadius = radius2;
            _circle3BreadRadius = radius3;
            _circle1BreadProbability = prob1;
            _circle2BreadProbability = prob2;
            _circle3BreadProbability = prob3;

            _refusedBreadsCircle1 = new HashSet<int>();
            _refusedBreadsCircle2 = new HashSet<int>();
            _refusedBreadsCircle3 = new HashSet<int>();
            _breadInWaterObjectiveGO = null;

            _foodCircleComponent1.GetComponent<CircleCollider2D>().radius = _circle1BreadRadius;
            _foodCircleComponent2.GetComponent<CircleCollider2D>().radius = _circle2BreadRadius;
            _foodCircleComponent3.GetComponent<CircleCollider2D>().radius = _circle3BreadRadius;
        }

        //The idea of this whole component is:
        //As long as the enemy duck exists, this takes note of EVERY breadInWater that is present in the lake, and eventually
        //stores it in one of the data structures present here.
        //Whenever someone needs to know if the duck has seen a bread, it can ask to this component about it.




        void Awake()
        {
            _foodCircleComponent1 = transform.Find("BreadCollider1").GetComponent<FoodCircleComponent>();
            _foodCircleComponent2 = transform.Find("BreadCollider2").GetComponent<FoodCircleComponent>();
            _foodCircleComponent3 = transform.Find("BreadCollider3").GetComponent<FoodCircleComponent>();
        }


        //Method used by the children circle colliders equipped with a FoodCircleComponent to notify this
        //component of the fact that a piece of bread has been identified
        public void NotifyBreadIdentified(GameObject breadGO, int idCircle)
        {
            float r = Random.Range(0f, 1f);
            switch (idCircle){
                case 1:
                    if (_breadInWaterObjectiveGO == null && !_refusedBreadsCircle1.Contains(breadGO.GetInstanceID()))
                    {
                        if (r < _circle1BreadProbability)
                        {
                            _breadInWaterObjectiveGO = breadGO;
                            Debug.Log("Circle 1 accepted");
                        }
                        else
                        {
                            _refusedBreadsCircle1.Add(breadGO.GetInstanceID());
                            _refusedBreadsCircle2.Add(breadGO.GetInstanceID());
                            _refusedBreadsCircle3.Add(breadGO.GetInstanceID());
                        }
                    }
                    break;
                case 2:
                    if (_breadInWaterObjectiveGO == null && !_refusedBreadsCircle2.Contains(breadGO.GetInstanceID()))
                    {
                        if (r < _circle2BreadProbability)
                        {
                            _breadInWaterObjectiveGO = breadGO;
                            Debug.Log("Circle 2 accepted");
                        }
                        else
                        {
                            _refusedBreadsCircle2.Add(breadGO.GetInstanceID());
                            _refusedBreadsCircle3.Add(breadGO.GetInstanceID());
                            Debug.Log("Circle 2 denyed");
                        }
                    }
                    break;
                case 3:
                    if (_breadInWaterObjectiveGO == null && !_refusedBreadsCircle3.Contains(breadGO.GetInstanceID()))
                    {
                        if (r < _circle3BreadProbability)
                        {
                            _breadInWaterObjectiveGO = breadGO;
                            Debug.Log("Circle 3 accepted");
                        }
                        else
                        {
                            _refusedBreadsCircle3.Add(breadGO.GetInstanceID());
                            Debug.Log("Circle 3 denyed");
                        }
                    }
                    break;
                default:
                    break;
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