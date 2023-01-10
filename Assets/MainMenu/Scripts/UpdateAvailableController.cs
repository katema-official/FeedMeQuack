using System.Collections;
using System.Collections.Generic;
using Music;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateAvailableController : MonoBehaviour
{
    [SerializeField] private GameObject textMeshProUGUI;

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

            if (!www.downloadHandler.text.Contains(PlayerPrefs.GetString("Version", DisclaimerController.GetGameVersion())))
            {
                textMeshProUGUI.SetActive(!textMeshProUGUI.activeInHierarchy);
                //Application.OpenURL("https://polimi-game-collective.itch.io/feed-me-quack");
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
}
