using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QTEStealNamespace
{

    public class QTEButtonManagerComponent : MonoBehaviour
    {

        //informations regarding the button itself

        private QTEManagerComponment _QTEManager;
        private EnumsQTESteal.KeysToPress _keyToPressEnum;
        private Sprite _sprite;

        //informations regarding the state of this button
        [SerializeField] private EnumsQTESteal.QTEButtonState _state;

        private GameObject _spriteChild;
        private GameObject _backgroundChild;

        private float _backgroundScale = 2.5f;

        //----------------------------------------------

        [SerializeField] private Sprite _upSprite;
        [SerializeField] private Sprite _downSprite;
        [SerializeField] private Sprite _leftSprite;
        [SerializeField] private Sprite _rightSprite;


        private delegate float GetAxisInput(string axis);


        //private Dictionary<EnumsQTESteal.KeysToPress, Sprite> _dictEnumKeySprite;
        //private Dictionary<EnumsQTESteal.KeysToPress, GetAxisInput> _dictEnumKeyActualKey2;
        //private Dictionary<EnumsQTESteal.KeysToPress, List<KeyCode>> _dictEnumKeyActualKey;
        private Dictionary<EnumsQTESteal.KeysToPress, ButtonInfo> _dictEnumKeyDataKey;

        private bool _completedSuccessFeedback = false;
        private bool _completedFailureFeedback = false;


        struct ButtonInfo
        {
            public Sprite Sprite;
            public string Axis;
            public float Coefficient;

        }

        private GetAxisInput GetAxisMethod = Input.GetAxisRaw;
        private float _tresholdButtonPressed = 0.75f;   //for controller



        public void Awake()
        {
            _spriteChild = transform.Find("Sprite").gameObject;
            _backgroundChild = transform.Find("Sprite/Background").gameObject;
            _backgroundChild.transform.localScale = new Vector3(_backgroundChild.transform.localScale.x * _backgroundScale,
                                                                _backgroundChild.transform.localScale.y * _backgroundScale,
                                                                0);

            //can't make them appear in the inspector, so I'll just initialize them here
            /*_dictEnumKeySprite = new Dictionary<EnumsQTESteal.KeysToPress, Sprite>() {

                { EnumsQTESteal.KeysToPress.Up, _upSprite },
                { EnumsQTESteal.KeysToPress.Down, _downSprite },
                { EnumsQTESteal.KeysToPress.Left, _leftSprite },
                { EnumsQTESteal.KeysToPress.Right, _rightSprite },
            };*/

            //float x = Input.GetAxisRaw("Horizontal"); //-1 sx, 1 dx
            //float y = Input.GetAxisRaw("Vertical");   //-1 down, 1 up

            /*_dictEnumKeyActualKey = new Dictionary<EnumsQTESteal.KeysToPress, List<KeyCode>>() { 

                { EnumsQTESteal.KeysToPress.Up, new List<KeyCode>(){KeyCode.W, KeyCode.UpArrow} },
                { EnumsQTESteal.KeysToPress.Down, new List<KeyCode>(){KeyCode.S, KeyCode.DownArrow} },
                { EnumsQTESteal.KeysToPress.Left, new List<KeyCode>(){KeyCode.A, KeyCode.LeftArrow} },
                { EnumsQTESteal.KeysToPress.Right, new List<KeyCode>(){KeyCode.D, KeyCode.RightArrow} },
            };*/

            /*_dictEnumKeyActualKey2 = new Dictionary<EnumsQTESteal.KeysToPress, GetAxisInput>() {
                { EnumsQTESteal.KeysToPress.Up, ver },
                { EnumsQTESteal.KeysToPress.Down, ver },
                { EnumsQTESteal.KeysToPress.Left, hor },
                { EnumsQTESteal.KeysToPress.Right, hor },
            };*/

            ButtonInfo upInfo;
            upInfo.Sprite = _upSprite;
            upInfo.Axis = "Vertical";
            upInfo.Coefficient = 1f;

            ButtonInfo downInfo;
            downInfo.Sprite = _downSprite;
            downInfo.Axis = "Vertical";
            downInfo.Coefficient = -1f;

            ButtonInfo rightInfo;
            rightInfo.Sprite = _rightSprite;
            rightInfo.Axis = "Horizontal";
            rightInfo.Coefficient = 1f;

            ButtonInfo leftInfo;
            leftInfo.Sprite = _leftSprite;
            leftInfo.Axis = "Horizontal";
            leftInfo.Coefficient = -1f;

            _dictEnumKeyDataKey = new Dictionary<EnumsQTESteal.KeysToPress, ButtonInfo>()
            {
                { EnumsQTESteal.KeysToPress.Up, upInfo },
                { EnumsQTESteal.KeysToPress.Down, downInfo},
                { EnumsQTESteal.KeysToPress.Left, leftInfo},
                { EnumsQTESteal.KeysToPress.Right, rightInfo},
            };

            _state = EnumsQTESteal.QTEButtonState.BeforePress;
        }



        //tolerance: when the center of the empty circle and the center of this button have a distance <= tolerance * max radius (of this button),
        //the button is in the InPressing state. Tolerance is basically a way to specify the radius of this circle
        public void Initialize(QTEManagerComponment QTEManager, EnumsQTESteal.KeysToPress keyEnum, float tolerance = 1f)
        {
            _QTEManager = QTEManager;
            _keyToPressEnum = keyEnum;
            if(tolerance < 0f || tolerance > 1f)
            {
                //we ignore the value passed
                //tolerance = transform.Find("Sprite").GetComponent<CircleCollider2D>().radius;
            }
            else
            {
                //we multiply it for the radius of the button
                transform.Find("Sprite").GetComponent<CircleCollider2D>().radius *= tolerance;
            }

            //_sprite = _dictEnumKeySprite[_keyToPressEnum];
            _sprite = _dictEnumKeyDataKey[_keyToPressEnum].Sprite;

            transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = _sprite;

        }




        // Start is called before the first frame update
        void Start()
        {
            _state = EnumsQTESteal.QTEButtonState.BeforePress;
            _spriteChild = transform.Find("Sprite").gameObject;

        }

        private bool _chanceTaken = false;

        // Update is called once per frame
        void Update()
        {

            switch (_state)
            {
                case EnumsQTESteal.QTEButtonState.BeforePress:

                    break;
                case EnumsQTESteal.QTEButtonState.InPressing:

                    if (!_chanceTaken)
                    {
                        if (Input.anyKeyDown || Mathf.Abs(GetAxisMethod("Horizontal")) > _tresholdButtonPressed || Mathf.Abs(GetAxisMethod("Vertical")) > _tresholdButtonPressed)
                        {
                            bool correct = false;

                            if ((GetAxisMethod(_dictEnumKeyDataKey[_keyToPressEnum].Axis) * _dictEnumKeyDataKey[_keyToPressEnum].Coefficient)
                                >= _tresholdButtonPressed)
                            {
                                correct = true;
                            }

                            _chanceTaken = true;

                            /*for (int i = 0; i < _dictEnumKeyActualKey[_keyToPressEnum].Count; i++)
                            {
                                if (Input.GetKeyDown(_dictEnumKeyActualKey[_keyToPressEnum][i]))
                                {
                                    correct = true;
                                }
                            }*/

                            if (correct)
                            {
                                ChangeState(EnumsQTESteal.QTEButtonState.Success);
                            }
                            else
                            {
                                ChangeState(EnumsQTESteal.QTEButtonState.Failure);
                            }
                        }
                    }
                    break;
                case EnumsQTESteal.QTEButtonState.Success:
                    if (_completedSuccessFeedback == false) ShowSuccess();
                    break;
                case EnumsQTESteal.QTEButtonState.Failure:
                    if (_completedFailureFeedback == false) ShowFailure();
                    break;
                case EnumsQTESteal.QTEButtonState.Final:
                    _QTEManager.NotifyButton();
                    Destroy(this.gameObject);
                    break;
                default:
                    break;
            }

            
        }


        public void ChangeState(EnumsQTESteal.QTEButtonState newState)
        {
            if (!(newState == EnumsQTESteal.QTEButtonState.Failure && _state == EnumsQTESteal.QTEButtonState.Success))
            {
                _state = newState;
            }

        }






        public float GetBackgroundScale()
        {
            return _backgroundScale;
        }

        public void SetVelocity(float vx, float vy)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector3(vx, vy, 0);
        }



        //#############################################################################################################################
        //############################################# SUCCESS AND FAILURE MANAGEMENT ################################################
        //#############################################################################################################################


        float _fadeOutDuration = 0.25f;    //how much does the fade out animation last


        private void ShowSuccess()
        {
            _QTEManager.NotifyButtonSuccess();
            _backgroundChild.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
            _completedSuccessFeedback = true;
            SetVelocity(0f, 0f);
            StartCoroutine(ShowSuccessCoroutine());
           
        }
        private IEnumerator ShowSuccessCoroutine()
        {
            float enlargeFactor = 0.015f;    //how much it should enlarge each time this function is called
            
            for(float t = 0f; t < 1f; t += Time.deltaTime / _fadeOutDuration)
            {

                transform.localScale = new Vector3(transform.localScale.x + enlargeFactor, transform.localScale.y + enlargeFactor, 0);

                Color cSprite = _spriteChild.GetComponent<SpriteRenderer>().color;
                cSprite.a = Mathf.Lerp(1f, 0f, t);
                _spriteChild.GetComponent<SpriteRenderer>().color = cSprite;

                Color cBackground = _backgroundChild.GetComponent<SpriteRenderer>().color;
                cBackground.a = Mathf.Lerp(1f, 0f, t);
                _backgroundChild.GetComponent<SpriteRenderer>().color = cBackground;
                yield return null;

            }

            ChangeState(EnumsQTESteal.QTEButtonState.Final);
            yield return null;
        }


        private void ShowFailure()
        {
            _backgroundChild.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            _completedFailureFeedback = true;
            StartCoroutine(ShowFailureCoroutine());
        }

        private IEnumerator ShowFailureCoroutine()
        {
            for (float t = 0f; t < 1f; t += Time.deltaTime / _fadeOutDuration)
            {

                Color cSprite = _spriteChild.GetComponent<SpriteRenderer>().color;
                cSprite.a = Mathf.Lerp(1f, 0f, t);
                _spriteChild.GetComponent<SpriteRenderer>().color = cSprite;

                Color cBackground = _backgroundChild.GetComponent<SpriteRenderer>().color;
                cBackground.a = Mathf.Lerp(1f, 0f, t);
                _backgroundChild.GetComponent<SpriteRenderer>().color = cBackground;
                yield return null;

            }

            ChangeState(EnumsQTESteal.QTEButtonState.Final);
            yield return null;

        }

    }

}