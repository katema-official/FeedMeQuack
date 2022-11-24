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
      
        //-------------------------------------
        private float _spitPower = 0.0f;
        private bool _canSpit = false;
        //-------------------------------------



        private PlayerController _controller = null;
        private PlayerMoveSkill _moveSkill = null;
        private PlayerEatSkill _eatSkill = null;
        private PlayerSpitSkillDescriptionSO _spitDesc = null;

        public override void SetDescription(PlayerSkillDescriptionSO desc)
        {
            _description = desc;
            _spitDesc = (PlayerSpitSkillDescriptionSO)_description;

            _maxPower = _spitDesc.MaxPower;
            _maxRange = _spitDesc.MaxRange ;
            _coolDown = _spitDesc.CoolDown;
            _chargeSpeed = _spitDesc.ChargeSpeed;
        }

        private void CheckData()
        {
            if (_controller.GetState() != PlayerState.Spitting)
            {
                _canSpit = false;
            }
        }


        void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _moveSkill = GetComponent<PlayerMoveSkill>();
            _eatSkill = GetComponent<PlayerEatSkill>();
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z) && _eatSkill.GetCatchedBread())
            {
                _controller.ChangeState(PlayerState.Spitting);
                CheckData();
            }

            if (Input.GetKeyUp(KeyCode.Z) && _eatSkill.GetCatchedBread())
            {
                _canSpit = true;
            }

            if (_canSpit && !_eatSkill.GetCatchedBread())
            {
                _controller.ChangeState(PlayerState.Normal);
                CheckData();
            }
        }

        void FixedUpdate()
        {

        }
    }
}
