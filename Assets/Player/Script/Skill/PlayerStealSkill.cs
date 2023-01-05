using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HUDNamespace;
using LevelStageNamespace;

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

        private HashSet<DuckEnemies.StealingComponent> _locatedEnemies;
        private DuckEnemies.StealingComponent _locatedEnemy;
        private bool _canSteal = false;

        //variable for enemy to steal
        private DuckEnemies.StealingComponent _enemyToSteal = null;



        public void Steal(DuckEnemies.StealingComponent enemy)
        {
            _controller.ChangeState(PlayerState.GettingRobbed);

            if (_controller.GetState() == PlayerState.GettingRobbed)
            {
                _moveSkill.EnableInput(false, true);
                _enemyToSteal = enemy;

                //find the point between ducks
                var playerPos = _controller.gameObject.transform.position;
                var enemyPos = _enemyToSteal.gameObject.transform.position;
                var dir = (enemyPos - playerPos);
                var len = dir.magnitude;
                dir.Normalize();
                var middlePos = playerPos + dir * (len * 0.5f);

                var distance = 1.5f;
                var enemyDir = 0;
                var enemyAngle = 0.0f;
                Vector3 enemyFinalPos;
                //place the ducks face to face
                if (middlePos.x < playerPos.x)
                {
                    //player on the right
                    var pos = middlePos;
                    pos.x += distance;
                    _controller.gameObject.transform.position = pos;
                    //_controller.gameObject.transform.rotation = Quaternion.AngleAxis(90.0f,new Vector3(0,0,1));
                    _moveSkill.SetRotation(90.0f);

                    playerPos = pos;
                    //enemy on the left
                    pos = middlePos;
                    pos.x -= distance;
                    enemyDir = 0;
                    enemyFinalPos = pos;
                    enemyAngle = 90.0f;

                }
                else
                {
                    //player on the left
                    var pos = middlePos;
                    pos.x -= distance;
                    _controller.gameObject.transform.position = pos;
                    //_controller.gameObject.transform.rotation = Quaternion.AngleAxis(-90.0f, new Vector3(0, 0, 1));
                    _moveSkill.SetRotation(-90.0f);
                    playerPos = pos;
                    //enemy on the right
                    pos = middlePos;
                    pos.x += distance;
                    enemyDir = 1;
                    enemyFinalPos = pos;
                    enemyAngle = -90.0f;
                }

                //this function should allow the enemy to pass to passive steal state and to displace it in the correct position/direction
                // enemyDir: 0 left | 1 right
                BreadNamespace.BreadInMouthComponent breadContended = _eatSkill.GetCaughtBread();//_enemyToSteal.StartGettingRobbed(enemyFinalPos);//and also enemyDir for the sprite
                                                                                                 // _enemyToSteal.SetPosition(enemyFinalPos);
                                                                                                 // _enemyToSteal.SetRotation(enemyAngle);
                _controller.GetAnimalSoundController().PlayStealing(transform);

                //let's active the Quick Time Event.
                LevelStageNamespace.LakeDescriptionComponent lakeDescriptionComponent = (LevelStageNamespace.LakeDescriptionComponent)_controller.GetLake();
                if (lakeDescriptionComponent)
                {
                    _eatSkill.StopEating();
                    lakeDescriptionComponent.PlayerStartStealFromEnemy(_controller.gameObject, breadContended.gameObject, playerPos.x, playerPos.y + 3f);
                }
            }
        }


        public override void SetDescription(PlayerSkillDescriptionSO desc)
        {
            _description = desc;
            _stealDesc = (PlayerStealSkillDescriptionSO)_description;

            _coolDown = _stealDesc.CoolDown;
            _controller.GetHUDManager().UpdateSkillCooldown(HUDManager.textFields.stealCD, _stealCoolDownElapsedSeconds);

        }
        public override void applyPowerUp(PlayerSkillAttribute attrib, float value)
        {
            if (attrib == PlayerSkillAttribute.StealSkill_CoolDown)
            {
                _coolDown += value;
                _coolDown = Mathf.Max(_coolDown, 1);
            }
        }
        public DuckEnemies.StealingComponent FindClosestEnemy()
        {
            float _minDistance = 10000000;
            DuckEnemies.StealingComponent res = null;
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
            _locatedEnemies = new HashSet<DuckEnemies.StealingComponent>();
        }
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetButtonDown("StealButton") && 
                _locatedEnemy && _locatedEnemy.IsEating() &&
                !_enemyToSteal &&

                _stealCoolDownElapsedSeconds <= 0)
            {
                _controller.ChangeState(PlayerState.Stealing);

                if (_controller.GetState() == PlayerState.Stealing)
                {
                    _moveSkill.EnableInput(false, true);
                    _enemyToSteal = _locatedEnemy;
                    //_controller.GetAnimalSoundController().PlayStealing("Mallard", transform);
                  
                    //find the point between ducks
                    var playerPos = _controller.gameObject.transform.position;
                    var enemyPos = _enemyToSteal.gameObject.transform.position;
                    var dir = (enemyPos - playerPos);
                    var len = dir.magnitude;
                    dir.Normalize();
                    var middlePos = playerPos + dir * (len * 0.5f);

                    var distance = 1.5f;
                    var enemyDir = 0;
                    Vector3 enemyFinalPos;
                    //place the ducks face to face
                    if (middlePos.x < playerPos.x)
                    {
                        //player on the right
                        var pos = middlePos;
                        pos.x += distance;
                        _controller.gameObject.transform.position = pos;
                        //_controller.gameObject.transform.rotation = Quaternion.AngleAxis(90.0f,new Vector3(0,0,1));
                        _moveSkill.SetRotation(90.0f);

                        playerPos = pos;
                        //enemy on the left
                        pos = middlePos;
                        pos.x -= distance;
                        enemyDir = 0;
                        enemyFinalPos = pos;

                    }
                    else
                    {
                        //player on the left
                        var pos = middlePos;
                        pos.x -= distance;
                        _controller.gameObject.transform.position = pos;
                        //_controller.gameObject.transform.rotation = Quaternion.AngleAxis(-90.0f, new Vector3(0, 0, 1));
                        _moveSkill.SetRotation(-90.0f);
                        playerPos = pos;
                        //enemy on the right
                        pos = middlePos;
                        pos.x += distance;
                        enemyDir = 1;
                        enemyFinalPos = pos;
                    }

                    //this function should allow the enemy to pass to passive steal state and to displace it in the correct position/direction
                    // enemyDir: 0 left | 1 right
                    BreadNamespace.BreadInMouthComponent breadContended = _enemyToSteal.StartGettingRobbed(enemyFinalPos);//and also enemyDir for the sprite
                    _controller.GetAnimalSoundController().PlayStealing(transform);

                    //let's active the Quick Time Event.
                    LevelStageNamespace.LakeDescriptionComponent lakeDescriptionComponent = (LevelStageNamespace.LakeDescriptionComponent)_controller.GetLake();
                    if (lakeDescriptionComponent)
                    {
                        _controller.GetStatusView().SetInteractionActive(false, 3);
                        lakeDescriptionComponent.PlayerStartStealFromEnemy(_controller.gameObject, breadContended.gameObject, playerPos.x, playerPos.y + 3f);
                        _controller.GetAnimalSoundController().UnSwim();
                    }                 
                }
            }

            //===================================================================================================
            //===================================================================================================
            //===================================================================================================
            //===================================================================================================
            //===================================================================================================


            if (_controller.GetState() != PlayerState.Stealing && _stealCoolDownElapsedSeconds > 0)
            {
                _stealCoolDownElapsedSeconds -= Time.deltaTime;

                if (_stealCoolDownElapsedSeconds < 0)
                    _stealCoolDownElapsedSeconds = 0;
                _controller.GetHUDManager().UpdateSkillCooldown(HUDManager.textFields.stealCD, _stealCoolDownElapsedSeconds);
            }

        }


        public void NotifyFinishedQTE(GameObject breadForPlayer, GameObject breadForEnemy)
        {
            _controller.GetAnimalSoundController().SetIsInStealingState(false);

            if (((_controller.GetState() == PlayerState.Stealing && _stealCoolDownElapsedSeconds <= 0) 
                || _controller.GetState() == PlayerState.GettingRobbed) && 
                _enemyToSteal)
            {
                if (_controller.GetState() == PlayerState.Stealing)
                {
                    _stealCoolDownElapsedSeconds = _coolDown;
                    _controller.GetHUDManager().UpdateSkillCooldown(HUDManager.textFields.stealCD, _stealCoolDownElapsedSeconds);
                }

                if (breadForEnemy == null)
                {
                    _enemyToSteal.AssignBreadAfterRobbery(null);
                }
                else
                {
                    _enemyToSteal.AssignBreadAfterRobbery(breadForEnemy.GetComponent<BreadNamespace.BreadInMouthComponent>()); // this function notifies the enemy about the end of the steal and provides the resulting bread.
                }
                
                if (breadForPlayer == null)
                {
                    _eatSkill.SetCaughtBread(null);
                }
                else
                {
                    _eatSkill.SetCaughtBread(breadForPlayer.GetComponent<BreadNamespace.BreadInMouthComponent>());  // this function allows the player to change from steal to eat or normal state and provides the resulting bread.
                }

                bool toPlace = false;
                Vector3 newPos;
                (toPlace, newPos) = ((LakeDescriptionComponent) _controller.GetLake()).AdjustPlacement(_controller.GetPosition());

                if (toPlace)
                {
                    _moveSkill.MoveTo(newPos);
                }

                var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach(var e in enemies)
                {
                    e.GetComponent<DuckEnemies.ChasingComponent>().NotifyPlayerJustRobbed();
                }

            }
            _enemyToSteal = null;
        }



        void FixedUpdate()
        {
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            var enemyController = collision.gameObject.GetComponent<DuckEnemies.StealingComponent>();

            if (enemyController)
            {
                if (collision.gameObject.GetComponent<DuckEnemies.EatingComponent>().GetBreadInMouthComponent() &&  _controller.GetState() == PlayerState.Normal && _stealCoolDownElapsedSeconds <= 0)
                    _controller.GetStatusView().SetInteractionActive(true, 3);
                else
                    _controller.GetStatusView().SetInteractionActive(false, 3);

                _locatedEnemies.Add(enemyController);
                _locatedEnemy = FindClosestEnemy();
                
                if (_locatedEnemy)
                    Debug.Log("Enemy located");
            }



        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            var enemyController = collision.gameObject.GetComponent<DuckEnemies.StealingComponent>();

            if (enemyController)
            {
                if (collision.gameObject.GetComponent<DuckEnemies.EatingComponent>().GetBreadInMouthComponent() && _controller.GetState() == PlayerState.Normal && _stealCoolDownElapsedSeconds<=0)
                    _controller.GetStatusView().SetInteractionActive(true, 3);
                else
                    _controller.GetStatusView().SetInteractionActive(false, 3);
            }
        }


        private void OnTriggerExit2D(Collider2D collision)
        {
            var enemyController = collision.gameObject.GetComponent<DuckEnemies.StealingComponent>();

            if (enemyController)
            {
                _controller.GetStatusView().SetInteractionActive(false, 3);

                _locatedEnemies.Remove(enemyController);
                _locatedEnemy = FindClosestEnemy();
               
                if (!_locatedEnemy)
                    Debug.Log("Enemy missed");
            }
        }





    }
}
