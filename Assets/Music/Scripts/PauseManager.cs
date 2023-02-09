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
            //if (gamepad != null)
            //{
            //   if (gamepad.startButton.wasPressedThisFrame)
            //   {
            //      Pause();
            //   }

            //}
         var tut = FindObjectOfType<TutorialComponent>();
         if (tut)
         {
            if (tut.IsActive()) 
                    return;
         }



         if ((Input.GetKeyDown(KeyCode.Escape) || (gamepad != null  && gamepad.startButton.wasPressedThisFrame )) && SceneManager.GetActiveScene().name != "MainMenu" && SceneManager.GetActiveScene().name != "GameOverScreen" && SceneManager.GetActiveScene().name != "GameEnd")
         {
            Pause();
         }
      }
        public bool IsActive()
        {
            return canvasPauseMenu.activeInHierarchy;
        }
      public void Pause()
      {
         Cursor.visible = !canvasPauseMenu.activeInHierarchy;
         canvasPauseMenu.SetActive(!canvasPauseMenu.activeInHierarchy);
            if (canvasPauseMenu.activeInHierarchy) Time.timeScale = 0;
            else Time.timeScale = 1;

            Lowpass();
      }

      private void Lowpass()
      {
         if (canvasPauseMenu.activeInHierarchy)
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
            Destroy(GameObject.Find("BigMinimap"));
            if (GameObject.Find("Tutorial")) Destroy(GameObject.Find("Tutorial"));
            SceneManager.LoadScene("Music/MainMenu");
         
        }
      
      public void BackToGame()
      {
         Pause();
      }

   }
}