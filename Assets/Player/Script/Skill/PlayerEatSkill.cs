using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HUDNamespace;

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

            _controller.GetHUDManager().ChangeText(HUDManager.textFields.eatingSpeed, _eatingSpeed);
            _controller.GetHUDManager().ChangeText(HUDManager.textFields.chewingRate, _chewingRate);
            _controller.GetHUDManager().ChangeText(HUDManager.textFields.mouthSize, _mouthSize);
        }

        public BreadNamespace.BreadInWaterComponent FindClosestBread()
        {
            GameObject[] breads = GameObject.FindGameObjectsWithTag("FoodInWater");
            float minDistance = 10000000;
            BreadNamespace.BreadInWaterComponent bread = null;
            for (int i = 0; i < breads.Length; i++)
            {
                var dist = Vector3.Distance(breads[i].transform.position, _controller.gameObject.transform.position);
                if (dist <= breads[i].GetComponent<CircleCollider2D>().radius + 2.5f)
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
                {
                    _moveSkill.EnableInput(true);
                    _controller.GetStatusView().SetMiniStatusActive(true);
                   _controller.GetStatusView().SetInteractionActive(false, 0);
                    _controller.GetStatusView().SetVisible(true);
                    _controller.GetStatusView().SetText("");
                }
                _caughtBread = bread;
                StartCoroutine(EatCoroutine());
            }
            else
            {
                _caughtBread = null;
                _controller.ChangeState(PlayerState.Normal);

                if (_controller.GetState() == PlayerState.Normal)
                {
                    _moveSkill.EnableInput(true);
                    _controller.GetStatusView().SetMiniStatusActive(false);
                    _controller.GetStatusView().SetInteractionActive(false, 0);
                    _controller.GetStatusView().SetText("");
                    _controller.GetAnimalSoundController().UnEat();
                }
                _hasBreadBeenFullyEaten = false;
            }
        }


        public void ReleaseBread()
        {
            if (_controller.GetState() != PlayerState.Eating && _caughtBread)
            {
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

            _controller.GetHUDManager().ChangeBreadPointsCollectedText(_controller.GetBreadPoints());
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
                    _controller.GetHUDManager().ChangeDigestedBreadPointsCollectedText(_controller.GetDigestedBreadPoints());

                    if(spentDBP == 0)
                    {
                        Music.UniversalAudio.PlaySound("CannotBuy", transform);
                    }
                    else
                    {
                        Music.UniversalAudio.PlaySound("Bought", transform);
                    }
                    return;
                }

                if (!_caughtBread)
                { 
                    var locatedBread = FindClosestBread();

                    if (locatedBread)
                    { 
                        _controller.ChangeState(PlayerState.Eating);

                        if (_controller.GetState() == PlayerState.Eating)
                        { 
                            _moveSkill.EnableInput(true);
                            //take a piece of the located bread, or the entire located bread based on mouth size
                            _caughtBread = locatedBread.GenerateNewBreadInMouth(_mouthSize).GetComponent<BreadNamespace.BreadInMouthComponent>();
                            _eatCoroutine = EatCoroutine();
                            _mustStopEating = false;
                            StartCoroutine(_eatCoroutine);
                        }
                    }
                }
            }



            if (_controller.GetState() == PlayerState.Eating && _hasBreadBeenFullyEaten)
            {
                    _caughtBread = null;
                    _controller.ChangeState(PlayerState.Normal);
                    _hasBreadBeenFullyEaten = false;
            }
        }

        public override void applyPowerUp(PlayerSkillAttribute attrib, float value)
        {
            if (attrib == PlayerSkillAttribute.EatSkill_ChewingRate)
            {
                _chewingRate += value;
                _chewingRate = Mathf.Max(_chewingRate, 0.05f);
                _controller.GetHUDManager().ChangeText(HUDManager.textFields.chewingRate, _chewingRate);
            }
            else if (attrib == PlayerSkillAttribute.EatSkill_EatingSpeed)
            {
                _eatingSpeed += value;
                _controller.GetHUDManager().ChangeText(HUDManager.textFields.eatingSpeed, _eatingSpeed);
            }
            else if(attrib == PlayerSkillAttribute.EatSkill_MouthSize)
            {
                _mouthSize += (int)value;
                _controller.GetHUDManager().ChangeText(HUDManager.textFields.mouthSize, _mouthSize);
            }
        }


        private void FixedUpdate()
        {
            if (_controller.GetState() == PlayerState.Eating && _caughtBread)
            {
                _moveSkill.Move(_eatingSpeed);
                _caughtBread.Move(_controller.GetMouthTransform().position);
            }
        }

        IEnumerator EatCoroutine()
        {
            int a;
            
            _mustStopEating = false;

            _controller.GetStatusView().SetMiniStatusActive(true);
            _controller.GetStatusView().SetInteractionActive(false, 0);
            _controller.GetStatusView().SetVisible(true);
            _controller.GetStatusView().SetText("");
            _controller.GetStatusView().SetIcon(_eatDesc.EatingStatusIcon);

            _controller.GetAnimalSoundController().Eat();

            while (!_hasBreadBeenFullyEaten && !_mustStopEating)
            {
                _controller.GetStatusView().SetText("" + _caughtBread.GetBreadPoints() + " BP");
                yield return new WaitForSeconds(_chewingRate);
                if (!_mustStopEating)
                {
                    (a, _hasBreadBeenFullyEaten) = _caughtBread.SubtractBreadPoints(1);//eat a point each chewingRate seconds
                    _controller.AddBreadPoints(1);
                    _controller.GetHUDManager().ChangeBreadPointsCollectedText(_controller.GetBreadPoints());

                }
            }
            _controller.GetStatusView().SetText("" + _caughtBread.GetBreadPoints() + " BP");
            _controller.GetStatusView().SetMiniStatusActive(false);
            _controller.GetStatusView().SetInteractionActive(false, 0);
            _controller.GetAnimalSoundController().UnEat();
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
                if (_controller.GetState() == PlayerState.Normal)
                    _controller.GetStatusView().SetInteractionActive(true, 4);
                else
                    _controller.GetStatusView().SetInteractionActive(false, 4);

                _locatedPowerUp = powerup;
            }

            var breadController = collision.gameObject.GetComponent<BreadNamespace.BreadInWaterComponent>();
            if (breadController)
            {
                _controller.GetStatusView().SetExtraText("<color=\"yellow\"><size=8px>Bread Points: <color=\"white\"><size=8px>" + breadController.GetBreadPoints());

                if (_controller.GetState() == PlayerState.Normal)
                    _controller.GetStatusView().SetInteractionActive(true, 0);
                else
                    _controller.GetStatusView().SetInteractionActive(false, 0);
            }

        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            var breadController = collision.gameObject.GetComponent<BreadNamespace.BreadInWaterComponent>();
            if (breadController)
            {
                _controller.GetStatusView().SetExtraText("<size=8px><color=\"yellow\">Bread Points: <color=\"white\">" + breadController.GetBreadPoints()+ "</size>");
               
                if (_controller.GetState() == PlayerState.Normal)
                    _controller.GetStatusView().SetInteractionActive(true, 0);
                else
                    _controller.GetStatusView().SetInteractionActive(false, 0);
            }

        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            var breadController = collision.gameObject.GetComponent<BreadNamespace.BreadInWaterComponent>();

            if (breadController)
            {
                _controller.GetStatusView().SetExtraText("");
                _controller.GetStatusView().SetInteractionActive(false,0);
            }


            if (!collision.gameObject.transform.parent) return;

            var powerup = collision.gameObject.transform.parent.gameObject.GetComponent<PowerUpsNamespace.PowerUpComponent>();
            if (powerup)
            {
                _controller.GetStatusView().SetInteractionActive(false, 4);
                _locatedPowerUp = null;
            }
        }

        public int GetMouthSize()
        {
            return _mouthSize;
        }
    }
}
