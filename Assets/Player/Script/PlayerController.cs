using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HUDNamespace;
using Music;

namespace Player
{
    public class PlayerController : MonoBehaviour
    { 
        [SerializeField] private float _breadPoints = 0.0f;
        [SerializeField] private float _digestedBreadPoints = 0.0f;

        private Rigidbody2D _rigidBody = null;
        private Camera _camera = null;
        private Transform _mouth = null;


        private LevelStageNamespace.LakeShopDescriptionComponent _currentLake = null;

        [SerializeField] private PlayerState _state = PlayerState.Normal;


        private StatusViewController _statusView = null;


        private PlayerDuckDescriptionSO _description = null;
        private List<PlayerSkill> _skills;
        private Animator _animator;

        private HUDManager _hudManager;
        private AnimalSoundController _animalSoundController;

        public Transform GetMouthTransform()
        {
            return _mouth;
        }
        public PlayerState GetState()
        {
            return _state;
        }
        public LevelStageNamespace.LakeShopDescriptionComponent GetLake()
        {
            _currentLake = GameObject.Find("WholeLake").GetComponent<LevelStageNamespace.LakeShopDescriptionComponent>();
            return _currentLake;
        }
        public Vector3 GetPosition()
        {
            return _rigidBody.position;
        }
        public void AddBreadPoints(float points)
        {
            _breadPoints += points;

            Debug.Log("Current player bread points: " + _breadPoints);
        }
        public int GetBreadPoints()
        {
            return (int) _breadPoints;
        }
        public Bounds GetCameraBounds()
        {
            return CameraUtility.OrthographicBounds(_camera);
        }
        public Animator GetAnimator()
        {
            return _animator;
        }

        public StatusViewController GetStatusView()
        {
            return _statusView;
        }

        public void NotifyStageCompleted(int points)
        {
            _digestedBreadPoints += (_breadPoints - points);
            _breadPoints = 0;
            GetHUDManager().ChangeBreadPointsCollectedText(GetBreadPoints());
            GetHUDManager().ChangeDigestedBreadPointsCollectedText(GetDigestedBreadPoints());
        }

        public void applyPowerUp(int spentDBP,List<PlayerSkillAttribute> listSkillAttribs, List<float> listValues)
        {
            if (spentDBP == 0) return;
            _digestedBreadPoints -= spentDBP;
            

            for(var i=0; i< listSkillAttribs.Count; i++)
            {
                var a = listSkillAttribs[i];
                var v = listValues[i];
                var skillName = "";

                if (a == PlayerSkillAttribute.MoveSkill_Speed)
                    skillName ="MoveSkill";
                else if (a == PlayerSkillAttribute.EatSkill_EatingSpeed ||
                    a == PlayerSkillAttribute.EatSkill_ChewingRate ||
                   a == PlayerSkillAttribute.EatSkill_MouthSize)
                    skillName = "EatSkill";
                else if (a == PlayerSkillAttribute.DashSkill_CoolDown ||
                   a == PlayerSkillAttribute.DashSkill_MaxDuration ||
                  a == PlayerSkillAttribute.DashSkill_MaxSpeed)
                    skillName = "DashSkill";
                else if (a == PlayerSkillAttribute.SpitSkill_ChargeSpeed ||
                    a == PlayerSkillAttribute.SpitSkill_CoolDown ||
                   a == PlayerSkillAttribute.SpitSkill_MaxPower ||
                    a == PlayerSkillAttribute.SpitSkill_MaxRange ||
                    a == PlayerSkillAttribute.SpitSkill_CarryingSpeed ) 
                    skillName = "SpitSkill";
                else if (a == PlayerSkillAttribute.StealSkill_CoolDown) 
                    skillName = "StealSkill";

                var s = _getPlayerSkillByName(skillName);
                if (s) s.applyPowerUp(a, v);
            }

        }
        public void ChangeState(PlayerState newState)
        {
            if (_state == PlayerState.Normal)
            {
                if (newState == PlayerState.Dashing ||
                    newState == PlayerState.Eating ||
                    newState == PlayerState.Carrying ||
                    newState == PlayerState.Stealing)
                {
                    _state = newState;
                }
            }
            else if (_state == PlayerState.Dashing)
            {
                if (newState == PlayerState.Normal)
                {
                    _state = newState;
                }
            }
            else if (_state == PlayerState.Eating)
            {
                if (/*newState == PlayerState.Spitting ||*/
                    newState == PlayerState.Normal ||
                     newState == PlayerState.GettingRobbed)
                {
                    _state = newState;
                }
            }
            else if (_state == PlayerState.Spitting)
            {
                if (newState == PlayerState.Normal)
                {
                    _state = newState;
                }
            }
            else if (_state == PlayerState.Stealing)
            {
                if (newState == PlayerState.Eating || 
                    newState == PlayerState.Normal)
                {
                    _state = newState;
                }
            }
            else if (_state == PlayerState.GettingRobbed)
            {
                if (newState == PlayerState.Eating ||
                    newState == PlayerState.Normal)
                {
                    _state = newState;
                }
            }
            else if (_state == PlayerState.Carrying)
            {
                if (newState == PlayerState.Spitting)
                {
                    _state = newState;
                }
            }
        }

        private PlayerSkill _getPlayerSkillByName(string name)
        {
            foreach (var s in _skills)
            {
                if (s.GetDescription().Type.Name == name)
                    return s;
            }

            return null;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.transform.parent.gameObject);

            _rigidBody = GetComponent<Rigidbody2D>();
            if (_rigidBody)
            {
                _rigidBody.gravityScale = 0f;
                _rigidBody.freezeRotation = true;
            }

            var duckTypeManager = GameObject.FindObjectOfType<DuckTypeManager>();
            _description = duckTypeManager.getTypeFromName("Mallard");

            _camera = transform.parent.GetComponentInChildren<Camera>();
            _mouth = transform.Find("Mouth");
            _statusView = transform.parent.Find("UI/StatusView").GetComponent<StatusViewController>();
            _hudManager = GameObject.FindObjectOfType<HUDManager>();
            _animalSoundController = GetComponent<AnimalSoundController>();

            _skills = new List<PlayerSkill>();
            foreach (var s in _description.Skills)
            {
                if (!s.EnabledByDefault) continue;
                var skill = SkillUtility.CreateSkillFromDescription(s,gameObject);
                _skills.Add(skill);
            }


            _animator= GetComponent<Animator>();

        }

        // Start is called before the first frame update
        private void Start()
        {
            _animalSoundController.SetIsEnemy(false);
        }

        // Update is called once per frame
        private void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (transform.parent.Find("UI/ExitMenu").gameObject.activeSelf)
                {
                    #if UNITY_EDITOR
                                        // Application.Quit() does not work in the editor so
                                        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                                        UnityEditor.EditorApplication.isPlaying = false;
                    #else
                             Application.Quit();
                    #endif
                }

                transform.parent.Find("UI/ExitMenu").gameObject.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (transform.parent.Find("UI/ExitMenu").gameObject.activeSelf)
                {
                    transform.parent.Find("UI/ExitMenu").gameObject.SetActive(false);
                }
            }*/

        }

        private void FixedUpdate()
        {
        }

        public int GetDigestedBreadPoints()
        {
            return (int) _digestedBreadPoints;

        }

        public HUDManager GetHUDManager()
        {
            return _hudManager;
        }

        public AnimalSoundController GetAnimalSoundController()
        {
            return _animalSoundController;
        }
    }
}
