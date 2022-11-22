using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public class PlayerSkill : MonoBehaviour
    {
        protected PlayerSkillDescriptionSO _description = null;
        


        public virtual void SetDescription(PlayerSkillDescriptionSO desc)
        {
            _description = desc;
        }
    }
}
