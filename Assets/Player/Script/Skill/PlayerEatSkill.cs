using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerEatSkill : PlayerSkill
    {
        //Eat Skill Data
        //------------------------------------------
        [SerializeField] private float _eatingSpeed = 0.0f;
        [SerializeField] private float _chewingRate = 0.0f;
        [SerializeField] private int _mouthSize = 0;
        //------------------------------------------

        private PlayerController _controller = null;
        private PlayerMoveSkill _moveSkill = null;
        private PlayerEatSkillDescriptionSO _eatDesc = null;


        private Rigidbody2D _rigidBody = null;
        private Vector3 _forwardAxis;
        private Vector3 _rightwardAxis;

        private bool _moveForward = false;
        private float _rotationMovement = 0.0f;

        private float _force = 0.0f;


        public override void SetDescription(PlayerSkillDescriptionSO desc)
        {
            _description = desc;
            _eatDesc = (PlayerEatSkillDescriptionSO)_description;

            _eatingSpeed = _eatDesc.EatingSpeed;
            _chewingRate = _eatDesc.ChewingRate;
            _mouthSize = _eatDesc.MouthSize;
        }
        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _controller = GetComponent<PlayerController>();
            _moveSkill = GetComponent<PlayerMoveSkill>();

            var duckTypeManager = GameObject.FindObjectOfType<DuckTypeManager>();
        }
        // Start is called before the first frame update
        void Start()
        {
            if (_controller.GetState() != PlayerState.Eating) return;

            //PlayerUtility.GetMovementAxis(ref _moveForward, ref _forwardAxis, ref _rightwardAxis);


        }

        // Update is called once per frame
        void Update()
        {
            if (_controller.GetState() != PlayerState.Eating) return;





            //PlayerUtility.Move(_eatingSpeed, _forwardAxis, _rightwardAxis, _rigidBody, _moveForward, ref _rotationMovement);
        }

        private void FixedUpdate()
        {
            if (_controller.GetState() != PlayerState.Eating) return;

            //PlayerUtility.Move(_speed, _forwardAxis, _rightwardAxis, _rigidBody, _moveForward, ref _rotationMovement);
            _moveSkill.Move(_eatingSpeed);
        }
    }
}
