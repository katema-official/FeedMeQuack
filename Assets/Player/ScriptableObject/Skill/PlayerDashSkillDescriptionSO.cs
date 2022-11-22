using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    [CreateAssetMenu(menuName = "Player/Description/Skill/Dash")]
    public class PlayerDashSkillDescriptionSO : PlayerSkillDescriptionSO
    {
        public float MaxSpeed = 0.0f;
        public float MaxDuration = 0.0f;
        public float CoolDown = 0.0f;
    }
}
