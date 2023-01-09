using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

namespace Music
{

    public class DisclaimerController : MonoBehaviour
    {
        private const float _timeForMainMenu = 15f;
        private const string _version = "1.0.1", _latestUpdateDay = "09/01/2023";

        private void Start()
        {
            if (!PlayerPrefs.HasKey("Version") || !PlayerPrefs.GetString("Version").Equals(_version))
            {
                PlayerPrefs.SetString("Version", _version);
                PlayerPrefs.SetInt("Tutorial", 1);
            }
            
        }
        
        private IEnumerator GetText() {
            var www = UnityWebRequest.Get("https://polimi-game-collective.itch.io/feed-me-quack");
            yield return www.SendWebRequest();
 
            if (www.result != UnityWebRequest.Result.Success) {
                Debug.Log(www.error);
            }
            else {
                // Show results as text
                Debug.Log(www.downloadHandler.text.Contains(_latestUpdateDay));
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (Time.time >= _timeForMainMenu || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || 
                Input.GetKeyDown(KeyCode.Insert))
            {
                SceneManager.LoadScene("XboxController");
            }

            

            var gamepad = Gamepad.current;
            if (gamepad != null)
            {
                if (gamepad.startButton.wasPressedThisFrame || gamepad.aButton.wasPressedThisFrame)
                {
                    SceneManager.LoadScene("XboxController");
                }
                    
            }
        }
    }
}