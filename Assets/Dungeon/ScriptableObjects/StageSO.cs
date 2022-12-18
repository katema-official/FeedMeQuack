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

        public int BreadPointsRequiredToCompleteStage;

        //for each kind of lake (small, medium and large) we specify, with other scriptable objects, how to handle the spawn of bread and enemies.
        [Header("List of BreadSpawnSO that describe how a bread piece\nwill be generated in a lake of a certain type.\nThe index corresponds to a lakeDimension value")]
        public BreadSpawnSO[] ListBreadSpawnSO = new BreadSpawnSO[Enum.GetValues(typeof(EnumsDungeon.LakeDimension)).Length];
        public EnemySpawnSO[] ListEnemySpawnSO = new EnemySpawnSO[Enum.GetValues(typeof(EnumsDungeon.LakeDimension)).Length];

        

        


        


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

            /*sum = 0f;
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
            }*/
        }

        private void OnEnable()
        {
            validate();
        }

    }





}