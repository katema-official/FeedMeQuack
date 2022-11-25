using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace QTEStealNamespace
{

    public class QTEManagerComponment : MonoBehaviour
    {


        private int _numberOfButtonsToPress;

        

        

        [SerializeField] private float _yOffsetButtons = 20f;
        [SerializeField] private float _buttonSpeed = 3f;
        [SerializeField] private GameObject _buttonPrefab;



        private void Awake()
        {
            

        }

        // Start is called before the first frame update
        void Start()
        {
            Initialize(0, 0, 4);
        }

        // Update is called once per frame
        void Update()
        {

        }

        //method to initialize this quick time event.
        //nButtons = how many buttons must be generated for this quick time event
        public void Initialize(int x, int y, int nButtons, float yOffsetButtons = 20f, float buttonSpeed = 3f)
        {
            transform.position = new Vector3(x, y, 0);
            _numberOfButtonsToPress = nButtons;
            _yOffsetButtons = yOffsetButtons;
            _buttonSpeed = buttonSpeed;
            SpawnButtons();
        }

        private void SpawnButtons()
        {
            //first of all, get the "width" (radius, in cas it's circular) of the sprite that represents a button
            //To do so, we use the background of the button. It's always a circle whose xscale = yscale gives us the radius of the button.
            float radiusOfAButton = 30f;//_buttonPrefab.transform.Find("Background").gameObject.transform.localScale.x;
            Debug.Log("radius = " + radiusOfAButton);
            Vector3 currentPositionOfPoint = new Vector3(transform.position.x + _yOffsetButtons + radiusOfAButton, transform.position.y, 0);
            for(int i = 0; i < _numberOfButtonsToPress; i++)
            {
                GameObject buttonGO = Instantiate(_buttonPrefab);
                EnumsQTESteal.KeysToPress keyChosen = (EnumsQTESteal.KeysToPress)Random.Range(0, System.Enum.GetValues(typeof(EnumsQTESteal.KeysToPress)).Length);
                buttonGO.GetComponent<QTEButtonManagerComponent>().Initialize(keyChosen);
                buttonGO.transform.position = currentPositionOfPoint;
                Debug.Log("AAA");

            }

        }

    }

}
