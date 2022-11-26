using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QTEStealNamespace
{

    public class EnumsQTESteal
    {

        public enum KeysToPress
        {
            Up,
            Down,
            Left,
            Right
        };

        public enum QTEButtonState
        {
            BeforePress,    //when the button is moving towars the area in which it needs to be pressed
            InPressing,     //when the button is inside the area in which it needs to be pressed
            Success,        //if the button was correctly pressed
            Failure,         //If the button was not correctly pressed or not pressed at all
            Final           //goes to this state immediately after going to success or failure.
        }
    }


}