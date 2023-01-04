using UnityEngine;
using UnityEngine.SceneManagement;

namespace Music
{

    public class DisclaimerController : MonoBehaviour
    {
        private const float TimeForMainMenu = 8f;

        // Update is called once per frame
        private void Update()
        {
            if (Time.time >= TimeForMainMenu || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || 
                Input.GetKeyDown(KeyCode.Insert))
            {
                SceneManager.LoadScene("PolimiLogo");
            }
        }
    }
}