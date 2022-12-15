using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuckEnemies
{
    public class DashingComponent : MonoBehaviour
    {
        private IdentifyFoodComponent _identifyFoodComponent;
        private float _dashTriggerProbability;




        public void Initialize(float dashTriggerProbability)
        {
            _dashTriggerProbability = dashTriggerProbability;
        }


        public bool WantsToDashTowardsObjectiveFood()
        {
            if (_dashTriggerProbability == 0f) return false;

            return _identifyFoodComponent.IsThereAnObjectiveFood() && (Random.Range(0f, 1f) <= _dashTriggerProbability);
        }



        void Awake()
        {
            _identifyFoodComponent = GetComponent<IdentifyFoodComponent>();
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
