using Music.Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void Play()
   {
      // Go in File -> Build Settings -> Add Open Scenes. Here will be loaded the scene at the specified index
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
   }

   public void Quit()
   {
      Application.Quit();
      Debug.Log("Player has quit the game!");
   }
}
