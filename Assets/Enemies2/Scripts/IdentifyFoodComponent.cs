using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }

        //The idea of this whole component is:
        //As long as the enemy duck exists, this takes note of EVERY breadInWater that is present in the lake, and eventually
        //stores it in one of the data structures present here.
        //Whenever someone needs to know if the duck has seen a bread, it can ask to this component about it.


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