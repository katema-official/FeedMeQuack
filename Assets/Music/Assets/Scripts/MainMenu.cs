using UnityEngine;
using UnityEngine.SceneManagement;

namespace Music.Assets.Scripts // To change correctly
{
   public class MainMenu : MonoBehaviour
   {
      public void Play()
      {
         UniversalAudio.StopAllMusic();
         // Go in File -> Build Settings -> Add Open Scenes. Here will be loaded the scene at the specified index
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
      }

      public void Quit()
      {
         Application.Quit();
      }

      private void Awake()
      {
         //UniversalAudio.CreateSlider();
         UniversalAudio.InitAllCoroutine();
      }

      private void Start()
      {
         UniversalAudio.PlayMusic("Menu", false);
      }

      private void Update()
      {
         //StartCoroutine(UniversalAudio.ChangeVolumes());
         StartCoroutine(UniversalAudio.UpdateTime());
      }
   }
}