using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
}
