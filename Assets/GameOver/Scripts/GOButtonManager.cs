using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOButtonManager : MonoBehaviour
{

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
        //foreach(Transform c in transform)
        //{
        //    c.GetComponent<GOButton>().SetEnable(false);
        //}

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
