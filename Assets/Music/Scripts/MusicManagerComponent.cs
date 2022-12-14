using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Music
{
    // This script sets the sliders, volumes, AudioSources and AudioMixers at the right values and settings. To attache to 
    // AudioManager in hierarchy

    public class MusicManagerComponent : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioSource audioSource1;
        [SerializeField] private AudioSource audioSource2;
        [SerializeField] private Slider[] musicSliders;
        [SerializeField] private Slider[] soundSliders;
        private static int _slidersIndex;
        private static float _musicVolumeAudioSource1, _soundVolume;
        private const string Duck = "Duck", Goose = "Goose", Coot = "Coot";
        public static Dictionary<string, int> stringAndNumberDictionary;
        private const float DefaultMusicValue = 0.5f, DefaultSoundValue = 0.95f;
        private static Button _defaultButton;
        private const float TimeToDestroyPoliMi = 15f;
        private void Awake()
        {
            InitMusicSliders();
            InitSoundSliders();
            UniversalAudio.InitAll(gameObject);
        }

        private void Start()
        {
            var buttons = GameObject.Find("MainMenuCanvas").transform
                .GetComponentsInChildren<Button>(true);
            foreach (var button in buttons)
            {
                if (button.gameObject.name.Equals("ResetButton"))
                {
                    _defaultButton = button;
                    break;
                }
            }

            _defaultButton.onClick.AddListener(DefaultVolumes);

            InitAnimalsSound();
            UniversalAudio.PlayMusic("Swimming", false);
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().name.Equals("MainMenu") && (Input.GetKeyDown(KeyCode.Space) || Time.time >= TimeToDestroyPoliMi))
            {
                Destroy(GameObject.Find("SplashStartingMenu"));
            }

            if (!SceneManager.GetActiveScene().name.Equals("MainMenu") && musicSliders[0] != musicSliders[1])
            {
                musicSliders[0] = musicSliders[1];
                soundSliders[0] = soundSliders[1];
            }

            _slidersIndex = SceneManager.GetActiveScene().name.Equals("MainMenu") ? 0 : 1;

            soundSliders[_slidersIndex].onValueChanged
                .AddListener(delegate { UpdateRightSoundSliders(_slidersIndex); });
            musicSliders[_slidersIndex].onValueChanged
                .AddListener(delegate { UpdateRightMusicSliders(_slidersIndex); });

            if (GameObject.FindGameObjectsWithTag("AudioManager").Length >= 2)
            {
                foreach (var gO in GameObject.FindGameObjectsWithTag("AudioManager"))
                {
                    if (gO.transform.GetComponentsInChildren<AudioSource>()[0] != audioSource1)
                    {
                        DestroyImmediate(gO);
                        break;
                    }
                }

                SetRightSliders();
                
                var buttons = GameObject.Find("MainMenuCanvas").transform
                    .GetComponentsInChildren<Button>(true);
                foreach (var button in buttons)
                {
                    if (button.gameObject.name.Equals("ResetButton"))
                    {
                        _defaultButton = button;
                        break;
                    }
                }
                _defaultButton.onClick.AddListener(DefaultVolumes);
            }

        }

        private static void InitAnimalsSound()
        {
            stringAndNumberDictionary ??= new Dictionary<string, int>()
            {
                [Duck] = 19,
                [Goose] = 32,
                [Coot] = 8
            };
        }

        private void UpdateRightSoundSliders(int sliderIndex)
        {
            SetSoundVolume(soundSliders[sliderIndex].value);

            soundSliders[1].value = SceneManager.GetActiveScene().name.Equals("MainMenu")
                ? soundSliders[0].value
                : soundSliders[1].value;

            mixer.SetFloat("soundVolume", Mathf.Log10(soundSliders[sliderIndex].value * 20));

            _soundVolume = soundSliders[sliderIndex].value;
        }

        private void UpdateRightMusicSliders(int sliderIndex)
        {
            SetMusicVolume(musicSliders[sliderIndex].value);

            musicSliders[1].value = SceneManager.GetActiveScene().name.Equals("MainMenu")
                ? musicSliders[0].value
                : musicSliders[1].value;

            mixer.SetFloat("musicVolume", Mathf.Log10(musicSliders[sliderIndex].value * 20));

            audioSource1.volume = musicSliders[sliderIndex].value;
            _musicVolumeAudioSource1 = audioSource1.volume;

            audioSource2.volume = musicSliders[sliderIndex].value;
        }

        private void InitMusicSliders()
        {
            musicSliders[0].minValue = 0.0001f;
            musicSliders[0].maxValue = 1;
            musicSliders[0].wholeNumbers = false;
            musicSliders[0].value = PlayerPrefs.GetFloat("MusicValue", DefaultMusicValue);
            SetMusicVolume(musicSliders[0].value);

            musicSliders[1].minValue = 0.0001f;
            musicSliders[1].maxValue = 1;
            musicSliders[1].wholeNumbers = false;
            musicSliders[1].value = musicSliders[0].value;

            mixer.SetFloat("musicVolume", Mathf.Log10(musicSliders[0].value * 20));

            audioSource1.volume = musicSliders[0].value;
            audioSource2.volume = musicSliders[0].value;

            _musicVolumeAudioSource1 = musicSliders[0].value;
        }

        private void InitSoundSliders()
        {

            soundSliders[0].minValue = 0.0001f;
            soundSliders[0].maxValue = 1;
            soundSliders[0].wholeNumbers = false;
            soundSliders[0].value = PlayerPrefs.GetFloat("SoundValue", DefaultSoundValue);

            SetSoundVolume(soundSliders[0].value);

            soundSliders[1].minValue = 0.0001f;
            soundSliders[1].maxValue = 1;
            soundSliders[1].wholeNumbers = false;
            soundSliders[1].value = soundSliders[0].value;

            mixer.SetFloat("soundVolume", Mathf.Log10(soundSliders[0].value * 20));

            _soundVolume = soundSliders[0].value;
        }

        private void SetRightSliders()
        {
            var sliders = GameObject.Find("MainMenuCanvas").transform.GetComponentsInChildren<Slider>(true);
            foreach (var s in sliders)
            {
                switch (s.gameObject.name)
                {
                    case "MusicSliderMainMenu":
                        musicSliders[0] = s;
                        break;
                    case "SoundSliderMainMenu":
                        soundSliders[0] = s;
                        break;
                    default:
                        continue;
                }

                soundSliders[0].value = soundSliders[1].value;
                musicSliders[0].value = musicSliders[1].value;

                audioSource1.volume = musicSliders[0].value;
                audioSource2.volume = musicSliders[0].value;
            }
        }

        private static void SetMusicVolume(float newValue)
        {
            PlayerPrefs.SetFloat("MusicVolume", newValue);
            //PlayerPrefs.Save();
            _musicVolumeAudioSource1 = newValue;
        }

        private static void SetSoundVolume(float newValue)
        {
            PlayerPrefs.SetFloat("SoundVolume", newValue);
            //PlayerPrefs.Save();
            _soundVolume = newValue;
        }

        public void DefaultVolumes()
        {
            mixer.ClearFloat("soundVolume");
            mixer.ClearFloat("musicVolume");
            SetMusicVolume(DefaultMusicValue);
            SetSoundVolume(DefaultSoundValue);

            soundSliders[0].value = DefaultSoundValue;
            musicSliders[0].value = DefaultMusicValue;

            musicSliders[1].value = SceneManager.GetActiveScene().name.Equals("MainMenu")
                ? musicSliders[0].value
                : musicSliders[1].value;
            soundSliders[1].value = SceneManager.GetActiveScene().name.Equals("MainMenu")
                ? soundSliders[0].value
                : soundSliders[1].value;

            audioSource1.volume = musicSliders[0].value;
            audioSource2.volume = musicSliders[0].value;

            _musicVolumeAudioSource1 = audioSource1.volume;
            _soundVolume = soundSliders[0].value;
        }

        public static float GetAudioSourceVolume()
        {
            return _musicVolumeAudioSource1;
        }

        public static float GetSoundVolume()
        {
            return _soundVolume;
        }

    }
    
}