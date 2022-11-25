using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QTEStealNamespace
{

    public class QTEButtonManagerComponent : MonoBehaviour
    {

        //informations regarding the button itself
        private EnumsQTESteal.KeysToPress _keyToPressEnum;
        private List<KeyCode> _keyToPressActual;
        private Sprite _sprite;

        //informations regarding the state of this button
        private EnumsQTESteal.QTEButtonState _state;

        private GameObject _spriteChild;


        //----------------------------------------------

        [SerializeField] private Sprite _upSprite;
        [SerializeField] private Sprite _downSprite;
        [SerializeField] private Sprite _leftSprite;
        [SerializeField] private Sprite _rightSprite;

        private Dictionary<EnumsQTESteal.KeysToPress, Sprite> _dictEnumKeySprite;
        private Dictionary<EnumsQTESteal.KeysToPress, List<KeyCode>> _dictEnumKeyActualKey;



        public void Awake()
        {
            //can't make them appear in the inspector, so I'll just initialize them here
            _dictEnumKeySprite = new Dictionary<EnumsQTESteal.KeysToPress, Sprite>() {
                { EnumsQTESteal.KeysToPress.Up, _upSprite },
                { EnumsQTESteal.KeysToPress.Down, _downSprite },
                { EnumsQTESteal.KeysToPress.Left, _leftSprite },
                { EnumsQTESteal.KeysToPress.Right, _rightSprite },
            };

            _dictEnumKeyActualKey = new Dictionary<EnumsQTESteal.KeysToPress, List<KeyCode>>() { 
                { EnumsQTESteal.KeysToPress.Up, new List<KeyCode>(){KeyCode.W, KeyCode.UpArrow} },
                { EnumsQTESteal.KeysToPress.Down, new List<KeyCode>(){KeyCode.S, KeyCode.DownArrow} },
                { EnumsQTESteal.KeysToPress.Left, new List<KeyCode>(){KeyCode.A, KeyCode.LeftArrow} },
                { EnumsQTESteal.KeysToPress.Right, new List<KeyCode>(){KeyCode.D, KeyCode.RightArrow} },
            };
        }



        //tolerance: when the center of the empty circle and the center of this button have a distance <= tolerance * max radius (of this button),
        //the button is in the InPressing state. Tolerance is basically a way to specify the radius of this circle
        public void Initialize(EnumsQTESteal.KeysToPress keyEnum, float tolerance = 1f)
        {
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
                /*_spriteChild.transform.localScale = new Vector3(_spriteChild.transform.localScale.x*tolerance,
                                                                _spriteChild.transform.localScale.y*tolerance,
                                                                _spriteChild.transform.localScale.z*tolerance)*/;

            }

            _sprite = _dictEnumKeySprite[_keyToPressEnum];
            transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = _sprite;

        }




        // Start is called before the first frame update
        void Start()
        {
            _state = EnumsQTESteal.QTEButtonState.BeforePress;
            _spriteChild = transform.Find("Sprite").gameObject;

        }



        // Update is called once per frame
        void Update()
        {
            for(int i = 0; i < _dictEnumKeyActualKey[_keyToPressEnum].Count; i++)
            {
                if (Input.GetKeyDown(_dictEnumKeyActualKey[_keyToPressEnum][i]))
                {
                    Debug.Log("HI! I AM " + _keyToPressEnum);
                }
            }
        }


        public void ChangeState(EnumsQTESteal.QTEButtonState newState)
        {
            if (!(newState == EnumsQTESteal.QTEButtonState.Failure && _state == EnumsQTESteal.QTEButtonState.Success))
            {
                _state = newState;
            }

        }
        

    }

}