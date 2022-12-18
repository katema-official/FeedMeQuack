using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LevelStageNamespace
{
    [CreateAssetMenu(fileName = "New EnemySpawn", menuName = "EnemySpawnSO")]
    public class EnemySpawnSO : ScriptableObject
    {
        //for the enemies: we specify a minimum and maximum number of enemies that need to spawn in a lake,
        //and associate to each of them a probability of being spawned in a lake
        //PROBLEMS: the generated enemies could deviate a lot from the expected value.
        //POSSIBLE SOLUTION: give a minimum amount of enemies to spawn for each type.

        public int MinNumberOfEnemiesPerLake;
        public int MaxNumberOfEnemiesPerLake;

        [Header("List of probabilities to spawn for each EnemyType.\nThey must sum up to 1")]
        [Range(0f, 1f)] public float[] ProbabilitiesOfEnemies = new float[Enum.GetValues(typeof(EnumsDungeon.EnemyType)).Length];  //the index corresponds to EnemyType

        [Header("List of minimum number of enemies to spawn for each EnemyType.\nTheir sum should be lower or equal than the MinNumberOfEnemiesPerLake")]
        public int[] MinNumberOfEachEnemy = new int[Enum.GetValues(typeof(EnumsDungeon.EnemyType)).Length];  //we assume that: MinNumberOfEachEnemy[0] = Mallard, MinNumberOfEachEnemy[1] = Coot, ...
        //this is possible since there is a corrispondence between the numbers between 0 and 4 and the values of the EnemyType enum.

    }
}