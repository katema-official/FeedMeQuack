using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public static class SkillUtility
    {
        public static PlayerSkill CreateSkillFromDescription(PlayerSkillDescriptionSO description, GameObject gameObject)
        {
            PlayerSkill skill = null;

            if (description.Type.Name == "MoveSkill")
                skill = gameObject.AddComponent<PlayerMoveSkill>();
            else if (description.Type.Name == "EatSkill")
                skill = gameObject.AddComponent<PlayerEatSkill>();
            else if (description.Type.Name == "DashSkill")
                skill = gameObject.AddComponent<PlayerDashSkill>();
            else if (description.Type.Name == "SpitSkill")
                skill = gameObject.AddComponent<PlayerSpitSkill>();
            else if (description.Type.Name == "StealSkill")
                skill = gameObject.AddComponent<PlayerStealSkill>();

            if (skill)
            { 
                skill.SetDescription(description);
                return skill;
            }

            return null;
        }
    }
}