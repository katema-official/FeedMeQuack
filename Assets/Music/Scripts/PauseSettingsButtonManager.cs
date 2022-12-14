using System.Collections;
using System.Collections.Generic;
using Music;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseSettingsButtonManager : GOButtonManager
{
    public GameObject PauseMenu;
    public GameObject PauseSettingsMenu;
    public override void OnButtonClick(FMQButtonType type)
    {
        if (type == FMQButtonType.PauseDefaultValues)
        {
            FindObjectOfType<MusicManagerComponent>().DefaultVolumes();
            //PauseMenu.SetActive(false);
        }
        else if (type == FMQButtonType.PauseBackToPauseMenu)
        {
            PauseMenu.SetActive(true);
            PauseSettingsMenu.SetActive(false);
        }
    }
    
}