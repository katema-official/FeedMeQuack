using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GOButtonManager : MonoBehaviour
{
    public void OnButtonClick(FMQButtonType type)
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





    // Start is called before the first frame update
    void Awake()
    {
      




    }

    // Start is called before the first frame update
    void Start()
    {
        SetEnableButtons(false);
        transform.GetChild(0).GetComponent<GOButton>().SetEnable(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
