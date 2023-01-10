using Music;
using UnityEngine;

public class PauseMenuButtonManager : GOButtonManager
{
    public GameObject PauseSettings;
    public GameObject BackToPauseMenu;
    public override void OnButtonClick(FMQButtonType type)
    {
        if (type == FMQButtonType.PauseBackToMainMenu)
        {
            FindObjectOfType<PauseManager>().BackToMainMenu();
        }
        else if (type == FMQButtonType.PauseSettings)
        {
            BackToPauseMenu.SetActive(false);
            PauseSettings.SetActive(true);
        }
        else if (type == FMQButtonType.PauseBackToGame)
        {
            BackToPauseMenu.SetActive(true);
            PauseSettings.SetActive(false);
            FindObjectOfType<PauseManager>().Pause();
        }
        else if (type == FMQButtonType.EnableTutorial)
        {
            FindObjectOfType<TutorialComponent>().Enable();
            BackToPauseMenu.SetActive(true);
            PauseSettings.SetActive(false);
            FindObjectOfType<PauseManager>().Pause();
        }
    }
    
}