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

        public bool IsLakeCleared;

        public bool IsStartingRoom;

        public bool IsFinalRoom;
        public EnumsDungeon.CompassDirection ExitStageDirection;  //to consider only if _isFinalRoom = true

        public EnumsDungeon.CompassDirection PlayerSpawnDirection;


        //after over a month of work, I decided to make lakes more complex. Each kind of lake where there is "combat" will have some obstacles,
        //described by a prefab and a method. Which actual obstacles belong to a lake, depends on this information
        public (int, List<(int, int)>) ObstaclesDescription;



    }
}