using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class MenuManager : Singleton<MenuManager>
{
    public enum Menu
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
        MainMenu.SetActive(false);
        LevelsMenu.SetActive(false);
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
    // Start is called before the first frame update
    void Start()
    {    
        SetMenu(Menu.Main);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetMenu(Menu.Main);
        }
    }
    
    public void OpenMainMenu()
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
    }

    public void Submit()
    {
        Debug.Log("Submitting Feedback");
        SetMenu(Menu.Main);
    }
}
