using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BreadNamespace;
using LevelStageNamespace;

namespace DuckEnemies
{
    public class IdentifyFoodComponent : MonoBehaviour
    {

        private float _circle1FoodRadius;                  //the radius of the three circles that define if a enemy
        private float _circle2FoodRadius;                  //duck saw a piece of bread
        private float _circle3FoodRadius;
        private float _circle1FoodProbability;  //probabilities that a duck sees a piece of bread
        private float _circle2FoodProbability;  //when it spawns inside a particular circle
        private float _circle3FoodProbability;

        //these sets save up the bread that was refused. Each bread gameobject is recorded as it InstanceID (GetInstanceID())
        private HashSet<int> _refusedFoodCircle1;
        private HashSet<int> _refusedFoodCircle2;
        private HashSet<int> _refusedFoodCircle3;
        private HashSet<GameObject> _identifiedFoods;
        //[SerializeField] private GameObject _foodInWaterObjectiveGO;
        //[SerializeField] private int _foodInWaterObjectiveID;

        private FoodCircleComponent _foodCircleComponent1;
        private FoodCircleComponent _foodCircleComponent2;
        private FoodCircleComponent _foodCircleComponent3;

        private LakeDescriptionComponent _lakeDescriptionComponent;

        public void Initialize(float radius1, float radius2, float radius3, float prob1, float prob2, float prob3)
        {
            _circle1FoodRadius = radius1;
            _circle2FoodRadius = radius2;
            _circle3FoodRadius = radius3;
            _circle1FoodProbability = prob1;
            _circle2FoodProbability = prob2;
            _circle3FoodProbability = prob3;

            _refusedFoodCircle1 = new HashSet<int>();
            _refusedFoodCircle2 = new HashSet<int>();
            _refusedFoodCircle3 = new HashSet<int>();
            _identifiedFoods = new HashSet<GameObject>();
            //_foodInWaterObjectiveGO = null;
            //_foodInWaterObjectiveID = -1;

            _foodCircleComponent1.GetComponent<CircleCollider2D>().radius = _circle1FoodRadius;
            _foodCircleComponent2.GetComponent<CircleCollider2D>().radius = _circle2FoodRadius;
            _foodCircleComponent3.GetComponent<CircleCollider2D>().radius = _circle3FoodRadius;
        }

        //The idea of this whole component is:
        //As long as the enemy duck exists, this takes note of EVERY breadInWater that is present in the lake, and eventually
        //stores it in one of the data structures present here.
        //Whenever someone needs to know if the duck has seen a bread, it can ask to this component about it.




        void Awake()
        {
            _foodCircleComponent1 = transform.Find("FoodCollider1").GetComponent<FoodCircleComponent>();
            _foodCircleComponent2 = transform.Find("FoodCollider2").GetComponent<FoodCircleComponent>();
            _foodCircleComponent3 = transform.Find("FoodCollider3").GetComponent<FoodCircleComponent>();
            _lakeDescriptionComponent = GameObject.Find("WholeLake").GetComponent<LakeDescriptionComponent>();
        }


