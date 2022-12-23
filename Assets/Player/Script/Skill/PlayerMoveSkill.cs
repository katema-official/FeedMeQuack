using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HUDNamespace;

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

            }
            else
            {
               // _rigidBody.velocity = Vector2.ClampMagnitude(_rigidBody.velocity * 0.9f, speed);

            }
        

            // _rigidBody.SetRotation(Quaternion.AngleAxis(_rotationMovement, Vector3.forward));

            // MoveCamera();

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
        
        
        public void Rotate()
        {
            if (_moveForward)
            {
                if (_enableInput)
                {
                    _finalDir = _forwardAxis;// _forwardAxis + _rightwardAxis;
                    _finalDir.Normalize();
                }

                float angle = Mathf.Atan2(-_finalDir.x, _finalDir.y) * Mathf.Rad2Deg;
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
            _lockMovement = true;
        }



        // Start is called before the first frame update
        void Start()
        {
            _controller.GetAnimator().SetFloat("Blend", 0.1428571f);
            MoveCamera();
        }

        // Update is called once per frame
        void Update()
        {
            if (!_enableInput) return;

            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");

            if (_lockMovement)
            {
                if ((h == 0 && v == 0) && (_forwardAxis.x != 0 || _forwardAxis.y != 0))
                    return;
                else
                    _lockMovement = false;
            }  

            _moveForward = false;
           // _forwardAxis = new Vector3(0, 0);

            _forwardAxis = new Vector3(h,v);

            if (h!=0 || v!=0)
                _moveForward = true;
        }


        private void FixedUpdate()
        {
            MoveCamera();
            var screenPos = _camera.WorldToScreenPoint(_rigidBody.position) + new Vector3(-45f, 80f, 0);
            //Debug.Log("screen: " + screenPos);
            //screenPos.y += 80;
            //screenPos.x -= 20;
            _controller.GetUICanvas().GetStatusView().SetPosition(screenPos);

            if (_controller.GetState() != PlayerState.Normal) return;

            Move(_speed/*, _moveForward*/);


        }
    }
}

