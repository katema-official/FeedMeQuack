using Music;
using UnityEngine;

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