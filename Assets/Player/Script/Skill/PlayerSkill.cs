using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public class PlayerSkill : MonoBehaviour
    {
        protected PlayerSkillDescriptionSO _description = null;


        public virtual PlayerSkillDescriptionSO GetDescription()
        {
            return _description;
        }
        public virtual void SetDescription(PlayerSkillDescriptionSO desc)
        {
            _description = desc;
        }

        public virtual void applyPowerUp(PlayerSkillAttribute attrib, float value)
        {
        }
    }
}
