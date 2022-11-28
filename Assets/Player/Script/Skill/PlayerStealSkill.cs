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

        private PlayerController _controller = null;
        private PlayerMoveSkill _moveSkill = null;
        private PlayerEatSkill _eatSkill = null;
        private PlayerStealSkillDescriptionSO _stealDesc = null;

        private HashSet<EnemyController> _locatedEnemies;
        private EnemyController _locatedEnemy;


        public override void SetDescription(PlayerSkillDescriptionSO desc)
        {
            _description = desc;
            _stealDesc = (PlayerStealSkillDescriptionSO)_description;

            _coolDown = _stealDesc.CoolDown;

        }

        public EnemyController FindClosestEnemy()
        {
            float _minDistance = 10000000;
            EnemyController res = null;
            foreach (var b in _locatedEnemies)
            {
                var dist = b.gameObject.transform.position - _controller.gameObject.transform.position;
                if (dist.magnitude < _minDistance)
                {
                    _minDistance = dist.magnitude;
                    res = b;
                }
            }
            return res;
        }
        void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _moveSkill = GetComponent<PlayerMoveSkill>();
            _eatSkill = GetComponent<PlayerEatSkill>();
            _locatedEnemies = new HashSet<EnemyController>();
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.X) && _eatSkill.GetCatchedBread() && _spitCoolDownElapsedSeconds <= 0)
            //{
            //    _controller.ChangeState(PlayerState.Spitting);

            //    if (_controller.GetState() == PlayerState.Spitting)
            //    {
            //        _moveSkill.EnableInput(true);
            //        _spitArrow.SetActive(true);
            //        _spitProgressBar.gameObject.SetActive(true);
            //    }

            //    CheckData();
            //}

            //if ((Input.GetKeyUp(KeyCode.Z) && _eatSkill.GetCatchedBread() && _spitCoolDownElapsedSeconds <= 0) ||
            //    (_spitPower >= _maxPower))
            //{
            //    _canSpit = true;
            //}

            //if (_canSpit && !_eatSkill.GetCatchedBread())
            //{
            //    _controller.ChangeState(PlayerState.Normal);

            //    if (_controller.GetState() == PlayerState.Normal)
            //    {
            //        _spitArrow.SetActive(false);
            //        _spitProgressBar.SetProgress(0);
            //        _spitProgressBar.gameObject.SetActive(false);

            //        _moveSkill.EnableInput(true);
            //        _spitCoolDownElapsedSeconds = _coolDown;
            //    }
            //    CheckData();
            //}


            //// _spitArrow.transform.rotation = Quaternion.//Rotate(new Vector3(0, 0, _moveSkill.GetAngle() * Mathf.Deg2Rad), Space.World);
            //Debug.Log("Spit Power: " + _moveSkill.GetAngle());
        }

        void FixedUpdate()
        {
            //if (_controller.GetState() == PlayerState.Spitting && _eatSkill.GetCatchedBread() && !_canSpit && _spitCoolDownElapsedSeconds <= 0)
            //{
            //    _moveSkill.Rotate();
            //    _eatSkill.GetCatchedBread().Move(_controller.GetMouthTransform().position);
            //    _spitArrow.transform.position = _controller.GetPosition();
            //    _spitArrow.transform.rotation = (Quaternion.AngleAxis(_moveSkill.GetAngle(), Vector3.forward));

            //    _spitProgressBar.gameObject.transform.position = _controller.GetPosition();
            //    _spitProgressBar.SetProgress((_spitPower / _maxPower));

            //    if (_spitPower < _maxPower)
            //    {
            //        _spitPower += _chargeSpeed * Time.deltaTime;
            //        // Debug.Log("Spit Power: " + _spitPower);
            //    }
            //    else
            //    {
            //        _spitPower = _maxPower;
            //        //  Debug.Log("Max Spit Power Reached: " + _spitPower);
            //    }
            //}


            //if (_controller.GetState() == PlayerState.Spitting && _eatSkill.GetCatchedBread() && _canSpit)
            //{
            //    Vector3 startPos = _controller.GetPosition();
            //    Vector3 endPos = _controller.GetPosition() + _moveSkill.GetDirection() * (_maxRange * (_spitPower / _maxPower));

            //    _eatSkill.ReleaseBread();
            //    _spitPower = 0;
            //}

            //if (_controller.GetState() != PlayerState.Spitting && _spitCoolDownElapsedSeconds > 0)
            //{
            //    _spitCoolDownElapsedSeconds -= Time.deltaTime;
            //}
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            var enemyController = collision.gameObject.GetComponent<EnemyController>();

            if (enemyController)
            {
                _locatedEnemies.Add(enemyController);
                _locatedEnemy = FindClosestEnemy();
                
                if (_locatedEnemy)
                    Debug.Log("Enemy located");
            }



        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var enemyController = collision.gameObject.GetComponent<EnemyController>();

            if (enemyController)
            {
                _locatedEnemies.Remove(enemyController);
                _locatedEnemy = FindClosestEnemy();
               
                if (!_locatedEnemy)
                    Debug.Log("Enemy missed");
            }
        }
    }
}
