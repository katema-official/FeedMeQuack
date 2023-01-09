using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateAvailableController : MonoBehaviour
{
    public GameObject _textMeshProUGUI;
    private const string _version = "1.0.1", _latestUpdateDay = "08/01/2023";
    private bool _isLatestVersion;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetText());
    }

    private IEnumerator GetText()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            var www = UnityWebRequest.Get("https://polimi-game-collective.itch.io/feed-me-quack");
            yield return www.SendWebRequest();

            if (!www.downloadHandler.text.Contains(_latestUpdateDay))
            {
                _isLatestVersion = true;
                _textMeshProUGUI.SetActive(!_textMeshProUGUI.activeInHierarchy);
                Application.OpenURL("https://polimi-game-collective.itch.io/feed-me-quack");
            }
            /*if (www.result != UnityWebRequest.Result.Success)
            {
                //Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text.Contains(_latestUpdateDay));
            }*/
        }
    }

    public void SendToSite()
    {
        Application.OpenURL("https://polimi-game-collective.itch.io/feed-me-quack");
    }
}
