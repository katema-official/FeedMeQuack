using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelStageNamespace
{

    [CreateAssetMenu(fileName = "New LakeDescription", menuName = "LakeDescriptionSO")]
    public class LakeDescriptionSO : ScriptableObject
    {
        public int NumberOfMallard;
        public int NumberOfCoot;
        public int NumberOfGoose;
        public int NumberOfFish;       //we'll think about them in the future
        public int NumberOfSeagull;    //we'll think about them in the future

        public int HasNorthRiver;
        public int HasSouthRiver;
        public int HasWestRiver;
        public int HasEastRiver;

        public EnumsDungeon.LakeDimension Dimension;

        public Dictionary<EnumsDungeon.BreadType, int> BreadToSpawnMap = new Dictionary<EnumsDungeon.BreadType, int>();







    }
}