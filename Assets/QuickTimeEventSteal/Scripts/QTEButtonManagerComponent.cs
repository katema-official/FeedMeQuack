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


        //tolerance: when the center of the empty circle and the center of this button have a distance <= tolerance * max radius (of this button),
        //the button is in the InPressing state. Tolerance is basically a way to specify the radius of this circle
        public void Initialize(Sprite spr, EnumsQTESteal.KeysToPress keyEnum, List<KeyCode> keysActual, float tolerance = 1f)
        {
            _sprite = spr;
            _keyToPressEnum = keyEnum;
            _keyToPressActual = keysActual;
            if(tolerance < 0f || tolerance > 1f)
            {
                //we ignore the value passed
                tolerance = transform.Find("Sprite").GetComponent<CircleCollider2D>().radius;
            }
            else
            {
                //we multiply it for the radius of the button
                transform.Find("Sprite").GetComponent<CircleCollider2D>().radius *= tolerance;
            }

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

        }




        

    }

}