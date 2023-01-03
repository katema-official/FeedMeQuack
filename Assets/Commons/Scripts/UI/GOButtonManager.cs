using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GOButtonManager : MonoBehaviour
{
    private int _currentIndex = 0;



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


    public void SetCurrentButtonIndex(int index)
    {
        _currentIndex = index;

        foreach (Transform c in transform)
        {
            if (c.GetComponent<GOButton>().GetIndex() == _currentIndex)
                c.GetComponent<GOButton>().SetEnable(true);
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
        SetCurrentButtonIndex(0);
        //SetEnableButtons(false);
        // transform.GetChild(0).GetComponent<GOButton>().SetEnable(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
