using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BreadNamespace
{
    [CreateAssetMenu(fileName = "New Bread", menuName = "BreadSO")]
    public class BreadSO : ScriptableObject
    {
        [Header("Min and Max value that a bread\nof this type can have")]
        public int MinBreadPoints;
        public int MaxBreadPoints;
        [Header("Min and Max value that a bread\nof this type can have when spawned")]
        public int MinBreadPointsSpawn;
        public int MaxBreadPointsSpawn;
        [Header("The type of this bread")]
        public LevelStageNamespace.EnumsDungeon.BreadType dimension;

    }
}
