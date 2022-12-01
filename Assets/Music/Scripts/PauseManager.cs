using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
      [SerializeField] private int timesLoadedMainMenu;

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
         
         foreach (var o in FindObjectsOfType<GameObject>())
         {
            //if (o.transform.name.Equals("AudioManager"))
            {
               Destroy(o);
            }
         }
         
         // Go in File -> Build Settings -> Add Open Scenes. Here will be loaded the scene at the specified index
         SceneManager.LoadScene("Music/MainMenu");

         UniversalAudio.PlayMusic("Menu", true);
         }

      private static class ClassExtension
      {
         public static List<GameObject> GetAllChildren(GameObject go)
         {
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < go.transform.childCount; i++)
            {
               list.Add(go.transform.GetChild(i).gameObject);
            }

            return list;
         }
      }
      

      public void BackToGame()
      {
         Pause();
      }

   }
}