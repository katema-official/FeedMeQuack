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

        private BreadController _locatedBread = null;
        private BreadController _catchedBread = null;

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
            _controller = GetComponent<PlayerController>();
            _moveSkill = GetComponent<PlayerMoveSkill>();

            var duckTypeManager = GameObject.FindObjectOfType<DuckTypeManager>();
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && !_catchedBread && _locatedBread)
            {

                _controller.ChangeState(PlayerState.Eating);

                if (_controller.GetState() == PlayerState.Eating)
                    _moveSkill.EnableInput(true);


                //take a piece of the located bread, or the entire located bread based on mouth size
                if (_locatedBread.GetPoints() > _mouthSize)
                {
                    _locatedBread.EatPoints(_mouthSize);
                    var l = _controller.GetLake();
                    _catchedBread = l.GenerateNewBread();
                    _catchedBread.SetPoints(_mouthSize);
                }
                else
                {
                    _catchedBread = _locatedBread;
                }
            }


            if (_controller.GetState() == PlayerState.Eating && _catchedBread)
            {
                if (_catchedBread.GetPoints() <= 0)
                {
                    _controller.GetLake().DestroyBread(_catchedBread);// the removing of bread should be handled by lake or other manaager
                    _catchedBread = null;
                    _controller.ChangeState(PlayerState.Normal);

                }
            }

            //PlayerUtility.Move(_eatingSpeed, _forwardAxis, _rightwardAxis, _rigidBody, _moveForward, ref _rotationMovement);
        }

        private void FixedUpdate()
        {
            if (_controller.GetState() == PlayerState.Eating && _catchedBread)
            {
                _moveSkill.Move(_eatingSpeed);
                _catchedBread.Move(_controller.GetMouthTransform().position);
                _catchedBread.EatPoints(_chewingRate * Time.deltaTime);//eat chewingRate points each second
            }
        }








        private void OnTriggerEnter2D(Collider2D collision)
        {
            var breadController = collision.gameObject.GetComponentInParent<BreadController>();

            if (breadController)
            {
                _locatedBread = breadController;
                Debug.Log("Bread located");
            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var breadController = collision.gameObject.GetComponentInParent<BreadController>();

            if (breadController)
            {
                _locatedBread = null;
                Debug.Log("Bread missed");
            }
        }
    }
}
