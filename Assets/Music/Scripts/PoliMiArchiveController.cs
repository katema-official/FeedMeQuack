using UnityEngine;
using UnityEngine.SceneManagement;

namespace Music
{
    public class PoliMiArchiveController : MonoBehaviour
    {
        private const float TimeToDestroyPoliMi = 8.8f;
        private AudioSource _audioSource;

        // Start is called before the first frame update
        private void Awake()
        {
            _audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
            _audioSource.volume = MusicManagerComponent.GetDefaultAudioSourceVolume();
        }

        private void Start()
        {
            _audioSource.Play();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Time.time >= TimeToDestroyPoliMi)
            {
                SceneManager.LoadScene("MainMenu");
            }

            if (SceneManager.GetActiveScene().name.Equals("MainMenu"))
            {
                Destroy(GameObject.Find("AudioSource"));
            }
        }
        
    }
}
