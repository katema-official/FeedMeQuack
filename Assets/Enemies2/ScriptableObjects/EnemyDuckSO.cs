using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuckEnemies
{
    [CreateAssetMenu(fileName = "new EnemyDuck", menuName = "EnemyDuckSO")]
    public class EnemyDuckSO : ScriptableObject
    {
        [Header("Roaming and FoodSeeking stuff")]
        public float SpeedRoaming;
        public float AccelerationRoaming;
        public float DecelerationRoaming;
        public float SteerRoaming;
        public float ChillingTime;

        public float SpeedFoodSeeking;
        public float AccelerationFoodSeeking;
        public float DecelerationFoodSeeking;
        public float SteerFoodSeeking;


        [Header("What the duck sees")]
        public float Circle1BreadRadius;
        public float Circle2BreadRadius;
        public float Circle3BreadRadius;
        [Range(0.0f, 1.0f)] public float Circle1BreadProbability;
        [Range(0.0f, 1.0f)] public float Circle2BreadProbability;
        [Range(0.0f, 1.0f)] public float Circle3BreadProbability;

        public float Circle1PlayerRadius;
        public float Circle2PlayerRadius;
        public float Circle3PlayerRadius;
        [Range(0.0f, 1.0f)] public float Circle1PlayerProbability;
        [Range(0.0f, 1.0f)] public float Circle2PlayerProbability;
        [Range(0.0f, 1.0f)] public float Circle3PlayerProbability;


        [Header("Dash and Steal abilities")]
        [Range(0.0f, 1.0f)] public float DashTriggerProbability;
        [Range(0.0f, 1.0f)] public float StealTriggerProbability;
        public float SpeedDash;
        public float AccelerationDash;
        public float DecelerationDash;
        public float SteerDash;

        public float SpeedChasing;
        public float AccelerationChasing;
        public float DecelerationChasing;
        public float SteerChasing;

        [Header("Eating stuff")]
        public int MouthSize;
        public float ChewingRate;




    }

}