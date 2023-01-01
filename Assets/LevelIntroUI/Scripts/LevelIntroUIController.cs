using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LevelIntroUIController : MonoBehaviour
{
    public void SetString(string str)
    {
        GetComponentInChildren<TextFadeInOut>().StartAnim(str);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
