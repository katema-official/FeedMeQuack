using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Music
{

    public class MusicManagerComponent : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioSource audioSource1;
        [SerializeField] private AudioSource audioSource2;
        [SerializeField] private Slider[] musicSliders;
        [SerializeField] private Slider[] soundSliders;
        private static int _slidersIndex;
        private static float _musicVolumeAudioSource1, _soundVolume;
        private static Dictionary<Scene, int> _sceneAndIndexDictionary;

        private void Awake()
        {
            InitSlidersAndPreferences();
        }

        private void Start()
        {
            UniversalAudio.InitAllCoroutine(gameObject);
        }

        private void Update()
        {
            if (musicSliders[0] == null && soundSliders[0] == null)
            {
                musicSliders[0] = musicSliders[1];
                soundSliders[0] = soundSliders[1];
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                UniversalAudio.PlayStealing("Duck", "Swan", transform);
            }
            _slidersIndex = SceneManager.GetActiveScene().buildIndex == 4 ? 0 : 1;

            soundSliders[_slidersIndex].onValueChanged.AddListener(delegate { UpdateRightSliders(_slidersIndex);});
            musicSliders[_slidersIndex].onValueChanged.AddListener(delegate { UpdateRightSliders(_slidersIndex);});
            
        }

        private void UpdateRightSliders(int sliderIndex)
        {
            SetSoundVolume(soundSliders[sliderIndex].value);
            SetMusicVolume(musicSliders[sliderIndex].value);
            
            mixer.SetFloat("musicVolume", Mathf.Log10(musicSliders[sliderIndex].value* 20));
            mixer.SetFloat("soundVolume", Mathf.Log10(soundSliders[sliderIndex].value * 20));
            audioSource1.volume = musicSliders[sliderIndex].value;
            audioSource2.volume = musicSliders[sliderIndex].value;
            
            _musicVolumeAudioSource1 = audioSource1.volume;
            _soundVolume = soundSliders[sliderIndex].value;
            musicSliders[1].value = SceneManager.GetActiveScene().buildIndex == 4 ? musicSliders[0].value : musicSliders[1].value;
            soundSliders[1].value = SceneManager.GetActiveScene().buildIndex == 4 ? soundSliders[0].value : soundSliders[1].value;
        }

        private void InitSlidersAndPreferences()
        {
            musicSliders[0].minValue = 0.0001f;
            musicSliders[0].maxValue = 1;
            musicSliders[0].wholeNumbers = false;
            musicSliders[0].value = PlayerPrefs.GetFloat("MusicValue", 0.40f);
            soundSliders[0].minValue = 0.0001f;
            soundSliders[0].maxValue = 1;
            soundSliders[0].wholeNumbers = false;
            soundSliders[0].value = PlayerPrefs.GetFloat("SoundValue", 0.85f);
            
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
            
            _slidersIndex = SceneManager.GetActiveScene().buildIndex == 4 ? 0 : 1;
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