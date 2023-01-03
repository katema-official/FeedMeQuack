using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FMQGameOverEndButtonManager : GOButtonManager
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
    }
}
