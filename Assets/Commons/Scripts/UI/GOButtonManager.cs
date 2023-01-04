using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class GOButtonManager : MonoBehaviour
{
    private int _currentIndex = 0;
    private int _maxIndex = 0;
    private GOButton _button = null;
    bool _gamePadUp = false;
    bool _gamePadDown = false;


    public virtual void OnButtonClick(FMQButtonType type)
    {
        //if (type== FMQButtonType.Play)
        //{
        //    SceneManager.LoadScene("StartRunLoading");
        //}
        //else if (type == FMQButtonType.MainMenu)
        //{
        //    SceneManager.LoadScene("MainMenu");
        //}
        //else if (type == FMQButtonType.MainMenu)
        //{
        //    SceneManager.LoadScene("MainMenu");
        //}
        //else if (type == FMQButtonType.MainMenu)
        //{
        //    SceneManager.LoadScene("MainMenu");
        //}
        //else if (type == FMQButtonType.MainMenu)
        //{
        //    SceneManager.LoadScene("MainMenu");
        //}
    }



    public void SetEnableButtons(bool enable)
    {

        foreach (Transform c in transform)
        {
            c.GetComponent<GOButton>().SetEnable(enable);
        }
    }


    public void SetCurrentButtonIndex(int index)
    {
        _currentIndex = index;


        if (index < 0)
            _currentIndex = _maxIndex;
        else if (index > _maxIndex)
            _currentIndex = 0;

        foreach (Transform c in transform)
        {
            if (c.GetComponent<GOButton>().GetIndex() == _currentIndex)
            { 
                c.GetComponent<GOButton>().SetEnable(true);
                _button = c.GetComponent<GOButton>();
            }
            else
                c.GetComponent<GOButton>().SetEnable(false);
        }
    }


    // Start is called before the first frame update
    void Awake()
    {
      




    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform c in transform)
        {
            if (c.GetComponent<GOButton>().GetIndex() > _maxIndex)
                _maxIndex = c.GetComponent<GOButton>().GetIndex();
        }

        SetCurrentButtonIndex(0);
    }

    // Update is called once per frame
    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.wKey.wasReleasedThisFrame) SetCurrentButtonIndex(_currentIndex - 1);
            if (keyboard.sKey.wasReleasedThisFrame) SetCurrentButtonIndex(_currentIndex + 1);
            if (keyboard.upArrowKey.wasReleasedThisFrame) SetCurrentButtonIndex(_currentIndex - 1);
            if (keyboard.downArrowKey.wasReleasedThisFrame) SetCurrentButtonIndex(_currentIndex + 1);
            if (keyboard.enterKey.wasReleasedThisFrame) 
                OnButtonClick(_button.GetButtonType());
        }

        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            Vector2 move = gamepad.leftStick.ReadValue();
            bool _switch = false;

            if (move.y > -0.4f && move.y < 0.4f)
            {
                _switch = false;
                _gamePadUp = false;
                _gamePadDown = false;
            }

            if (move.y>0.8f)
            {
               if (!_gamePadUp)
               {
                    _switch = true;
                    _gamePadUp = true;
               }
            }
            else if (move.y < -0.8f)
            {
                if (!_gamePadDown)
                {
                    _switch = true;
                    _gamePadDown = true;
                }
            }



            if (_switch)
            {
                if (_gamePadUp) SetCurrentButtonIndex(_currentIndex - 1);
                if (_gamePadDown)  SetCurrentButtonIndex(_currentIndex + 1);
                _switch = false;
            }


            if (gamepad.aButton.wasReleasedThisFrame)
                OnButtonClick(_button.GetButtonType());

            //    if (keyboard.wKey.isPressed) SetCurrentButtonIndex(_currentIndex - 1);
            //    if (keyboard.sKey.isPressed) SetCurrentButtonIndex(_currentIndex + 1);
            //    if (keyboard.upArrowKey.isPressed) SetCurrentButtonIndex(_currentIndex - 1);
            //    if (keyboard.downArrowKey.isPressed) SetCurrentButtonIndex(_currentIndex + 1);
        }
    }
}
