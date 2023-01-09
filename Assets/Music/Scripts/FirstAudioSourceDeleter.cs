using UnityEngine;

namespace Music
{
    public class FirstAudioSourceDeleter : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            if (GameObject.Find("AudioSource"))
            {
                PoliMiArchiveController.GetAudioSource().Stop();
                Destroy(GameObject.Find("AudioSource"));
                Time.timeScale = 1;
            }
        }

    }
}