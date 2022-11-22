using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    [CreateAssetMenu(menuName = "Player/Description/Skill/Eat")]
    public class PlayerEatSkillDescriptionSO: PlayerSkillDescriptionSO
    {
        public float EatingSpeed = 0.0f;
        public float ChewingRate = 0.0f;
        public int   MouthSize = 0;
    }
}
