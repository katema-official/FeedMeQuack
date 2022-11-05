using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelStageNamespace
{
    
    [CreateAssetMenu(fileName = "New Stage", menuName = "StageSO")]
    public class StageSO : ScriptableObject
    {
        public int MinNumberOfLakes;
        public int MaxNumberOfLakes;
        [Range(0f,1f)] public float ProbabilityOfMediumLake;
        [Range(0f,1f)] public float ProbabilityOfBigLake;

        //private int LastLake;   //must be a lakedescription, actually


    }





}