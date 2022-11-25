using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace QTEStealNamespace
{

    public class QuickTimeEventManagerComponment : MonoBehaviour
    {


        private int _numberOfButtonsToPress;

        [SerializeField] private Sprite _upSprite;
        [SerializeField] private Sprite _downSprite;
        [SerializeField] private Sprite _leftSprite;
        [SerializeField] private Sprite _rightSprite;

        private Dictionary<EnumsQTESteal.KeysToPress, Sprite> _dictEnumKeySprite;
        private Dictionary<EnumsQTESteal.KeysToPress, List<KeyCode>> _dictEnumKeyActualKey;



        private void Awake()
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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //method to initialize this quick time event.
        //nButtons = how many buttons must be generated for this quick time event
        public void Initialize(int nButtons)
        {
            _numberOfButtonsToPress = nButtons;
        }



    }

}
