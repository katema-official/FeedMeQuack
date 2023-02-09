using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Music
{

    public class XboxSceneController : MonoBehaviour
    {
        private const float _timeForMainMenu = 5f;
        private float _time;

        // Update is called once per frame
        private void Update()
        {
            _time += Time.deltaTime;
            if (_time >= _timeForMainMenu || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || 
                Input.GetKeyDown(KeyCode.Insert))
            {
                SceneManager.LoadScene("TeamScreen");
            }

            

            var gamepad = Gamepad.current;
            if (gamepad != null)
            {
                if (gamepad.startButton.wasPressedThisFrame || gamepad.aButton.wasPressedThisFrame)
                {
                    SceneManager.LoadScene("TeamScreen");
                }
                    
            }
        }
    }
}