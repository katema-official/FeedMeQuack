using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Music.Assets.Scripts // To change correctly
{
    public static class UniversalAudio
    {
        [RequireComponent(typeof(AudioSource))] // We're dealing with AudioSource. It's better to specify it 
        public class UniversalAudioMonoBehaviour : MonoBehaviour // To give MonoBehaviour to a static class
        {
        }

        private static UniversalAudioMonoBehaviour
            _universalAudioMonoBehaviour; // Variable reference for the class to use instance at the occurrence

        private static Dictionary<string, float>
            _audioClipTimerDictionary; // Keys are clips name, Values are the seconds where the clip will start

        // The more timeOfFading is, the longer the time to reach volume finalFadeVolume is
        private static float _timeOfFading = 1f;

        private static Slider _sliderController;
        
        private static float _startFadeInVolume;
        
        // _startFadeOutVolume and _finalFadeInVolume are essentially the max volume of the AudioListener (1 is 100% of the
        // AudioListener volume intensity). So, in the options these values will be set as the component value of the Slider 

        private static float _startFadeOutVolume = 1f;

        private static float _finalFadeInVolume = 1f;

        private static float _finalFadeOutVolume;

        private static AudioSource _audioSource1, _audioSource2;

        private const string PathFromSourcesForMusic = "Songs/"; // Inside folder "Resources", if there is a relative path, write it here

        private const string PathFromSourcesForSound = "SFX/"; // Inside folder "Resources", if there is a relative path, write it here

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Main initializations 

        //Now Initialize the variable (the instance _universalAudioMonoBehaviour)
        private static void Init()
        {
            // If the instance not exists the first time we call the static class
            if (_universalAudioMonoBehaviour == null)
            {
                //Create an empty object called UniversalAudioMB
                GameObject gameObject = new GameObject("UniversalAudioMonoBehaviour");


                //Add this script to the object
                _universalAudioMonoBehaviour = gameObject.AddComponent<UniversalAudioMonoBehaviour>();
            }
        }

        private static void CreateAudioSources()
        {
            // Create the AudioSources if they not exist
            if (_audioSource1 == null)
            {
                //Create an empty object called MyStatic
                GameObject gameObject = new GameObject("AudioSource1");

                //Add this script to the object
                _audioSource1 = gameObject.AddComponent<AudioSource>();
            }

            if (_audioSource2 == null)
            {
                GameObject gameObject = new GameObject("AudioSource2");

                _audioSource2 = gameObject.AddComponent<AudioSource>();
            }

            // To enable looping 
            _audioSource1.loop = true;
            _audioSource2.loop = true;
        }

        // Initialize dictionary
        private static void InitDictionary()
        {
            if (_audioClipTimerDictionary.IsUnityNull()) // If not already initialized
            {
                _audioClipTimerDictionary = new Dictionary<string, float>();

                foreach (var clipName in Resources.LoadAll("Songs", typeof(AudioClip)))
                {
                    _audioClipTimerDictionary.Add(clipName.name, 0);
                }
            }
            else // If it exists, clean all the values to 0
            {
                foreach (var clipName in _audioClipTimerDictionary.Keys)
                {
                    _audioClipTimerDictionary[clipName] = 0;
                }
            }
        }

        // Overload to clean only the track whose name is clipName
        private static void InitDictionary(string clipName)
        {
            _audioClipTimerDictionary[clipName] = 0;
        }

        private static void CreateSlider()
        {
            // Create the Slider if it doesn't exist
            if (_sliderController == null)
            {
                //Create an empty object called MyStatic
                GameObject gameObject = new GameObject("SliderVolumeMusic");

                //Add this script to the object
                _sliderController = gameObject.AddComponent<Slider>();

                InitSliderValues();
            }
        }
        
        private static void InitSliderValues()
        {
            _sliderController.minValue = 0;
            _sliderController.maxValue = 1;
            _sliderController.wholeNumbers = false;
            _sliderController.value = 0.85f;
        }
        
        public static float GetSliderVolume()
        {
            return _sliderController.value;
        }

        public static void SetSliderVolume(float newVolume)
        {
            _sliderController.value = newVolume;
        }
        
        //Now, a simple function to initialize in one shot all the main stuff
        public static void InitAllCoroutine()
        {
            // Call the initializers 
            //CreateSlider();
            Init();
            CreateAudioSources();
            InitDictionary();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        // Getter and Setter of the variables
        public static void SetTimeOfFading(float newTime)
        {
            _timeOfFading = newTime;
        }

        public static float GetTimeOfFading()
        {
            return _timeOfFading;
        }

        public static void SetStarFadeInVolume(float newFadeInStartVolume)
        {
            _startFadeInVolume = newFadeInStartVolume;
        }

        public static float GetStarFadeInVolume()
        {
            return _startFadeInVolume;
        }

        public static void SetFinalFadeInVolume(float newFadeInFinalVolume)
        {
            _finalFadeInVolume = newFadeInFinalVolume;
        }

        public static float GetFinalFadeInVolume()
        {
            return _finalFadeInVolume;
        }

        public static void SetStartFadeOutVolume(float newFadeOutStartVolume)
        {
            _startFadeOutVolume = newFadeOutStartVolume;
        }

        public static float GetStartFadeOutVolume()
        {
            return _startFadeOutVolume;
        }

        public static void SetFinalFadeOutVolume(float newFadeOutFinalVolume)
        {
            _finalFadeOutVolume = newFadeOutFinalVolume;
        }

        public static float GetFinalFadeOutVolume()
        {
            return _finalFadeOutVolume;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Now the methods/functions
        
        // To do the fading used in PlayMusic()...
        private static IEnumerator
            FadeTrack(string clipName, bool fromStart) // fromStart = true if the song has to begin from the start
        {
            float timeElapsed = 0; // Initialize a counter

            if (!fromStart) // If the track has to resume from the point of interruption
            {
                if (_audioSource1.isPlaying) // If the first AudioSource is playing something...
                {
                    // It's the same of (AudioClip)Resources.Load(pathFromSourcesForMusic + clipName, typeof(AudioClip));
                    _audioSource2.clip = Resources.Load<AudioClip>(PathFromSourcesForMusic + clipName);

                    _audioSource2.time =
                        _audioClipTimerDictionary[clipName]; // (The incoming track starts from the value
                    // displayed in _audioClipTimerDictionary)
                    _audioSource2.Play();

                    while
                        (timeElapsed <
                         _timeOfFading) // ...bring its volume progressively to 0, while the second AudioSource does a Fade in
                    {
                        // Mathf.Lerp does a linear interpolation. It's the simplest way
                        _audioSource2.volume =
                            Mathf.Lerp(_startFadeInVolume, _finalFadeInVolume, timeElapsed / _timeOfFading);
                        _audioSource1.volume = Mathf.Lerp(_startFadeOutVolume, _finalFadeOutVolume,
                            timeElapsed / _timeOfFading);
                        timeElapsed += Time.deltaTime;
                        yield return null;
                    }

                    _audioSource1.Stop();
                }
                else // If it's the second AudioSource that's playing, do as above but swapped
                {
                    _audioSource1.clip = Resources.Load<AudioClip>(PathFromSourcesForMusic + clipName);
                    _audioSource1.time = _audioClipTimerDictionary[clipName];
                    _audioSource1.Play();
                    while (timeElapsed < _timeOfFading)
                    {
                        _audioSource1.volume =
                            Mathf.Lerp(_startFadeInVolume, _finalFadeInVolume, timeElapsed / _timeOfFading);
                        _audioSource2.volume = Mathf.Lerp(_startFadeOutVolume, _finalFadeOutVolume,
                            timeElapsed / _timeOfFading);
                        timeElapsed += Time.deltaTime;
                        yield return null;
                    }

                    _audioSource2.Stop();
                }
            }
            else // If we want that the music has to start from the beginning
            {
                InitDictionary(clipName); // Its new value is 0 now

                // Repeat the function but starting from the interruption point 0
                yield return FadeTrack(clipName, !fromStart);
            }
        }

        public static void PlayMusic(string songName, bool fromStart){
            //Call the Coroutine
            _universalAudioMonoBehaviour.StartCoroutine(FadeTrack(songName, fromStart));
        }
        
        public static void PlaySound(string clipName, Transform thisTransform)
        {
            if (clipName.Equals(
                    "GameOver")) // The GameOver is treated as a Sound that obliterate all other AudioSources
            {
                StopAllMusic();
            }

            // Create a temporary AudioSource that will die at the end of the sound 
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>(PathFromSourcesForSound + clipName),
                thisTransform.position);
        }

        public static void StopAllMusic() // To stop all the AudioSources
        {
            AudioSource[] allAudioSources = UnityEngine.Object.FindObjectsOfType(typeof(AudioSource)) as AudioSource[];

            try
            {
                foreach (AudioSource audioS in allAudioSources)
                {
                    audioS.Stop();
                }
            }
            catch (NullReferenceException nre)
            {
                Debug.Log(nre.StackTrace);
            }
        }

        public static IEnumerator
            UpdateTime() // Iterator to update every deltaTime the value of the clips in the _audioClipTimerDictionary
            // if they exist and if an AudioSource is playing the considered clip
        {
            if (_audioSource1.isPlaying && _audioClipTimerDictionary.ContainsKey(_audioSource1.clip.name))
            {
                _audioClipTimerDictionary[_audioSource1.clip.name] += Time.deltaTime;

                // To create a loop of the songs, before reaching the length of the clip, we have to start the song all over again,
                // so we have to update _audioClipTimerDictionary but it's not enough: we have also to impose directly that the
                // AS position of the clip is at the beginning again
                if (_audioClipTimerDictionary[_audioSource1.clip.name] >=
                    Resources.Load<AudioClip>(PathFromSourcesForMusic + _audioSource1.clip.name).length -
                    0.01) // 0.01 only for superstition
                {
                    _audioClipTimerDictionary[_audioSource1.clip.name] = 0;
                }
            }

            // As above, but with the other AudioSource
            if (_audioSource2.isPlaying && _audioClipTimerDictionary.ContainsKey(_audioSource2.clip.name))
            {
                _audioClipTimerDictionary[_audioSource2.clip.name] += Time.deltaTime;

                if (_audioClipTimerDictionary[_audioSource2.clip.name] >=
                    Resources.Load<AudioClip>(PathFromSourcesForMusic + _audioSource2.clip.name).length - 0.01)
                {
                    _audioClipTimerDictionary[_audioSource2.clip.name] = 0;

                }

            }

            yield return null;
        }

        public static IEnumerator ChangeVolumes()
        {
            _finalFadeInVolume = GetSliderVolume();
            _startFadeOutVolume = _finalFadeInVolume;

            yield return null;
        }
        
    }
}