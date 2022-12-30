using UnityEngine;

namespace Music
{
    public class FirstAudioSourceDeleter : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            PoliMiArchiveController.GetAudioSource().Stop();
            Destroy(GameObject.Find("AudioSource"));
        }

    }
}