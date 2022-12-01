using System.Collections.Generic;
using System.Collections;
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
        private static readonly string Duck = "Duck", Goose = "Goose", Swan = "Swan";
        public static Dictionary<string, int> stringAndNumberDictionary;

        private void Awake()
        {
            InitSlidersAndPreferences();
            UniversalAudio.InitAllCoroutine(gameObject);
            //UniversalAudio.PlayMusic("Menu", true);
        }

        private static void InitAnimalsSound()
        {
            stringAndNumberDictionary ??= new Dictionary<string, int>()
                {
                    [Duck] = 19,
                    [Goose] = 32,
                    [Swan] = 18
                };
        }

        private void Start()
        {
            Component[] sliders = GameObject.Find("MainMenuCanvas").transform.GetComponentsInChildren(typeof(Slider), true);
            foreach (Slider s in sliders)
            {
                if (s.gameObject.name == "MusicSliderMainMenu")
                {
                    musicSliders[0] = s;
                }
                if (s.gameObject.name == "SoundSliderMainMenu")
                {
                    soundSliders[0] = s;
                }
            }
            InitAnimalsSound();
            UniversalAudio.PlayMusic("Menu", true);
        }

        private void Update()
        {
            if(UniversalAudio.universalAudioMonoBehaviour == null)
            {
                UniversalAudio.Init(gameObject);    //???
            }

            if (musicSliders[0] == null && soundSliders[0] == null)
            {
                musicSliders[0] = musicSliders[1];
                soundSliders[0] = soundSliders[1];
            }
            
            _slidersIndex = SceneManager.GetActiveScene().name == "MainMenu" ? 0 : 1;

            soundSliders[_slidersIndex].onValueChanged.AddListener(delegate { UpdateRightSliders(_slidersIndex);});
            musicSliders[_slidersIndex].onValueChanged.AddListener(delegate { UpdateRightSliders(_slidersIndex);});
        }

        private void UpdateRightSliders(int sliderIndex)
        {
            SetSoundVolume(soundSliders[sliderIndex].value);
            SetMusicVolume(musicSliders[sliderIndex].value);
            
            musicSliders[1].value = SceneManager.GetActiveScene().name == "MainMenu" ? musicSliders[0].value : musicSliders[1].value;
            soundSliders[1].value = SceneManager.GetActiveScene().name == "MainMenu" ? soundSliders[0].value : soundSliders[1].value;
            
            mixer.SetFloat("musicVolume", Mathf.Log10(musicSliders[sliderIndex].value* 20));
            mixer.SetFloat("soundVolume", Mathf.Log10(soundSliders[sliderIndex].value * 20));
            audioSource1.volume = musicSliders[sliderIndex].value;
            audioSource2.volume = musicSliders[sliderIndex].value;
            
            _musicVolumeAudioSource1 = audioSource1.volume;
            _soundVolume = soundSliders[sliderIndex].value;
        }

        private void InitSlidersAndPreferences()
        {
            musicSliders[0].minValue = 0.0001f;
            musicSliders[0].maxValue = 1;
            musicSliders[0].wholeNumbers = false;
            musicSliders[0].value = PlayerPrefs.GetFloat("MusicValue", 0.25f);
            soundSliders[0].minValue = 0.0001f;
            soundSliders[0].maxValue = 1;
            soundSliders[0].wholeNumbers = false;
            soundSliders[0].value = PlayerPrefs.GetFloat("SoundValue", 1);
            
            SetMusicVolume(musicSliders[0].value);
            SetSoundVolume(soundSliders[0].value);
            
            musicSliders[1].minValue = 0.0001f;
            musicSliders[1].maxValue = 1;
            musicSliders[1].wholeNumbers = false;
            musicSliders[1].value = musicSliders[0].value;
            
            soundSliders[1].minValue = 0.0001f;
            soundSliders[1].maxValue = 1;
            soundSliders[1].wholeNumbers = false;
            soundSliders[1].value = soundSliders[0].value;
            
            _slidersIndex = SceneManager.GetActiveScene().name == "MainMenu" ? 0 : 1;
            mixer.SetFloat("musicVolume", Mathf.Log10(musicSliders[0].value * 20));
            mixer.SetFloat("soundVolume", Mathf.Log10(soundSliders[0].value * 20));
            audioSource1.volume = musicSliders[_slidersIndex].value;
            audioSource2.volume = musicSliders[_slidersIndex].value;

            _musicVolumeAudioSource1 = musicSliders[_slidersIndex].value;
            _soundVolume = soundSliders[_slidersIndex].value;
        }

        private static void SetMusicVolume(float newValue)
        {
            PlayerPrefs.SetFloat("MusicVolume", newValue);
            PlayerPrefs.Save();
        }

        private static void SetSoundVolume(float newValue)
        {
            PlayerPrefs.SetFloat("SoundVolume", newValue);
            PlayerPrefs.Save();
        }
        
        public void DefaultVolumes()
        {
            mixer.ClearFloat("soundVolume");
            mixer.ClearFloat("musicVolume");
            SetMusicVolume(0.25f);
            SetSoundVolume(1);

            soundSliders[0].value = 1;
            musicSliders[0].value = 0.25f;
            
            musicSliders[1].value = SceneManager.GetActiveScene().name == "MainMenu" ? musicSliders[0].value : musicSliders[1].value;
            soundSliders[1].value = SceneManager.GetActiveScene().name == "MainMenu" ? soundSliders[0].value : soundSliders[1].value;
            
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