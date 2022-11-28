using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace QTEStealNamespace
{

    public class QTEManagerComponment : MonoBehaviour
    {


        private int _numberOfButtonsToPress;
        private int _numberOfButtonsEnded;
        private int _numberOfButtonsCorrectlyPressed;
        

        

        [SerializeField] private float _xOffsetButtons = 10f;
        [SerializeField] private int _maxNumberOfEmptySpaces = 0; //sometimes we can introduce a gap between two buttons, large as much as a button. How many there can be, at max?
        [SerializeField] private float _buttonSpeed = 10f;
        [SerializeField] private GameObject _buttonPrefab;



        private void Awake()
        {
            

        }

        // Start is called before the first frame update
        void Start()
        {
            //Initialize(0, 0, 40, 15, 1, 8);
        }

        // Update is called once per frame
        void Update()
        {

        }

        //method to initialize this quick time event.
        //nButtons = how many buttons must be generated for this quick time event
        public void Initialize(float x, float y, int nButtons, float xOffsetButtons = 10f, int maxNumberOfEmptySpaces = 0, float buttonSpeed = 10f)
        {
            transform.position = new Vector3(x, y, 0);
            _numberOfButtonsToPress = nButtons;
            _xOffsetButtons = xOffsetButtons;
            _maxNumberOfEmptySpaces = maxNumberOfEmptySpaces;
            _buttonSpeed = buttonSpeed;
            SpawnButtons();
        }

        private void SpawnButtons()
        {
            //first of all, get the "width" (radius, in cas it's circular) of the sprite that represents a button
            //To do so, we use the background of the button. It's always a circle whose xscale = yscale gives us the radius of the button.
            float diameterOfAButton = _buttonPrefab.GetComponent<QTEButtonManagerComponent>().GetBackgroundScale();
            Vector3 currentPositionOfPoint = new Vector3(transform.position.x + _xOffsetButtons + diameterOfAButton, transform.position.y, 0);
            for(int i = 0; i < _numberOfButtonsToPress; i++)
            {
                GameObject buttonGO = Instantiate(_buttonPrefab);
                EnumsQTESteal.KeysToPress keyChosen = (EnumsQTESteal.KeysToPress)Random.Range(0, System.Enum.GetValues(typeof(EnumsQTESteal.KeysToPress)).Length);
                buttonGO.GetComponent<QTEButtonManagerComponent>().Initialize(this, keyChosen);
                buttonGO.transform.position = currentPositionOfPoint;
                buttonGO.GetComponent<QTEButtonManagerComponent>().SetVelocity(-_buttonSpeed, 0f);

                currentPositionOfPoint.x += diameterOfAButton + Random.Range(0, _maxNumberOfEmptySpaces + 1) * diameterOfAButton;

            }
        }




        //used by a button spawned by this manager to notify that a button has ended its life (either was correctly pressed or not).
        public void NotifyButton()
        {
            _numberOfButtonsEnded++;
        }


        //called by a button spawned by this manager to notify that a button was correctly pressed
        public void NotifyButtonSuccess()
        {
            _numberOfButtonsCorrectlyPressed++;
        }


    }

}
