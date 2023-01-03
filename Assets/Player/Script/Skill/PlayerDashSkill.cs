using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HUDNamespace;
using UnityEngine.SceneManagement;

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

        private GameObject _obstaclesGO;
        private List<GameObject> _obstaclesList;
        private List<GameObject> _enemies;

        public override void SetDescription(PlayerSkillDescriptionSO desc)
        {
            _description = desc;
            _dashDesc = (PlayerDashSkillDescriptionSO)_description;

            _maxSpeed = _dashDesc.MaxSpeed;
            _maxDuration = _dashDesc.MaxDuration;
            _coolDown = _dashDesc.CoolDown;

            _controller.GetHUDManager().UpdateSkillCooldown(HUDManager.textFields.dashCD, _dashCoolDownElapsedSeconds);
        }
        public override void applyPowerUp(PlayerSkillAttribute attrib, float value)
        {
            if (attrib == PlayerSkillAttribute.DashSkill_CoolDown)
            {
                _coolDown += value;
            }
            else if (attrib == PlayerSkillAttribute.DashSkill_MaxDuration)
            {
                _maxDuration += value;
            }
            else if (attrib == PlayerSkillAttribute.DashSkill_MaxSpeed)
            {
                _maxSpeed += (int)value;
            }

        }
        private void CheckData()
        {
            if (_controller.GetState() == PlayerState.Dashing)
            {
                _dashCoolDownElapsedSeconds = 0;
                _controller.GetHUDManager().UpdateSkillCooldown(HUDManager.textFields.dashCD, _dashCoolDownElapsedSeconds);
            }
            else
            {
                _dashElapsedSeconds = 0.0f;
                _dashCoolDownElapsedSeconds = _coolDown;
                _controller.GetHUDManager().UpdateSkillCooldown(HUDManager.textFields.dashCD, _dashCoolDownElapsedSeconds);
            }
        }
        //I'm so sorry but rn I don't want to do anything difficult to do something so simple
        private IEnumerator UpdateObstacles()
        {
            yield return new WaitForSeconds(0.05f);
            _obstaclesList.RemoveAll(x => !x.activeSelf);
            //In this way I only work with active obstacles
            yield return null;
        }

        private void GetAllObstaclesGameObjects(GameObject obstacleGO, List<GameObject> ret)
        {
            if (obstacleGO.transform.childCount == 0)
            {
                ret.Add(obstacleGO);
            }

            foreach (Transform child in obstacleGO.transform)
            {
                GetAllObstaclesGameObjects(child.gameObject, ret);
            }
        }
        
        // called second
        void OnSceneLoaded(Scene scene, LoadSceneMode  scene2)
        {
            _enemies.Clear();
            _obstaclesList.Clear();
            _obstaclesGO = GameObject.Find("Obstacles").transform.GetChild(0).gameObject;
            GetAllObstaclesGameObjects(_obstaclesGO, _obstaclesList);
            _obstaclesList.RemoveAll(x => !x.activeSelf);
            _enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        }
    

        void OnEnterDashingState()
        {
            _controller.ChangeState(PlayerState.Dashing);

            if (_controller.GetState() == PlayerState.Dashing)
            {
                _controller.GetAnimator().SetBool("Dash", true);


                _controller.GetAnimalSoundController().Fly();

                _moveSkill.EnableInput(false);

                
                _controller.GetStatusView().SetInteractionActive(true, 1);
                

                foreach (GameObject obstacle in _obstaclesList)
                {
                    Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), obstacle.GetComponent<CompositeCollider2D>(), true);
                }

                foreach (GameObject enemy in _enemies)
                {
                    Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), enemy.GetComponent<CircleCollider2D>(), true);
                }
            }

        }

        void OnExitDashingState(bool externalTerrain = false)
        {
            var p = _controller.GetPosition();

            if (!_controller.GetLake().Contains(p) && !externalTerrain) return;



            _controller.ChangeState(PlayerState.Normal);

            if (_controller.GetState() == PlayerState.Normal)
            {
                _controller.GetAnimator().SetBool("Dash", false);

                _controller.GetAnimalSoundController().UnFly();

                _controller.GetStatusView().SetInteractionActive(false, 1);
                _moveSkill.EnableInput(true);
                _dashElapsedSeconds = 0.0f;
                _dashCoolDownElapsedSeconds = _coolDown;
                _controller.GetHUDManager().UpdateSkillCooldown(HUDManager.textFields.dashCD, _dashCoolDownElapsedSeconds);

                foreach (GameObject obstacle in _obstaclesList)
                {
                    Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), obstacle.GetComponent<CompositeCollider2D>(), false);
                }

                foreach (GameObject enemy in _enemies)
                {
                    Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), enemy.GetComponent<CircleCollider2D>(), false);
                }
            }
            CheckData();
        }


        void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _moveSkill = GetComponent<PlayerMoveSkill>();
            _obstaclesList = new List<GameObject>();
            _enemies = new List<GameObject>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("DashButton") && _dashCoolDownElapsedSeconds <= 0)
            {
                if (_controller.GetState() == PlayerState.Dashing)
                {
                    _dashElapsedSeconds = _maxDuration;
                    OnExitDashingState();
                    
                }
                else
                {
                    //var p = _controller.GetPosition() + _moveSkill.GetDirection() * _noDashArea;
                    //if (_controller.GetLake().Contains(p))
                    //{
                    //    _controller.ChangeState(PlayerState.Dashing);
                    //}
                    OnEnterDashingState();
                }
            }

            if (_controller.GetState() == PlayerState.Dashing && _dashElapsedSeconds >= _maxDuration && _dashCoolDownElapsedSeconds <= 0)
            {
                OnExitDashingState();
            }
        }

        void FixedUpdate()
        {
            if (_controller.GetState() == PlayerState.Dashing && _dashElapsedSeconds < _maxDuration && _dashCoolDownElapsedSeconds <= 0)
            {
                _dashElapsedSeconds += Time.deltaTime;
                _moveSkill.Move(_maxSpeed, true);
            }
            else if (_controller.GetState() == PlayerState.Dashing && _dashElapsedSeconds >= _maxDuration)
            {
                _moveSkill.Move(_maxSpeed, true);
            }


            if (_controller.GetState() != PlayerState.Dashing && _dashCoolDownElapsedSeconds > 0)
            {
                _dashCoolDownElapsedSeconds -= Time.deltaTime;

                if (_dashCoolDownElapsedSeconds < 0)
                    _dashCoolDownElapsedSeconds = 0;
                _controller.GetHUDManager().UpdateSkillCooldown(HUDManager.textFields.dashCD, (int) _dashCoolDownElapsedSeconds);
            }
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (_controller.GetState() == PlayerState.Dashing)
            {
                OnExitDashingState(true);
            }
        }

        void OnCollisionExit2D(Collision2D other)
        {
        }
    }
}