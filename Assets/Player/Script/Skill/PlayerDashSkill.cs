using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player
{
    public class PlayerDashSkill : PlayerSkill
    {
        //Dash Skill Data
        //------------------------------------------
        [SerializeField]  private float _maxSpeed = 0.0f;
        [SerializeField] private float _maxDuration = 0.0f;
        [SerializeField] private float _coolDown = 0.0f;
        //-------------------------------------
        private float _dashElapsedSeconds = 0.0f;
        private float _dashCoolDownElapsedSeconds = 0.0f;
        //-------------------------------------


        private PlayerController _controller = null;
        private PlayerMoveSkill _moveSkill = null;
        private PlayerDashSkillDescriptionSO _dashDesc = null;
        
        
        public override void SetDescription(PlayerSkillDescriptionSO desc)
        {
            _description = desc;
            _dashDesc = (PlayerDashSkillDescriptionSO)_description;

            _maxSpeed = _dashDesc.MaxSpeed;
            _maxDuration = _dashDesc.MaxDuration;
            _coolDown = _dashDesc.CoolDown;
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