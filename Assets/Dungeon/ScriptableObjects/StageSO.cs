using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LevelStageNamespace
{
    
    [CreateAssetMenu(fileName = "New Stage", menuName = "StageSO")]
    public class StageSO : ScriptableObject
    {
        //for the lakes: we specify a minimum and maximum number of lakes that need to spawn in this stage,
        //and associate to each kind of lake a probability to be spawned. The sum of these probabilities
        //must be one.
        public int MinNumberOfLakes;
        public int MaxNumberOfLakes;
        [Header("List of probabilities to spawn for each LakeDimension.\nThey must sum up to 1")]
        [Range(0f, 1f)] public float[] ProbabilitiesOfLake = new float[Enum.GetValues(typeof(EnumsDungeon.LakeDimension)).Length];   //the index corresponds to LakeDimension

        //for the bread: we specify a minimum and maximum number of bread that has to spawn, and associate to each
        //kind of bread a probability of being chosen.
        public int MinNumberOfBreadPerLake;
        public int MaxNumberOfBreadPerLake;

        //needs as many entries as the number of pieces of bread available, and their sum must be equal to 1.
        //The entries follow te same order of the BreadType enum
        [Header("List of probabilities to spawn for each BreadType.\nThey must sum up to 1")]
        [Range(0f, 1f)] public float[] ProbabilitiesOfBread = new float[Enum.GetValues(typeof(EnumsDungeon.BreadType)).Length];    //the index corresponds to BreadType

        //for the enemies: we specify a minimum and maximum number of enemies that need to spawn in each lake,
        //and associate to each of them a probability of being spawned in each lake
        //PROBLEMS: the generated enemies could deviate a lot from the expected value.
        //POSSIBLE SOLUTION: give a minimum amount of enemies to spawn for each type.

        public int MinNumberOfEnemiesPerLake;
        public int MaxNumberOfEnemiesPerLake;

        [Header("List of probabilities to spawn for each EnemyType.\nThey must sum up to 1")]
        [Range(0f, 1f)] public float[] ProbabilitiesOfEnemies = new float[Enum.GetValues(typeof(EnumsDungeon.EnemyType)).Length];  //the index corresponds to EnemyType

        [Header("List of minimum number of enemies to spawn for each EnemyType.\nTheir sum should be lower or equal than the MinNumberOfEnemiesPerLake")]
        public int[] MinNumberOfEachEnemy = new int[Enum.GetValues(typeof(EnumsDungeon.EnemyType)).Length];  //we assume that: MinNumberOfEachEnemy[0] = Mallard, MinNumberOfEachEnemy[1] = Coot, ...
        //this is possible since there is a corrispondence between the numbers between 0 and 4 and the values of the EnemyType enum.

        public int BreadPointsRequiredToCompleteStage;


        [Header("List of BreadSpawnSO that describe how a bread piece\nwill be generated in a lake of a certain type.\nThe index corresponds to a lakeDimension value")]
        public BreadSpawnSO[] ListBreadSpawnSO = new BreadSpawnSO[Enum.GetValues(typeof(EnumsDungeon.LakeDimension)).Length];


        public void validate()
        {
            float sum = 0f;
            for (int i = 0; i < ProbabilitiesOfLake.Length; i++)
            {
                sum += ProbabilitiesOfLake[i];
                //Debug.Log("i = " + ProbabilitiesOfLake[i]);
            }
            if (sum != 1f)
            {
                Debug.LogError("Error: the sum of probabilities for the lakes is not 1!");
            }

            sum = 0f;
            for(int i = 0; i < ProbabilitiesOfBread.Length; i++)
            {
                sum += ProbabilitiesOfBread[i];
                //Debug.Log("i = " + ProbabilitiesOfBread[i]);
            }
            if(sum != 1f)
            {
                Debug.LogError("Error: the sum of probabilities for the pieces of bread is not 1!");
            }

            sum = 0f;
            for (int i = 0; i < ProbabilitiesOfEnemies.Length; i++)
            {
                sum += ProbabilitiesOfEnemies[i];
                //Debug.Log("i = " + ProbabilitiesOfEnemies[i]);
            }
            if (sum != 1f)
            {
                Debug.LogError("Error: the sum of probabilities for the enemies is not 1!");
            }

            if((ProbabilitiesOfLake.Length != Enum.GetValues(typeof(EnumsDungeon.LakeDimension)).Length) ||
                (ProbabilitiesOfBread.Length != Enum.GetValues(typeof(EnumsDungeon.BreadType)).Length) ||
                    (ProbabilitiesOfEnemies.Length != Enum.GetValues(typeof(EnumsDungeon.EnemyType)).Length)){
                        Debug.Log("How could you change the length of my arrays? You monster");
            }

            int minEnemiesRequired = 0;
            for(int i = 0; i < MinNumberOfEachEnemy.Length; i++)
            {
                minEnemiesRequired += MinNumberOfEachEnemy[i];
            }
            if(minEnemiesRequired > MinNumberOfEnemiesPerLake)
            {
                Debug.Log("Error: the minimum number of enemies required exceeds the minimum number of enemies to spawn (could give problems)");
            }
        }

        private void OnEnable()
        {
            validate();
        }

    }





}