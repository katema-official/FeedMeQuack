using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Music // To change correctly
{
    public static class UniversalAudio
    {
        //[RequireComponent(typeof(AudioSource))] // We're dealing with AudioSource. It's better to specify it 
        public class UniversalAudioMonoBehaviour : MonoBehaviour // To give MonoBehaviour to a static class
        {
        }

        private static UniversalAudioMonoBehaviour
            _universalAudioMonoBehaviour; // Variable reference for the class to use instance at the occurrence

        private static Dictionary<string, float>
            _audioClipTimerDictionary; // Keys are clips name, Values are the seconds where the clip will start

        // The more timeOfFading is, the longer the time to reach volume finalFadeVolume is
        private static float _timeOfFading;

        private static float _startFadeInVolume;

        // _startFadeOutVolume and _finalFadeInVolume are essentially the max volume of the AudioListener (1 is 100% of the
        // AudioListener volume intensity). So, in the options these values will be set as the component value of the Slider 

        private static float _finalFadeInVolume =
            PlayerPrefs.GetFloat("MusicVolume", MusicManagerComponent.GetAudioSourceVolume());

        private static float _startFadeOutVolume = _finalFadeInVolume;

        private static float _finalFadeOutVolume;

        private static AudioSource _audioSource1, _audioSource2;

        private const string
            PathFromSourcesForMusic = "Songs/"; // Inside folder "Resources", if there is a relative path, write it here

        private const string
            PathFromSourcesForSound = "SFX/"; // Inside folder "Resources", if there is a relative path, write it here

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Main initializations 

        //Now Initialize the variable (the instance _universalAudioMonoBehaviour)
        private static void Init(GameObject musicManagerGo)
        {
            // If the instance not exists the first time we call the static class
            if (_universalAudioMonoBehaviour == null)
            {

                //Add this script to the object
                _universalAudioMonoBehaviour = musicManagerGo.AddComponent<UniversalAudioMonoBehaviour>();
            }
        }

        private static void GetAudioSources(GameObject musicManager)
        {
            // Create the AudioSources if they not exist
            if (_audioSource1 == null)
            {
                GameObject gameObject = musicManager.transform.Find("AudioSource1").gameObject;

                //Add this script to the object
                _audioSource1 = gameObject.GetComponent<AudioSource>();
            }

            if (_audioSource2 == null)
            {
                GameObject gameObject = musicManager.transform.Find("AudioSource2").gameObject;

                _audioSource2 = gameObject.GetComponent<AudioSource>();
            }

            // To enable looping 
            _audioSource1.loop = true;
            _audioSource2.loop = true;
        }

        // Initialize dictionary
        private static void InitDictionary()
        {
            if (_audioClipTimerDictionary == null) // If not already initialized
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

        //Now, a simple function to initialize in one shot all the main stuff
        public static void InitAllCoroutine(GameObject musicManagerGo)
        {
            // Call the initializers 
            Init(musicManagerGo);
            GetAudioSources(musicManagerGo);
            InitDictionary();
            SetTrueTimeOfFading();
            SetRightVolumes();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Getter and Setter of the variables
        private static void SetTrueTimeOfFading()
        {
            _timeOfFading = SceneManager.GetActiveScene().buildIndex == 4 ? 0 : 1;
        }

        public static float GetTimeOfFading()
        {
            return _timeOfFading;
        }

        public static float GetStarFadeInVolume()
        {
            return _startFadeInVolume;
        }

        private static void SetRightVolumes()
        {
            _finalFadeInVolume = _finalFadeInVolume.Equals(MusicManagerComponent.GetAudioSourceVolume())
                ? _finalFadeInVolume
                : MusicManagerComponent.GetAudioSourceVolume();
            _startFadeOutVolume = _finalFadeInVolume;
        }

        public static float GetFinalFadeInVolume()
        {
            return _finalFadeInVolume;
        }

        public static float GetStartFadeOutVolume()
        {
            return _startFadeOutVolume;
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
            SetRightVolumes();

            float timeElapsed = 0; // Initialize a counter

            if (!fromStart) // If the track has to resume from the point of interruption
            {
                if (_audioSource1.isPlaying &&
                    !_audioSource2.IsUnityNull()) // If the first AudioSource is playing something...
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

                    _audioSource2.volume = _finalFadeInVolume;
                    _audioSource1.volume = _finalFadeOutVolume;
                    _audioClipTimerDictionary[_audioSource1.clip.name] = _audioSource1.time;
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

                    _audioSource1.volume = _finalFadeInVolume;
                    _audioSource2.volume = _finalFadeOutVolume;
                    _audioClipTimerDictionary[_audioSource2.clip.name] = _audioSource2.time;
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

        public static void PlayMusic(string songName, bool fromStart)
        {
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
            UniversalPlayClipAtPoint(Resources.Load<AudioClip>(PathFromSourcesForSound + clipName),
                thisTransform.position);
        }

        public static void StopAllMusic() // To stop all the AudioSources
        {
            var allAudioSources = (AudioSource[])Object.FindObjectsOfType(typeof(AudioSource)); // casting as Object.FindObjectsOfType(typeof(AudioSource)) as AudioSource[] is much faster

            if (allAudioSources != null)
            {
                foreach (AudioSource audioS in allAudioSources)
                {
                    audioS.Stop();
                }
            }

        }

        private static void UniversalPlayClipAtPoint
            (AudioClip clip, Vector3 position, string audioMixerPath = "Mixers/GameAudioMixer")
        {
            var gameObject = new GameObject("One shot audio");
            gameObject.transform.position = position;
            var audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
            var audioMixer = Resources.Load(audioMixerPath) as AudioMixer;
            if (audioMixer != null)
            {
                AudioMixerGroup[] musicGroups = audioMixer.FindMatchingGroups("SoundMaster");
                audioSource.outputAudioMixerGroup = musicGroups[0];
            }

            audioSource.clip = clip;
            audioSource.spatialBlend = 1;
            audioSource.volume = MusicManagerComponent.GetSoundVolume();
            audioSource.Play();
            Object.Destroy(gameObject, clip.length *
                                       (Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
        }

        private static IEnumerator Stealing(string robber, string robbed, Transform thisTransform)
        {
            var random = new Unity.Mathematics.Random((uint)DateTime.Now.Ticks);
            var numberRobber = random.NextInt(0, 18);
            switch (numberRobber)
            {
                case 0:
                    PlaySound(robber, thisTransform);
                    break;
                case < 10:
                    PlaySound(robber + " " + "0" + numberRobber, thisTransform);
                    break;
                default:
                    PlaySound(robber + " " + numberRobber, thisTransform);
                    break;
            }

            var numberRobbed = random.NextInt(0, 18);

            yield return new WaitForSecondsRealtime(random.NextFloat(0.5f, 3f));

            switch (numberRobbed)
            {
                case 0:
                    PlaySound(robbed, thisTransform);
                    break;
                case < 10:
                    PlaySound(robbed + " " + "0" + numberRobbed, thisTransform);
                    break;
                default:
                    PlaySound(robbed + " " + numberRobbed, thisTransform);
                    break;
            }

            yield return new WaitForSeconds(random.NextFloat(0.5f, 3f));
        }

        public static void PlayStealing(string robber, string robbed, Transform thisTransform)
        {
            _universalAudioMonoBehaviour.StartCoroutine(Stealing(robber, robbed, thisTransform));
        }

    }
}