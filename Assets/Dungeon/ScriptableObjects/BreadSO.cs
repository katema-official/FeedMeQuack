using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BreadNamespace
{
    [CreateAssetMenu(fileName = "New Bread", menuName = "BreadSO")]
    public class BreadSO : ScriptableObject
    {
        public int MinAmountOfBreadPoints;
        public int MaxAmountOfBreadPoints;
        public LevelStageNamespace.EnumsDungeon.BreadType dimension;

    }
}
