using UnityEngine;
using UnityEngine.SceneManagement;

namespace Music
{
    public class PoliMiArchiveController : MonoBehaviour
    {
        private const float TimeToDestroyPoliMi = 8.9f;
        private static AudioSource _audioSource;
        private float _lifeTime;

        public static AudioSource GetAudioSource()
        {
            return _audioSource;
        }

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
            _lifeTime += Time.deltaTime;
            
            if (Input.GetKeyDown(KeyCode.Space) || _lifeTime >= TimeToDestroyPoliMi)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
        
    }
}
