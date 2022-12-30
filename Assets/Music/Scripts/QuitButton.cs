using UnityEditor;
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