        //Method used by the children circle colliders equipped with a FoodCircleComponent to notify this
        //component of the fact that a piece of bread has been identified
        public void NotifyFoodIdentified(GameObject foodGO, int idCircle)
        {
            if (_lakeDescriptionComponent.IsBreadInWaterInLake(foodGO.transform.position, foodGO.GetComponent<CircleCollider2D>().radius) == false) return;
            float r = Random.Range(0f, 1f);
            switch (idCircle){
                case 1:
                    if(_identifiedFoods.Contains(foodGO) == false && !_refusedFoodCircle1.Contains(foodGO.GetInstanceID()))
                    //if ((_foodInWaterObjectiveGO == null || _foodInWaterObjectiveGO.GetInstanceID() != _foodInWaterObjectiveID) && !_refusedFoodCircle1.Contains(foodGO.GetInstanceID()))
                    {
                        if (r < _circle1FoodProbability)
                        {
                            //_foodInWaterObjectiveGO = foodGO;
                            //_foodInWaterObjectiveID = _foodInWaterObjectiveGO.GetInstanceID();
                            SaveIdentifiedFood(foodGO);
                            Debug.Log("Circle 1 accepted");
                        }
                        else
                        {
                            _refusedFoodCircle1.Add(foodGO.GetInstanceID());
                            _refusedFoodCircle2.Add(foodGO.GetInstanceID());
                            _refusedFoodCircle3.Add(foodGO.GetInstanceID());
                        }
                    }
                    break;
                case 2:
                    if(_identifiedFoods.Contains(foodGO) == false && !_refusedFoodCircle2.Contains(foodGO.GetInstanceID()))
                    //if ((_foodInWaterObjectiveGO == null || _foodInWaterObjectiveGO.GetInstanceID() != _foodInWaterObjectiveID) && !_refusedFoodCircle2.Contains(foodGO.GetInstanceID()))
                    {
                        if (r < _circle2FoodProbability)
                        {
                            //_foodInWaterObjectiveGO = foodGO;
                            //_foodInWaterObjectiveID = _foodInWaterObjectiveGO.GetInstanceID();
                            SaveIdentifiedFood(foodGO);
                            Debug.Log("Circle 2 accepted");
                        }
                        else
                        {
                            _refusedFoodCircle2.Add(foodGO.GetInstanceID());
                            _refusedFoodCircle3.Add(foodGO.GetInstanceID());
                            Debug.Log("Circle 2 denyed");
                        }
                    }
                    break;
                case 3:
                    if (_identifiedFoods.Contains(foodGO) == false && !_refusedFoodCircle3.Contains(foodGO.GetInstanceID()))
                    //if ((_foodInWaterObjectiveGO == null || _foodInWaterObjectiveGO.GetInstanceID() != _foodInWaterObjectiveID) && !_refusedFoodCircle3.Contains(foodGO.GetInstanceID()))
                    {
                        if (r < _circle3FoodProbability)
                        {
                            //_foodInWaterObjectiveGO = foodGO;
                            //_foodInWaterObjectiveID = _foodInWaterObjectiveGO.GetInstanceID();
                            SaveIdentifiedFood(foodGO);
                            Debug.Log("Circle 3 accepted");
                        }
                        else
                        {
                            _refusedFoodCircle3.Add(foodGO.GetInstanceID());
                            Debug.Log("Circle 3 denyed");
                        }
                    }
                    break;
                default:
                    break;
            }

        }


        private void SaveIdentifiedFood(GameObject foodGO)
        {
            _identifiedFoods.Add(foodGO);
        }


        public bool IsThereAnObjectiveFood()
        {
            _identifiedFoods.RemoveWhere(x => x == null);
            if(_identifiedFoods.Count == 0)
            {
                return false;
            }
            return true;

            //return (_foodInWaterObjectiveGO != null && _foodInWaterObjectiveGO.GetInstanceID() == _foodInWaterObjectiveID);
        }

        //function to call whenever we want to clean the data structures.
        //Basically, call this function when you want the duck do be aware again of every possible
        //BreadInWater that is in this lake.
        public void ForgetAboutAllFood()
        {
            _refusedFoodCircle1.Clear();
            _refusedFoodCircle2.Clear();
            _refusedFoodCircle3.Clear();
            _identifiedFoods.Clear();
            //_foodInWaterObjectiveGO = null;
            //_foodInWaterObjectiveID = -1;
        }

        
        public GameObject GetObjectiveFood()
        {
            float minDist = 10000f;
            GameObject foodClosest = null;
            foreach(GameObject foodGO in _identifiedFoods)
            {
                if (foodGO != null)
                {
                    //Debug.Log("foodGO identified = " + foodGO);
                    float currentDist = Vector2.Distance(transform.position, foodGO.transform.position);
                    if (currentDist < minDist)
                    {
                        minDist = currentDist;
                        foodClosest = foodGO;
                    }
                }
                else
                {
                    //Debug.Log("foodGO was null :(");
                }

            }

            return foodClosest;

            //return _foodInWaterObjectiveGO;
        }


    }
}