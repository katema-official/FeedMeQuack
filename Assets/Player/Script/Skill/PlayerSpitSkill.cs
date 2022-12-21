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
        [SerializeField] private float _carryingSpeed = 0.0f;
        //------------------------------------------

        //-------------------------------------
        [SerializeField]  private float _spitCoolDownElapsedSeconds = 0.0f;
        [SerializeField]  private float _spitPower = 0.0f;
        private bool _canSpit = false;
        private GameObject _spitArrow = null;
        private SpitProgressBar _spitProgressBar = null;
        //-------------------------------------


        [SerializeField] private BreadNamespace.BreadInMouthComponent _caughtBread = null;

        private PlayerController _controller = null;
        private PlayerMoveSkill _moveSkill = null;
        private PlayerEatSkill _eatSkill = null;
        private PlayerBreadManager _breadManager = null;
        private PlayerSpitSkillDescriptionSO _spitDesc = null;

        public override void SetDescription(PlayerSkillDescriptionSO desc)
        {
            _description = desc;
            _spitDesc = (PlayerSpitSkillDescriptionSO)_description;

            _maxPower = _spitDesc.MaxPower;
            _maxRange = _spitDesc.MaxRange;
            _coolDown = _spitDesc.CoolDown;
            _chargeSpeed = _spitDesc.ChargeSpeed;
            _carryingSpeed = _spitDesc.CarryingSpeed;
        }
        public override void applyPowerUp(PlayerSkillAttribute attrib, float value)
        {
            if (attrib == PlayerSkillAttribute.SpitSkill_ChargeSpeed)
            {
                _chargeSpeed += value;
            }
            else if (attrib == PlayerSkillAttribute.SpitSkill_CarryingSpeed)
            {
                _carryingSpeed += value;
            }
            else if (attrib == PlayerSkillAttribute.SpitSkill_CoolDown)
            {
                _coolDown += value;
            }
            else if (attrib == PlayerSkillAttribute.SpitSkill_MaxPower)
            {
                _maxPower += value;
            }
            else if (attrib == PlayerSkillAttribute.SpitSkill_MaxRange)
            {
                _maxRange += value;
            }
        }
        private void CheckData()
        {
            if (_controller.GetState() != PlayerState.Spitting)
            {
                _canSpit = false;
            }
        }
        private BreadNamespace.BreadInWaterComponent FindClosestBread()
        {
            GameObject[] breads = GameObject.FindGameObjectsWithTag("FoodInWater");
            float minDistance = 10000000;
            BreadNamespace.BreadInWaterComponent bread = null;
            for (int i = 0; i < breads.Length; i++)
            {
                var dist = Vector3.Distance(breads[i].transform.position, _controller.gameObject.transform.position);
                if (dist <= 3f)
                {
                    if (dist <= minDistance)
                    {
                        minDistance = dist;
                        bread = breads[i].GetComponent<BreadNamespace.BreadInWaterComponent>();
                    }
                }
            }

            return bread;
        }

        void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _moveSkill = GetComponent<PlayerMoveSkill>();
            _eatSkill = GetComponent<PlayerEatSkill>();
            _breadManager = GameObject.FindObjectOfType<PlayerBreadManager>();

            _spitArrow = GameObject.Find("SpitArrow");
            _spitProgressBar = GameObject.FindObjectOfType<SpitProgressBar>();
            _spitArrow.SetActive(false);
            _spitProgressBar.gameObject.SetActive(false);
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("SpitButton") /*&& !_caughtBread*//*_eatSkill.GetCaughtBread()*/ && _spitCoolDownElapsedSeconds <= 0)
            {

                if (_controller.GetState() != PlayerState.Spitting && _controller.GetState() != PlayerState.Carrying && !_caughtBread)
                {
                    var locatedBread = FindClosestBread();

                    if (locatedBread)
                    {
                        _caughtBread = locatedBread.GenerateNewBreadInMouth(locatedBread.GetBreadPoints()).GetComponent<BreadNamespace.BreadInMouthComponent>();
                        _controller.ChangeState(PlayerState.Carrying);
                    } 
                    else
                    {
                        return;
                    }
                }
                else if (_controller.GetState() == PlayerState.Carrying && _caughtBread)
                {
                    _controller.ChangeState(PlayerState.Spitting);
                }






                if (_controller.GetState() == PlayerState.Carrying)
                {
                    //also interrupt the eating coroutine
                    //_eatSkill.StopEating();

                    _moveSkill.EnableInput(true);
                    _controller.GetUICanvas().GetStatusView().SetVisible(true);
                    //_spitArrow.SetActive(true);
                    //_spitProgressBar.gameObject.SetActive(true);
                }
                else if (_controller.GetState() == PlayerState.Spitting)
                {
                    //also interrupt the eating coroutine
                    // _eatSkill.StopEating();
                    _controller.GetUICanvas().GetStatusView().SetVisible(false);
                    _moveSkill.EnableInput(true);
                    _spitArrow.SetActive(true);
                    _spitProgressBar.gameObject.SetActive(true);
                }

                CheckData();
            }

            if (((Input.GetButtonUp("SpitButton") && _caughtBread/*_eatSkill.GetCaughtBread()*/ && _spitCoolDownElapsedSeconds <= 0) ||

                (_spitPower >= _maxPower)) && _controller.GetState() == PlayerState.Spitting)
            {
                _canSpit = true;
            }

            if (_canSpit && !_caughtBread/*_eatSkill.GetCaughtBread()*/)
            {
                _controller.ChangeState(PlayerState.Normal);

                if (_controller.GetState() == PlayerState.Normal)
                {
                    _spitArrow.SetActive(false);

                   _spitProgressBar.SetProgress(0);
                    _spitProgressBar.gameObject.SetActive(false);
                    _controller.GetUICanvas().GetStatusView().SetVisible(false);

                    _moveSkill.EnableInput(true);
                    _spitCoolDownElapsedSeconds = _coolDown;
                }
                CheckData();
            }

         
           // _spitArrow.transform.rotation = Quaternion.//Rotate(new Vector3(0, 0, _moveSkill.GetAngle() * Mathf.Deg2Rad), Space.World);
           // Debug.Log("Spit Power: " + _moveSkill.GetAngle());
        }

        void FixedUpdate()
        {
            if (_controller.GetState() == PlayerState.Carrying && _caughtBread)
            {
                _moveSkill.Move(_carryingSpeed);
                _caughtBread.Move(_controller.GetMouthTransform().position);
            }
            
            else if (_controller.GetState() == PlayerState.Spitting && _caughtBread/*_eatSkill.GetCaughtBread()*/ && !_canSpit && _spitCoolDownElapsedSeconds <= 0)
            {
                _moveSkill.Rotate();
                // _eatSkill.GetCaughtBread().Move(_controller.GetMouthTransform().position);
                _caughtBread.Move(_controller.GetMouthTransform().position);

                _spitArrow.transform.position = _controller.GetPosition();
                _spitArrow.transform.rotation = (Quaternion.AngleAxis(_moveSkill.GetAngle(), Vector3.forward));

                _spitProgressBar.gameObject.transform.position = _controller.GetPosition();
                _spitProgressBar.SetProgress((_spitPower / _maxPower));

                if (_spitPower < _maxPower)
                {
                    _spitPower += _chargeSpeed * Time.deltaTime;
                   // Debug.Log("Spit Power: " + _spitPower);
                }
                else
                {
                    _spitPower = _maxPower;
                  //  Debug.Log("Max Spit Power Reached: " + _spitPower);
                }
            }


            if (_controller.GetState() == PlayerState.Spitting && _caughtBread/*_eatSkill.GetCaughtBread()*/ && _canSpit)
            {
                Vector3 startPos = _controller.GetPosition();
                Vector3 endPos = _controller.GetPosition() + _moveSkill.GetDirection() * (_maxRange * (_spitPower / _maxPower));
                _breadManager.ThrowBread(_caughtBread/*_eatSkill.GetCaughtBread()*/, startPos, endPos);


                //_eatSkill.ReleaseBread();
                _caughtBread = null;
                _spitPower = 0;
            }

            if (_controller.GetState() != PlayerState.Spitting && _spitCoolDownElapsedSeconds > 0)
            {
                _spitCoolDownElapsedSeconds -= Time.deltaTime;

                if (_spitCoolDownElapsedSeconds < 0)
                    _spitCoolDownElapsedSeconds = 0;
            }
        }
    }
}
