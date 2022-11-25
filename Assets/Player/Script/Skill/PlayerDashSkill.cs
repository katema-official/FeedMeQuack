using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player
{
    public class PlayerDashSkill : PlayerSkill
    {
        //Dash Skill Data
        //------------------------------------------
        [SerializeField] private float _maxSpeed = 0.0f;
        [SerializeField] private float _maxDuration = 0.0f;
        [SerializeField] private float _coolDown = 0.0f;
        //-------------------------------------
        [SerializeField] private float _dashElapsedSeconds = 0.0f;
        [SerializeField] private float _dashCoolDownElapsedSeconds = 0.0f;
        private float _noDashArea = 10.0f;
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

        private void CheckData()
        {
            if (_controller.GetState() == PlayerState.Dashing)
            {
                _dashCoolDownElapsedSeconds = 0;
            }
            else
            {
                _dashElapsedSeconds = 0.0f;
                _dashCoolDownElapsedSeconds = _coolDown;
            }
        }


        void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _moveSkill = GetComponent<PlayerMoveSkill>();
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q) && _dashCoolDownElapsedSeconds <= 0)
            {
                if (_controller.GetState() == PlayerState.Dashing)
                {
                    _controller.ChangeState(PlayerState.Normal);

                    if (_controller.GetState() == PlayerState.Normal)
                    {
                        _moveSkill.EnableInput(true);
                        _dashElapsedSeconds = 0.0f;
                        _dashCoolDownElapsedSeconds = _coolDown;
                    }
                }
                else
                {
                    var p = _controller.GetPosition() + _moveSkill.GetDirection() * _noDashArea;

                    if (_controller.GetLake().Contains(p))
                    {
                        _controller.ChangeState(PlayerState.Dashing);
                    }
                }

                if (_controller.GetState() == PlayerState.Dashing)
                    _moveSkill.EnableInput(false);

            }


            if (_controller.GetState() == PlayerState.Dashing && _dashElapsedSeconds >= _maxDuration && _dashCoolDownElapsedSeconds <= 0)
            {
                _controller.ChangeState(PlayerState.Normal);

                if (_controller.GetState() == PlayerState.Normal)
                    _moveSkill.EnableInput(true);

                CheckData();
            }
        }

        void FixedUpdate()
        {
            if (_controller.GetState() == PlayerState.Dashing && _dashElapsedSeconds < _maxDuration && _dashCoolDownElapsedSeconds <= 0)
            {
                _dashElapsedSeconds += Time.deltaTime;
                _moveSkill.Move(_maxSpeed, true);
            }

            if (_controller.GetState() != PlayerState.Dashing && _dashCoolDownElapsedSeconds > 0)
            {
                _dashCoolDownElapsedSeconds -= Time.deltaTime;
            }
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (_controller.GetState() == PlayerState.Dashing)
            {
                _controller.ChangeState(PlayerState.Normal);

                if (_controller.GetState() == PlayerState.Normal)
                {
                    _moveSkill.EnableInput(true);
                    _dashElapsedSeconds = 0.0f;
                    _dashCoolDownElapsedSeconds = _coolDown;
                }
            }
        }

        void OnCollisionExit2D(Collision2D other)
        {
        }


    }
}