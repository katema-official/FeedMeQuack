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

        private float _chewingElapsedSeconds = 0;

        private PlayerController _controller = null;
        private PlayerMoveSkill _moveSkill = null;
        private PlayerEatSkillDescriptionSO _eatDesc = null;

        [SerializeField] private BreadNamespace.BreadInWaterComponent _locatedBread = null;
        [SerializeField] private BreadNamespace.BreadInMouthComponent _caughtBread = null;

        private HashSet<BreadNamespace.BreadInWaterComponent> _locatedBreads;


        private bool _hasBreadBeenFullyEaten = false;

        public override void SetDescription(PlayerSkillDescriptionSO desc)
        {
            _description = desc;
            _eatDesc = (PlayerEatSkillDescriptionSO)_description;

            _eatingSpeed = _eatDesc.EatingSpeed;
            _chewingRate = _eatDesc.ChewingRate;
            _mouthSize = _eatDesc.MouthSize;
        }

        public BreadNamespace.BreadInWaterComponent FindClosestBread()
        {
            float _minDistance = 10000000;
            BreadNamespace.BreadInWaterComponent res = null;

            _locatedBreads.RemoveWhere(s => s == null);

            foreach (var b in _locatedBreads)
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

        public BreadNamespace.BreadInMouthComponent GetCaughtBread()
        {
            return _caughtBread;
        }
        public void SetCaughtBread(BreadNamespace.BreadInMouthComponent bread)
        {
            if (bread)
            {
                _controller.ChangeState(PlayerState.Eating);
                if (_controller.GetState() == PlayerState.Eating)
                    _moveSkill.EnableInput(true);
                _caughtBread = bread;
            }
            else
            {
                _caughtBread = null;
                _controller.ChangeState(PlayerState.Normal);
                if (_controller.GetState() == PlayerState.Normal)
                    _moveSkill.EnableInput(true);
                _hasBreadBeenFullyEaten = false;
            }
        }


        public void ReleaseBread()
        {
            if (_controller.GetState() != PlayerState.Eating && _caughtBread)
            {
                Destroy(_caughtBread.gameObject);
                _caughtBread = null;
            }
        }


        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _moveSkill = GetComponent<PlayerMoveSkill>();
            _locatedBreads = new HashSet<BreadNamespace.BreadInWaterComponent>();
            var duckTypeManager = GameObject.FindObjectOfType<DuckTypeManager>();
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("EatButton") && !_caughtBread && _locatedBread)
            {
                _controller.ChangeState(PlayerState.Eating);

                if (_controller.GetState() == PlayerState.Eating)
                    _moveSkill.EnableInput(true);

                //take a piece of the located bread, or the entire located bread based on mouth size
                _caughtBread = _locatedBread.GenerateNewBreadInMouth(_mouthSize).GetComponent<BreadNamespace.BreadInMouthComponent>();
            }


            if (_controller.GetState() == PlayerState.Eating && _hasBreadBeenFullyEaten)
            {
                //if (/*_caughtBread.GetPoints() <= 0*/ _hasBreadBeenFullyEaten)
                //{
                    //_controller.GetLake().DestroyBread(_caughtBread);// the removing of bread should be handled by lake or other manaager
                    _caughtBread = null;
                    _controller.ChangeState(PlayerState.Normal);
                    _hasBreadBeenFullyEaten = false;

                //}
            }
        }

        private void FixedUpdate()
        {
            if (_controller.GetState() == PlayerState.Eating && _caughtBread)
            {
                _moveSkill.Move(_eatingSpeed);
                _caughtBread.Move(_controller.GetMouthTransform().position);
                _chewingElapsedSeconds += Time.deltaTime;

                if (_chewingElapsedSeconds >= _chewingRate && !_hasBreadBeenFullyEaten)
                {
                    int a;
                    (a, _hasBreadBeenFullyEaten) = _caughtBread.SubtractBreadPoints(1);//eat a point each chewingRate seconds
                    _controller.AddBreadPoints(1);
                    _chewingElapsedSeconds = 0;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var breadController = collision.gameObject.GetComponent<BreadNamespace.BreadInWaterComponent>();
            if (breadController)
            {
                _locatedBreads.Add(breadController);
                _locatedBread = FindClosestBread();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var breadController = collision.gameObject.GetComponent<BreadNamespace.BreadInWaterComponent>();

            if (breadController)
            {
                _locatedBreads.Remove(breadController);
                _locatedBread = FindClosestBread();
            }
        }
    }
}
