using System.Collections;
using System.Collections.Generic;
using Music;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SettingsButtonManager : GOButtonManager
{
    public GameObject MainMenuButtons;
    public GameObject SettingsMenu;
    public override void OnButtonClick(FMQButtonType type)
    {
        if (type == FMQButtonType.PauseDefaultValues)
        {
            FindObjectOfType<MusicManagerComponent>().DefaultVolumes();
            //PauseMenu.SetActive(false);
        }
        else if (type == FMQButtonType.MainMenu)
        {
            MainMenuButtons.SetActive(true);
            SettingsMenu.SetActive(false);
        }
    }
    
}