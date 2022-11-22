using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    [CreateAssetMenu(menuName = "Player/Description/Skill/Move")]
    public class PlayerMoveSkillDescriptionSO : PlayerSkillDescriptionSO
    {
        public float Speed = 0.0f;
    }
}
