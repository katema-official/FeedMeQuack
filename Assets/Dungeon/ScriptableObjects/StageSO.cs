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

        //for the bread: we specify a minimum and maximum number of bread that has to spawn, and associate to each
        //kind of bread a probability of being chosen.
        public int MinNumberOfBreadPerLake;
        public int MaxNumberOfBreadPerLake;

        //needs as many entries as the number of pieces of bread available, and their sum must be equal to 1.
        //The entries follow te same order of the BreadType enum
        [Header("List of probabilities to spawn for each BreadType.\nThey must sum up to 1")]
        [Range(0f, 1f)] public List<float> ProbabilitiesOfBread;

        public void validate()
        {
            float sum = 0f;
            for(int i = 0; i < ProbabilitiesOfBread.Count; i++)
            {
                sum += ProbabilitiesOfBread[i];
                Debug.Log("i = " + ProbabilitiesOfBread[i]);
            }
            if(sum != 1f)
            {
                Debug.LogError("Error: the sum of probabilities for the pieces of bread is not 1!");
            }
        }

    }





}