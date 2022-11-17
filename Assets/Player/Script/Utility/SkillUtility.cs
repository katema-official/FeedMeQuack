using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public static class SkillUtility
    {
        public static PlayerSkill CreateSkillFromDescription(PlayerSkillDescriptionSO description, GameObject gameObject)
        {
            if (description.Type.Name == "EatSkill")
                return gameObject.AddComponent<PlayerEatSkill>();

            return null;
        }
    }
}