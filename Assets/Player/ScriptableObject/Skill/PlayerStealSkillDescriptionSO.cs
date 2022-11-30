using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    [CreateAssetMenu(menuName = "Player/Description/Skill/Steal")]
    public class PlayerStealSkillDescriptionSO : PlayerSkillDescriptionSO
    {
        public float CoolDown = 0.0f;
    }
}
