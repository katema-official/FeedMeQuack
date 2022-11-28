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
        //-------------------------------------
        [SerializeField] private float _stealCoolDownElapsedSeconds = 0.0f;




        private PlayerController _controller = null;
        private PlayerMoveSkill _moveSkill = null;
        private PlayerEatSkill _eatSkill = null;
        private PlayerStealSkillDescriptionSO _stealDesc = null;

        private HashSet<EnemyController> _locatedEnemies;
        private EnemyController _locatedEnemy;
        private bool _canSteal = false;


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

            if (Input.GetKeyDown(KeyCode.LeftShift) && 
                _locatedEnemy && /*_locatedEnemy.GetCatchedBread() &&*/
                _controller.GetState() != PlayerState.Stealing &&
                _stealCoolDownElapsedSeconds <= 0)
            {
                _controller.ChangeState(PlayerState.Stealing);

                if (_controller.GetState() == PlayerState.Stealing)
                {
                    _moveSkill.EnableInput(false);
                    // _locatedEnemy.StealFromDuck(); //this function should allow the enemy to pass to passive steal state
                    // _qteController.StartQTEStealActive(); //let's active the Quick Time Event.
                }
            }




            //===================================================================================================
            //===================================================================================================
            //===================================================================================================
            //===================================================================================================
            //===================================================================================================





            if (_controller.GetState() == PlayerState.Stealing && _locatedEnemy && _stealCoolDownElapsedSeconds <= 0)
            {
                BreadNamespace.BreadInMouthComponent breadForDuck = null;
                BreadNamespace.BreadInMouthComponent breadForEnemy = null;

                //if (_qteController.IsFinished())
                //{
                //    (breadForDuck,breadForEnemy) = _qteController.GetResult();
                //    _locatedEnemy.NotifyFinishedSteal(breadForEnemy);
                //    _eatSkill.SetCatchedBread(breadForDuck);
                //    _stealCoolDownElapsedSeconds = _coolDown;
                //}
            }

            if (_controller.GetState() != PlayerState.Stealing && _stealCoolDownElapsedSeconds > 0)
            {
                _stealCoolDownElapsedSeconds -= Time.deltaTime;

                if (_stealCoolDownElapsedSeconds < 0)
                    _stealCoolDownElapsedSeconds = 0;
            }

        }

        void FixedUpdate()
        {
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
