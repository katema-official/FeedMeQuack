using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuButtonManager : GOButtonManager
{
    public override void OnButtonClick(FMQButtonType type)
    {
        if (type== FMQButtonType.Play)
        {
            SceneManager.LoadScene("StartRunLoading");
        }
        else if (type == FMQButtonType.MainMenu)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else if (type == FMQButtonType.Settings)
        {
            SetMenu(Menu.Settings);
        }
        else if (type == FMQButtonType.Credits)
        {
            SetMenu(Menu.About);
        }
        else if (type == FMQButtonType.Feedback)
        {
            SetMenu(Menu.Feedback);
        }
        else if (type == FMQButtonType.Quit)
        {
            Quit();
        }
    }
    
    private void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
    
    private enum Menu
    {
        Main,
        Levels,
        Settings,
        Feedback,
        About
    };
    
    public GameObject MainMenu;
    public GameObject SettingsMenu;
    public GameObject LevelsMenu;
    public GameObject FeedbackMenu;
    public GameObject AboutMenu;

    private void SetMenu(Menu menu)
    {
        //MainMenu.SetActive(false);
        //LevelsMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        FeedbackMenu.SetActive(false);
        AboutMenu.SetActive(false);

        switch (menu)
        {
            case Menu.Main:
                MainMenu.SetActive(true);
                break;
            case Menu.Settings:
                SettingsMenu.SetActive(true);
                break;
            case Menu.Levels:
                LevelsMenu.SetActive(true);
                break;
            case Menu.Feedback:
                FeedbackMenu.SetActive(true);
                break;
            case Menu.About:
                AboutMenu.SetActive(true);
                break;
        }
    }

    public void Play()
    {
        
    }

    /*public void OpenMainMenu()
    {
        SetMenu(Menu.Main);
    }
    
    public void OpenLevelsMenu()
    {
        SceneManager.LoadScene("StartRunLoading");
        //SetMenu(Menu.Levels);
    }

    public void OpenSettingsMenu()
    {
        SetMenu(Menu.Settings);
    }
    
    public void OpenFeedbackMenu()
    {
        SetMenu(Menu.Feedback);
    }

    public void OpenAboutMenu()
    {
        SetMenu(Menu.About);
    }*/
    
}