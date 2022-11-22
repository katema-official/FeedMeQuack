using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    [CreateAssetMenu(menuName = "Player/Description/Skill/SkillType")]
    public class PlayerSkillTypeSO : ScriptableObject
    {
        public string Name = "";
        public string Description = "";
    }
}