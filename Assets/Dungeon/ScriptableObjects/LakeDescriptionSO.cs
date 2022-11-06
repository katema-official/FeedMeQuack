using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelStageNamespace
{

    [CreateAssetMenu(fileName = "New LakeDescription", menuName = "LakeDescriptionSO")]
    public class LakeDescriptionSO : ScriptableObject
    {
        public Dictionary<EnumsDungeon.EnemyType, int> EnemiesToSpawnMap = new Dictionary<EnumsDungeon.EnemyType, int>();

        public bool HasNorthRiver;
        public bool HasSouthRiver;
        public bool HasWestRiver;
        public bool HasEastRiver;

        public EnumsDungeon.LakeDimension Dimension;

        public Dictionary<EnumsDungeon.BreadType, int> BreadToSpawnMap = new Dictionary<EnumsDungeon.BreadType, int>();

        public bool _isLakeCleared;

        public bool _isStartingRoom;

        public bool _isFinalRoom;
        public EnumsDungeon.CompassDirection _exitStageDirection;  //to consider only if _isFinalRoom = true







    }
}