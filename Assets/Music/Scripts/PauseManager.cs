using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Music
{
   // This script is for Pause Menu in-game
   public class PauseManager : MonoBehaviour
   {
      public AudioMixerSnapshot paused;
      public AudioMixerSnapshot unpaused;
      public GameObject canvasPauseMenu;

      private void Update()
      {
         var gamepad = Gamepad.current;
         if (gamepad != null)
         {
            if (gamepad.startButton.wasPressedThisFrame)
            {
               Pause();
            }
               
         }
         
         if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "MainMenu" && SceneManager.GetActiveScene().name != "GameOverScreen" && SceneManager.GetActiveScene().name != "GameEnd")
         {
            Pause();
         }
      }

      public void Pause()
      {
         Cursor.visible = !canvasPauseMenu.activeInHierarchy;
         canvasPauseMenu.SetActive(!canvasPauseMenu.activeInHierarchy);
         Time.timeScale = Time.timeScale == 0 ? 1 : 0;
         Lowpass();
      }

      private void Lowpass()
      {
         if (Time.timeScale == 0)
         {
            paused.TransitionTo(0.001f);
         }
         else
         {
            unpaused.TransitionTo(0.001f);
         }
      }

      public void Quit()
      {
#if UNITY_EDITOR
         EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
      }

      public void BackToMainMenu()
      {
            Pause();
            UniversalAudio.StopAllMusic();

            Destroy(GameObject.Find("LevelStageManagerObject"));
            Destroy(GameObject.Find("DuckPlayer"));
            Destroy(GameObject.Find("DuckTypeManager"));
            Destroy(GameObject.Find("HUD"));
            Destroy(GameObject.Find("Minimap"));
            
            SceneManager.LoadScene("Music/MainMenu");
         
        }
      
      public void BackToGame()
      {
         Pause();
      }

   }
}