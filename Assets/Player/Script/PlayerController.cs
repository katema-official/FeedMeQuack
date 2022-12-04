using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private PlayerState _state = PlayerState.Normal;

        private PlayerDuckDescriptionSO _description = null;
        private List<PlayerSkill> _skills;
        private Animator _animator;

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

        public Animator GetAnimator()
        {
            return _animator;
        }

        public void NotifyStageCompleted(int points)
        {
            _digestedBreadPoints += (_breadPoints - points);
            _breadPoints = 0;
        }

        public void ChangeState(PlayerState newState)
        {
            if (_state == PlayerState.Normal)
            {
                if (newState == PlayerState.Dashing ||
                    newState == PlayerState.Eating ||
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
                if (newState == PlayerState.Spitting ||
                    newState == PlayerState.Normal ||
                    newState == PlayerState.Stealing)
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
            
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void FixedUpdate()
        {
        }
    }
}
