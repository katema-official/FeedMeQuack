using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerSpitSkill : PlayerSkill
    {
        //Spit Skill Data
        //------------------------------------------
        [SerializeField] private float _maxPower = 0.0f;
        [SerializeField] private float _maxRange = 0.0f;
        [SerializeField] private float _coolDown = 0.0f;
        [SerializeField] private float _chargeSpeed = 0.0f;
        //------------------------------------------

        private PlayerController _controller = null;
        private PlayerMoveSkill _moveSkill = null;
        private PlayerSpitSkillDescriptionSO _spitDesc = null;

        private BreadController _locatedBread = null;
        private BreadController _catchedBread = null;

        public override void SetDescription(PlayerSkillDescriptionSO desc)
        {
            _description = desc;
            _spitDesc = (PlayerSpitSkillDescriptionSO)_description;

            _maxPower = _spitDesc.MaxPower;
            _maxRange = _spitDesc.MaxRange ;
            _coolDown = _spitDesc.CoolDown;
            _chargeSpeed = _spitDesc.ChargeSpeed;

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
