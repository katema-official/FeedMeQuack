using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Music
{

    public class DisclaimerController : MonoBehaviour
    {
        private const float TimeForMainMenu = 15f;

        // Update is called once per frame
        private void Update()
        {
            if (Time.time >= TimeForMainMenu || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || 
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