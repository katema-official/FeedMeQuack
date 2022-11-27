using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerStealSkill : PlayerSkill
    {
        //Steal Skill Data
        //------------------------------------------
        [SerializeField] private float _coolDown = 0.0f;
        //------------------------------------------
        private PlayerStealSkillDescriptionSO _stealDesc = null;


        public override void SetDescription(PlayerSkillDescriptionSO desc)
        {
            _description = desc;
            _stealDesc = (PlayerStealSkillDescriptionSO)_description;

            _coolDown = _stealDesc.CoolDown;

        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
