using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Music
{

    public class DisclaimerController : MonoBehaviour
    {
        private const float _timeForMainMenu = 15f;
        private const string _version = "1.0.4";

        public static string GetGameVersion()
        {
            return _version;
        }
        
        private void Start()
        {
            if (!PlayerPrefs.HasKey("Version") || !PlayerPrefs.GetString("Version").Equals(_version))
            {
                PlayerPrefs.SetString("Version", _version);
                PlayerPrefs.SetInt("Tutorial", 1);
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