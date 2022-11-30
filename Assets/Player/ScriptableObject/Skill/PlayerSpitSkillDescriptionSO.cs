using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    [CreateAssetMenu(menuName = "Player/Description/Skill/Spit")]
    public class PlayerSpitSkillDescriptionSO : PlayerSkillDescriptionSO
    {
        public float MaxPower = 0.0f;
        public float MaxRange = 0.0f;
        public float CoolDown = 0.0f;
        public float ChargeSpeed = 0.0f;
    }
}