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
        private PlayerBreadManager _breadManager = null;

       // [SerializeField] private BreadNamespace.BreadInWaterComponent _locatedBread = null;
        [SerializeField] private BreadNamespace.BreadInMouthComponent _caughtBread = null;

        private HashSet<BreadNamespace.BreadInWaterComponent> _locatedBreads;


        [SerializeField] private PowerUpsNamespace.PowerUpComponent _locatedPowerUp = null;



        private bool _hasBreadBeenFullyEaten = false;

        private IEnumerator _eatCoroutine;
        private bool _mustStopEating = false;

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
            //float _minDistance = 10000000;
            //BreadNamespace.BreadInWaterComponent res = null;

            //_locatedBreads.RemoveWhere(s => s == null);

            //foreach (var b in _locatedBreads)
            //{
            //    var dist = b.gameObject.transform.position - _controller.gameObject.transform.position;
            //    if (dist.magnitude < _minDistance)
            //    {
            //        _minDistance = dist.magnitude;
            //        res = b;
            //    }
            //}
            //return res;

            GameObject[] breads = GameObject.FindGameObjectsWithTag("FoodInWater");
            float minDistance = 10000000;
            BreadNamespace.BreadInWaterComponent bread = null;
            for (int i = 0; i < breads.Length; i++)
            {
                var dist = Vector3.Distance(breads[i].transform.position, _controller.gameObject.transform.position);
                if (dist <= breads[i].GetComponent<CircleCollider2D>().radius)
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
                StartCoroutine(EatCoroutine());
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
                // Destroy(_caughtBread.gameObject);
                _breadManager.ReleaseBread(_caughtBread);
                _caughtBread = null;
            }
        }


        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _moveSkill = GetComponent<PlayerMoveSkill>();
            _locatedBreads = new HashSet<BreadNamespace.BreadInWaterComponent>();
            _breadManager = GameObject.FindObjectOfType<PlayerBreadManager>();

            var duckTypeManager = GameObject.FindObjectOfType<DuckTypeManager>();
        }


        


        // Start is called before the first frame update
        void Start()
        {
        }



        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("EatButton") /*&& _locatedBread*/)

            {
                if (_locatedPowerUp)
                {
                    int spentDBP = 4;
                    List<PlayerSkillAttribute> listAttribs = new List<PlayerSkillAttribute>{ PlayerSkillAttribute.SpitSkill_MaxPower, PlayerSkillAttribute.SpitSkill_MaxRange };
                    List<float> listValues = new List<float> { 10,20 };

                    //call here buyPowerUp
                    (spentDBP, listAttribs, listValues) = _locatedPowerUp.BuyPowerUp(_controller.GetDigestedBreadPoints());



                    _controller.applyPowerUp(spentDBP, listAttribs, listValues);
                  //  if(spentDBP > 0) _locatedPowerUp = null;
                    return;
                }

                if (!_caughtBread)
                { 
                    var locatedBread = FindClosestBread();

                    if (locatedBread)
                    { 
                        _controller.ChangeState(PlayerState.Eating);

                        if (_controller.GetState() == PlayerState.Eating)
                            _moveSkill.EnableInput(true);

               
                        //take a piece of the located bread, or the entire located bread based on mouth size
                        _caughtBread = locatedBread.GenerateNewBreadInMouth(_mouthSize).GetComponent<BreadNamespace.BreadInMouthComponent>();
                        _eatCoroutine = EatCoroutine();
                        _mustStopEating = false;
                        StartCoroutine(_eatCoroutine);
                    }
                }
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

        public override void applyPowerUp(PlayerSkillAttribute attrib, float value)
        {
            if (attrib == PlayerSkillAttribute.EatSkill_ChewingRate)
            {
                _chewingRate += value;
            }
            else if (attrib == PlayerSkillAttribute.EatSkill_EatingSpeed)
            {
                _eatingSpeed += value;
            }
            else if(attrib == PlayerSkillAttribute.EatSkill_MouthSize)
            {
                _mouthSize += (int)value;
            }
        }


        private void FixedUpdate()
        {
            if (_controller.GetState() == PlayerState.Eating && _caughtBread)
            {
                _moveSkill.Move(_eatingSpeed);
                _caughtBread.Move(_controller.GetMouthTransform().position);
                //_chewingElapsedSeconds += Time.deltaTime;

                //if (/*_chewingElapsedSeconds >= _chewingRate &&*/ !_hasBreadBeenFullyEaten)
                //{
                //    // int a;
                //    // (a, _hasBreadBeenFullyEaten) = _caughtBread.SubtractBreadPoints(1);//eat a point each chewingRate seconds
                //    // _controller.AddBreadPoints(1);
                //    // _chewingElapsedSeconds = 0;

                //    StartCoroutine(MyCoroutine());
                //}

                //if (_chewingElapsedSeconds >= _chewingRate && !_hasBreadBeenFullyEaten)
                //{
                //    int a;
                //    (a, _hasBreadBeenFullyEaten) = _caughtBread.SubtractBreadPoints(1);//eat a point each chewingRate seconds
                //    _controller.AddBreadPoints(1);
                //    _chewingElapsedSeconds = 0;
                //}

            }
        }

        IEnumerator EatCoroutine()
        {
            int a;
            
            _mustStopEating = false;
           
            _controller.GetUICanvas().GetStatusView().SetVisible(true);
            _controller.GetUICanvas().GetStatusView().SetText("");
            _controller.GetUICanvas().GetStatusView().SetIcon(_eatDesc.EatingStatusIcon);
       

            Music.UniversalAudio.PlaySound("Duck 01", _controller.transform);

            while (!_hasBreadBeenFullyEaten && !_mustStopEating)
            {
                _controller.GetUICanvas().GetStatusView().SetText("" + _caughtBread.GetBreadPoints());
                yield return new WaitForSeconds(_chewingRate);
                if (!_mustStopEating)
                {
                    (a, _hasBreadBeenFullyEaten) = _caughtBread.SubtractBreadPoints(1);//eat a point each chewingRate seconds
                    // Debug.Log("Bread eaten before");
                    _controller.AddBreadPoints(1);
                   

                }
            }
            _controller.GetUICanvas().GetStatusView().SetText("" + _caughtBread.GetBreadPoints());
            _controller.GetUICanvas().GetStatusView().SetVisible(false);

            //Debug.Log("Bread eaten after");
            yield break;
        }

        public void StopEating()
        {
            _mustStopEating = true;
            StopCoroutine(_eatCoroutine);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            PowerUpsNamespace.PowerUpComponent powerup = null;
            powerup = ((collision.gameObject)?.transform.parent)?.gameObject.GetComponent<PowerUpsNamespace.PowerUpComponent>();
            if (powerup)
            {
                _locatedPowerUp = powerup;
            }






            //var powerup = collision.gameObject.transform.parent.gameObject.GetComponent<PowerUpsNamespace.PowerUpComponent>();
            


            //var breadController = collision.gameObject.GetComponent<BreadNamespace.BreadInWaterComponent>();
            //if (breadController)
            //{
            //    _locatedBreads.Add(breadController);
            //    _locatedBread = FindClosestBread();
            //}

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.gameObject.transform.parent) return;

            var powerup = collision.gameObject.transform.parent.gameObject.GetComponent<PowerUpsNamespace.PowerUpComponent>();
            if (powerup)
            {
                _locatedPowerUp = null;
            }

            //var breadController = collision.gameObject.GetComponent<BreadNamespace.BreadInWaterComponent>();

            //if (breadController)
            //{
            //    _locatedBreads.Remove(breadController);
            //    _locatedBread = FindClosestBread();
            //}
        }






        public int GetMouthSize()
        {
            return _mouthSize;
        }
    }
}
