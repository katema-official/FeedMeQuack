using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LevelStageNamespace
{

    [CreateAssetMenu(fileName = "New BreadSpawn", menuName = "BreadSpawnSO")]
    public class BreadSpawnSO : ScriptableObject
    {
        //for the bread: we specify a minimum and maximum number of bread that has to spawn, and associate to each
        //kind of bread a probability of being chosen.
        public int MinNumberOfBreadPerLake;
        public int MaxNumberOfBreadPerLake;

        //needs as many entries as the number of pieces of bread available, and their sum must be equal to 1.
        //The entries follow te same order of the BreadType enum
        [Header("List of probabilities to spawn for each BreadType.\nThey must sum up to 1")]
        [Range(0f, 1f)] public float[] ProbabilitiesOfBread = new float[Enum.GetValues(typeof(EnumsDungeon.BreadType)).Length];    //the index corresponds to BreadType


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