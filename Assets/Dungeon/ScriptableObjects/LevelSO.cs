using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelStageNamespace
{

    [CreateAssetMenu(fileName = "New Level", menuName = "Level")]
    public class LevelSO : ScriptableObject
    {
        public List<StageSO> Stages;
    }





}