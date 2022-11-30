using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Music
{
   public class PauseManager : MonoBehaviour
   {
      public AudioMixerSnapshot paused;
      public AudioMixerSnapshot unpaused;
      public GameObject canvasPauseMenu;

      private void Update()
      {
         if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 4 && SceneManager.GetActiveScene().buildIndex != 1)
         {
            canvasPauseMenu.SetActive(!canvasPauseMenu.activeInHierarchy);
            Pause();
         }
      }

      private void Pause()
      {
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
         // Go in File -> Build Settings -> Add Open Scenes. Here will be loaded the scene at the specified index
         SceneManager.LoadScene(4);
      }
      
      public void BackToGame()
      {
         Pause();
      }

   }
}