using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HUDNamespace;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMoveSkill : PlayerSkill
    {
        //Move Skill Data
        //------------------------------------------
        [SerializeField] private float _speed = 0.0f;
        //------------------------------------------

        private PlayerController _controller = null;
        private PlayerMoveSkillDescriptionSO _moveDesc = null;
        private Camera _camera = null;

        private Rigidbody2D _rigidBody = null;
        private Vector3 _forwardAxis;
        private Vector3 _rightwardAxis;
        private Vector3 _finalDir;

        private bool _lockMovement = false;
        private bool _initLock = false;
        private Vector2 _lockInputAxis;
        private Vector2 _oldVelocity;
        private Vector3 _oldCursorPos;

        private bool _h = false;
        private bool _v = false;


        private bool _moveForward = false;
        private float _rotationMovement = 0.0f;

        private float _force = 0.0f;
        private float _overrideSpeed = 0.0f;


        private bool _enableInput = true;

        public override void SetDescription(PlayerSkillDescriptionSO desc)
        {
            _description = desc;
            _moveDesc = (PlayerMoveSkillDescriptionSO)_description;

            _speed = _moveDesc.Speed;
            _controller.GetHUDManager().ChangeText(HUDManager.textFields.speed, _speed);
        }

        public void EnableInput(bool enable, bool resetVelocity = false)
        {
            _enableInput = enable;
           
            if (resetVelocity)
                _rigidBody.velocity = new Vector2(0, 0);
        }

        public Vector3 GetDirection()
        {
            return _finalDir;
        }
        public float GetAngle()
        {
            return _rotationMovement;
        }
        public void SetRotation(float angle)
        {
            _rotationMovement = angle;


            //up
            if (angle >= 0 && angle < 22.5)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.1428571f);
            }
            //up-left
            else if (angle >= 22.5f && angle < 77.5f)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.2857143f);
            }
            //left
            else if (angle >= 77.5f && angle < 112.5)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.4285714f);
            }
            //down-left
            else if (angle >= 112.5 && angle < 157.5)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.5714286f);
            }
            //down
            else if (angle >= 157.5 && angle < 180)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.7142857f);
            }


            //down
            else if (angle >= -180 && angle < -157.5)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.7142857f);
            }
            //down-right
            else if (angle >= -157.5 && angle < -112.5)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.8571429f);
            }
            //right
            else if (angle >= -112.5 && angle < -77.5f)
            {
                _controller.GetAnimator().SetFloat("Blend", 1f);
            }
            //up-right
            else if (angle >= -77.5f && angle < -22.5f)
            {
                _controller.GetAnimator().SetFloat("Blend", 0);
            }
            //up
            else if (angle >= -22.5 && angle < 0)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.1428571f);
            }
        }
        public void MoveTo(Vector3 pos)
        {
            transform.position = pos;
        }
        public void Move(float speed, bool moveForward = false)
        {
            // if (_moveForward || moveForward)
            //{
            // var ax = new Vector3();
            // if (_enableInput)
            // {
            //     _finalDir = _forwardAxis;//_forwardAxis + _rightwardAxis;
            //    _finalDir.Normalize();

            //  var x = (_forwardAxis.x >= 0) ? Mathf.Ceil(_forwardAxis.x) : Mathf.Floor(_forwardAxis.x);
            //   var y = (_forwardAxis.y >= 0) ? Mathf.Ceil(_forwardAxis.y) : Mathf.Floor(_forwardAxis.y);

            //   ax = new Vector3(x,y, 0);
            //   ax.Normalize();
            // }


            // float angle = Mathf.Atan2(-_finalDir.x, _finalDir.y) * Mathf.Rad2Deg;
            // float angleInt = Mathf.Atan2(-ax.x, ax.y) * Mathf.Rad2Deg;



            ////up
            //if (angle >= 0 && angle < 22.5)
            //{
            //    angle = 0;
            //}
            ////up-left
            //else if (angle >= 22.5f && angle < 77.5f)
            //{
            //    angle = 45;
            //}
            ////left
            //else if (angle >= 77.5f && angle < 112.5)
            //{
            //    angle = 90;
            //}
            ////down-left
            //else if (angle >= 112.5 && angle < 157.5)
            //{
            //    angle = 135;
            //}
            ////down
            //else if (angle >= 157.5 && angle < 180)
            //{
            //    angle = 180;
            //}



            ////down
            //else if (angle >= -180 && angle < -157.5)
            //{
            //    angle = -180;
            //}
            ////down-right
            //else if (angle >= -157.5 && angle < -112.5)
            //{
            //    angle = -135;
            //}
            ////right
            //else if (angle >= -112.5 && angle < -77.5f)
            //{
            //    angle = -90;
            //}
            ////up-right
            //else if (angle >= -77.5f && angle < -22.5f)
            //{
            //    angle = -45;
            //}
            ////up
            //else if (angle >= -22.5 && angle < 0)
            //{
            //    angle = 0;
            //}



            // _rotationMovement = angle;

            if (_moveForward || moveForward)
            {
                if (_enableInput)
                {
                    _finalDir = _forwardAxis;// _forwardAxis + _rightwardAxis;
                    _finalDir.Normalize();
                }

                float angle = Mathf.Atan2(-_finalDir.x, _finalDir.y) * Mathf.Rad2Deg;


                //bool slowDown = false;

                //if (Mathf.Abs(angle + _rotationMovement) > 180.0f)
                //{
                //    slowDown = true;
                //    Debug.Log("slow down");
                //}


                //var a = (angle > 0 && angle <= 180.0f) ? angle : angle + 360.0f;
                //var r = (_rotationMovement > 0 && _rotationMovement <= 180.0f) ? _rotationMovement : _rotationMovement + 360.0f;


                //if (Mathf.Abs(a - r) > 180.0f)
                //{
                //    slowDown = true;
                //    Debug.Log("slow down");
                //}


                _rotationMovement = angle;
               

                //up
                if (angle >= 0 && angle < 22.5)
                {
                    _controller.GetAnimator().SetFloat("Blend", 0.1428571f);
                }
                //up-left
                else if (angle >= 22.5f && angle < 77.5f)
                {
                    _controller.GetAnimator().SetFloat("Blend", 0.2857143f);
                }
                //left
                else if (angle >= 77.5f && angle < 112.5)
                {
                    _controller.GetAnimator().SetFloat("Blend", 0.4285714f);
                }
                //down-left
                else if (angle >= 112.5 && angle < 157.5)
                {
                    _controller.GetAnimator().SetFloat("Blend", 0.5714286f);
                }
                //down
                else if (angle >= 157.5 && angle < 180)
                {
                    _controller.GetAnimator().SetFloat("Blend", 0.7142857f);
                }



                //down
                else if (angle >= -180 && angle < -157.5)
                {
                    _controller.GetAnimator().SetFloat("Blend", 0.7142857f);
                }
                //down-right
                else if (angle >= -157.5 && angle < -112.5)
                {
                    _controller.GetAnimator().SetFloat("Blend", 0.8571429f);
                }
                //right
                else if (angle >= -112.5 && angle < -77.5f)
                {
                    _controller.GetAnimator().SetFloat("Blend", 1f);
                }
                //up-right
                else if (angle >= -77.5f && angle < -22.5f)
                {
                    _controller.GetAnimator().SetFloat("Blend", 0);
                }
                //up
                else if (angle >= -22.5 && angle < 0)
                {
                    _controller.GetAnimator().SetFloat("Blend", 0.1428571f);
                }


                //if (slowDown)
                //    _force = speed * 10.8f;
                //else
                _force = speed * 1.5f;




                _rigidBody.AddForce(_finalDir * _force, ForceMode2D.Force);
                _rigidBody.velocity = Vector2.ClampMagnitude(_rigidBody.velocity, speed);
               // Debug.Log("_rigidBody.velocity " + _rigidBody.velocity);

            }
            else
            {
               // _rigidBody.velocity = Vector2.ClampMagnitude(_rigidBody.velocity * 0.9f, speed);

            }


            var mag = (_rigidBody.velocity).magnitude;


            //if (mag>= _speed*0.8f)
            //{
            //    if (_controller.GetState() == PlayerState.Normal)
            //        _controller.GetStatusView().SetInteractionActive(true, 5);
            //    else
            //        _controller.GetStatusView().SetInteractionActive(false, 5);
            //}
            //else
            //{
            //    _controller.GetStatusView().SetInteractionActive(false, 5);
            //}


            //if (mag < speed*0.8f)
            //    _oldVelocity = _rigidBody.velocity;
      

            //Debug.Log("_rigidBody.velocity " + _oldVelocity);

            // _rigidBody.SetRotation(Quaternion.AngleAxis(_rotationMovement, Vector3.forward));

            // MoveCamera();

        }
        public void SetOldVelocity(Vector2 oldVelocity)
        {
            if (oldVelocity.magnitude>0)
                _oldVelocity = oldVelocity;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_lockMovement) return;

            if (collision.name == "TriggerExitedCollider")
            {
                Debug.Log("TriggerExitedCollider Entered with vel: "+ _rigidBody.velocity);
                SetOldVelocity(_rigidBody.velocity);
            }
        }


        private void MoveCamera()
        {
            _camera.transform.position = _rigidBody.position;
            var cameraBounds = CameraUtility.OrthographicBounds(_camera);
            Bounds lakeBounds;

            if (_controller.GetLake())
                lakeBounds = _controller.GetLake().GetTerrainBounds();
            else
                lakeBounds = new Bounds();

            Vector3 newCamPos = transform.position;

            if (cameraBounds.min.x < lakeBounds.min.x) newCamPos.x += lakeBounds.min.x - cameraBounds.min.x;
            if (cameraBounds.max.x > lakeBounds.max.x) newCamPos.x -= cameraBounds.max.x - lakeBounds.max.x;
            if (cameraBounds.min.y < lakeBounds.min.y) newCamPos.y += lakeBounds.min.y - cameraBounds.min.y;
            if (cameraBounds.max.y > lakeBounds.max.y) newCamPos.y -= cameraBounds.max.y - lakeBounds.max.y;

            _camera.transform.position = newCamPos;
        }        



        public void RotateAnimator(float angle)
        {
            //up
            if (angle >= 0 && angle < 22.5)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.1428571f);
            }
            //up-left
            else if (angle >= 22.5f && angle < 77.5f)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.2857143f);
            }
            //left
            else if (angle >= 77.5f && angle < 112.5)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.4285714f);
            }
            //down-left
            else if (angle >= 112.5 && angle < 157.5)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.5714286f);
            }
            //down
            else if (angle >= 157.5 && angle < 180)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.7142857f);
            }



            //down
            else if (angle >= -180 && angle < -157.5)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.7142857f);
            }
            //down-right
            else if (angle >= -157.5 && angle < -112.5)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.8571429f);
            }
            //right
            else if (angle >= -112.5 && angle < -77.5f)
            {
                _controller.GetAnimator().SetFloat("Blend", 1f);
            }
            //up-right
            else if (angle >= -77.5f && angle < -22.5f)
            {
                _controller.GetAnimator().SetFloat("Blend", 0);
            }
            //up
            else if (angle >= -22.5 && angle < 0)
            {
                _controller.GetAnimator().SetFloat("Blend", 0.1428571f);
            }
        }
        
        
        public void Rotate()
        {
            if (_moveForward)
            {
                if (_enableInput)
                {

                    //var w = Screen.width;
                    //var h = Screen.height;

                    //var pos = Input.mousePosition;
                    //var centerPos = new Vector3(w / 2.0f, h / 2.0f, 0);
                    //var dirCursor = (pos - centerPos).normalized;

                    //if (_enableInput)
                    //    _finalDir = _forwardAxis;// _forwardAxis + _rightwardAxis;
                    //else
                    //    _finalDir = dirCursor;

                    _finalDir = _forwardAxis;
                    _finalDir.Normalize();
                }

                float angle = Mathf.Atan2(-_finalDir.x, _finalDir.y) * Mathf.Rad2Deg;
                _rotationMovement = angle;


                RotateAnimator(angle);
                ////up
                //if (angle >= 0 && angle < 22.5)
                //{
                //    _controller.GetAnimator().SetFloat("Blend", 0.1428571f);
                //}
                ////up-left
                //else if (angle >= 22.5f && angle < 77.5f)
                //{
                //    _controller.GetAnimator().SetFloat("Blend", 0.2857143f);
                //}
                ////left
                //else if (angle >= 77.5f && angle < 112.5)
                //{
                //    _controller.GetAnimator().SetFloat("Blend", 0.4285714f);
                //}
                ////down-left
                //else if (angle >= 112.5 && angle < 157.5)
                //{
                //    _controller.GetAnimator().SetFloat("Blend", 0.5714286f);
                //}
                ////down
                //else if (angle >= 157.5 && angle < 180)
                //{
                //    _controller.GetAnimator().SetFloat("Blend", 0.7142857f);
                //}



                ////down
                //else if (angle >= -180 && angle < -157.5)
                //{
                //    _controller.GetAnimator().SetFloat("Blend", 0.7142857f);
                //}
                ////down-right
                //else if (angle >= -157.5 && angle < -112.5)
                //{
                //    _controller.GetAnimator().SetFloat("Blend", 0.8571429f);
                //}
                ////right
                //else if (angle >= -112.5 && angle < -77.5f)
                //{
                //    _controller.GetAnimator().SetFloat("Blend", 1f);
                //}
                ////up-right
                //else if (angle >= -77.5f && angle < -22.5f)
                //{
                //    _controller.GetAnimator().SetFloat("Blend", 0);
                //}
                ////up
                //else if (angle >= -22.5 && angle < 0)
                //{
                //    _controller.GetAnimator().SetFloat("Blend", 0.1428571f);
                //}
            }
            else
            {
                if (_enableInput)
                { 
                    var pos = Input.mousePosition;
                    if (pos == _oldCursorPos) return;

                    var w = Screen.width;
                    var h = Screen.height;

                    var centerPos = _camera.WorldToScreenPoint(_rigidBody.position);//new Vector3(w / 2.0f, h / 2.0f, 0);
                    var dirCursor = (pos - centerPos).normalized;

                    _finalDir = dirCursor;
                    _finalDir.Normalize();


                    _oldCursorPos = pos;
                }

                float angle = Mathf.Atan2(-_finalDir.x, _finalDir.y) * Mathf.Rad2Deg;
                _rotationMovement = angle;


                RotateAnimator(angle);
            }

           // _rigidBody.SetRotation(Quaternion.AngleAxis(_rotationMovement, Vector3.forward));

           // MoveCamera();
        }
        public override void applyPowerUp(PlayerSkillAttribute attrib, float value) 
        {
            if (attrib == PlayerSkillAttribute.MoveSkill_Speed)
            {
                _speed += value;
                _controller.GetHUDManager().ChangeText(HUDManager.textFields.speed, _speed);
            }
        }

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _controller = GetComponent<PlayerController>();
            _camera = transform.parent.GetComponentInChildren<Camera>();

            var duckTypeManager = GameObject.FindObjectOfType<DuckTypeManager>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // called second
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //_rigidBody = GetComponent<Rigidbody2D>();
            //_rigidBody.velocity = _oldVelocity;
            _lockMovement = true;
            Debug.Log("Sceneloaded");
        }


        // Start is called before the first frame update
        void Start()
        {
            _controller.GetAnimator().SetFloat("Blend", 0.1428571f);
        //    _rigidBody.velocity = _oldVelocity;
            MoveCamera();
        }


        


        // Update is called once per frame
        void Update()
        {
            if (!_enableInput) return;
            if (_lockMovement)
            {
                _rigidBody = GetComponent<Rigidbody2D>();
                _rigidBody.velocity = _oldVelocity; 
                Debug.Log("Velocity set at: " + _rigidBody.velocity);
                _lockMovement = false;
               
            }

            //var h = Input.GetAxisRaw("Horizontal");
            //var v = Input.GetAxisRaw("Vertical");

            float v = 0;
            float h = 0;


            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) v = 1;
                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)  v = -1;  
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)  h = -1;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) h = 1;
            }

            var gamepad = Gamepad.current;
            if (gamepad != null)
            {
                Vector2 move = gamepad.leftStick.ReadValue();
                h += move.x;
                v += move.y;
            }






            _moveForward = false;
            _forwardAxis = new Vector3(h, v);

            if (_forwardAxis.x != 0 || _forwardAxis.y != 0)
            {
                _moveForward = true;
               // _controller.GetAnimalSoundController().Swim();
            }
            else
            {
               // _controller.GetAnimalSoundController().UnSwim();
            }
        }


        private void FixedUpdate()
        {
            // _oldVelocity = _rigidBody.velocity;


            var mag = (_rigidBody.velocity).magnitude/_speed;

            _controller.GetAnimalSoundController().Swim(mag);


            MoveCamera();
            // var screenPos = _camera.WorldToScreenPoint(_rigidBody.position) + new Vector3(-45f, 80f, 0);
            //Debug.Log("screen: " + screenPos);
            //screenPos.y += 80;
            //screenPos.x -= 20;

            //if (_lockMovement)
            //{
            //    _rigidBody = GetComponent<Rigidbody2D>();
            //    _rigidBody.velocity = new Vector2(0, 0);
            //    return;
            //}

            var pos = _rigidBody.position + new Vector2(0, 3);
            _controller.GetStatusView().SetPosition(pos);

            if (_controller.GetState() != PlayerState.Normal) return;

            Move(_speed/*, _moveForward*/);


        }
    }
}

