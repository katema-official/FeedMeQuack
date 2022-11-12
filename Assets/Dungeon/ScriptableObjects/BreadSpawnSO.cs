using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelStageNamespace
{

    [CreateAssetMenu(fileName = "New BreadSpawn", menuName = "BreadSpawnSO")]
    public class BreadSpawnSO : ScriptableObject
    {

        public float MinSpawnTime;    //minimum time that needs to pass between a bread piece and the following
        public float MaxSpawnTime;    //maximum time that can pass between a bread piece and the following

        //parameters relative to how the bread is thrown

        //the minimum and maximum value at which the fake gravity applies to the bread.
        //If it is seen that a random value between the two doesn't give a very realistic feeling, try
        //to reduce the gap between the two, or eve set it to zero (min = max)
        public float MinGravity;
        public float MaxGravity;

        //the minimum and maximum value that the initial velocity can have
        public float MinInitialVelocity;
        public float MaxInitialVelocity;


    }
}