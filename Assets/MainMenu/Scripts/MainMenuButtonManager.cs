using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuButtonManager : GOButtonManager
{
    public GameObject MainMenu;
    public GameObject SettingsMenu;
    public GameObject MainMenuButtons;
    public GameObject FeedbackMenu;
    public GameObject AboutMenu;

    private bool ok = false;
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.1f);
        ok = true;
        yield return null;
    }

    void Awake()
    {
        StartCoroutine(Wait());
    }

    public override void OnButtonClick(FMQButtonType type)
    {
        if (!ok) return;
        if (type== FMQButtonType.Play)
        {
            SceneManager.LoadScene("StartRunLoading");
        }
        else if (type == FMQButtonType.MainMenu)
        {
            MainMenuButtons.SetActive(true);
            //SceneManager.LoadScene("MainMenu");
        }
        else if (type == FMQButtonType.Settings)
        {
            MainMenuButtons.SetActive(false);
            SetMenu(Menu.Settings);
        }
        else if (type == FMQButtonType.Credits)
        {
            MainMenuButtons.SetActive(false);
            SetMenu(Menu.About);
        }
        else if (type == FMQButtonType.Feedback)
        {
            MainMenuButtons.SetActive(false);
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