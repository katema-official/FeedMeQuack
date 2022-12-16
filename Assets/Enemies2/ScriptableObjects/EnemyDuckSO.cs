using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuckEnemies
{
    [CreateAssetMenu(fileName = "new EnemyDuck", menuName = "EnemyDuckSO")]
    public class EnemyDuckSO : ScriptableObject
    {
        [Header("Roaming stuff")]
        public float SpeedRoaming;
        public float AccelerationRoaming;
        public float DecelerationRoaming;
        public float SteerRoaming;
        public float ChillingTime;
        public float DesiredRoamingDistance;
        public float StopAtRoaming;

        [Header("FoodSeeking stuff")]
        public float SpeedFoodSeeking;
        public float AccelerationFoodSeeking;
        public float DecelerationFoodSeeking;
        public float SteerFoodSeeking;
        public float StopAtFoodSeeking;


        [Header("What the duck sees")]
        public float Circle1FoodRadius;
        public float Circle2FoodRadius;
        public float Circle3FoodRadius;
        [Range(0.0f, 1.0f)] public float Circle1FoodProbability;
        [Range(0.0f, 1.0f)] public float Circle2FoodProbability;
        [Range(0.0f, 1.0f)] public float Circle3FoodProbability;

        public float Circle1PlayerRadius;
        public float Circle2PlayerRadius;
        public float Circle3PlayerRadius;
        [Range(0.0f, 1.0f)] public float Circle1PlayerProbability;
        [Range(0.0f, 1.0f)] public float Circle2PlayerProbability;
        [Range(0.0f, 1.0f)] public float Circle3PlayerProbability;


        [Header("Dash ability")]
        [Range(0.0f, 1.0f)] public float DashTriggerProbability;
        public float SpeedDash;
        public float AccelerationDash;
        public float DecelerationDash;
        public float SteerDash;

        [Header("Steal ability")]
        [Range(0.0f, 1.0f)] public float StealTriggerProbability;
        public float SpeedChasing;
        public float AccelerationChasing;
        public float DecelerationChasing;
        public float SteerChasing;

        [Header("Eating stuff")]
        public int MouthSize;
        public float ChewingRate;
        public float DigestingTime;     //how many seconds a duck stays still after eating




    }

}