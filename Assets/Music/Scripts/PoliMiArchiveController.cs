using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Music
{
    public class PoliMiArchiveController : MonoBehaviour
    {
        private const float TimeToDestroyPoliMi = 9.55f;
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
            var gamepad = Gamepad.current;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) ||
                    (gamepad != null && (gamepad.startButton.wasPressedThisFrame || gamepad.aButton.wasPressedThisFrame)) || _lifeTime >= TimeToDestroyPoliMi)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

    }
}