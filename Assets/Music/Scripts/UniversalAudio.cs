using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Music // To change correctly
{
    // This script exposes static methods to implement audio. The main functions are: PlayMusic(string songName, bool fromStart),
    // PlaySound(string clipName, Transform thisTransform), PlayStealing(string animalName, Transform thisTransform)
    // and StopAllMusic(). It's the access to the methods of EatingController, AnimalSoundController and SpitBarController classes
    public static class UniversalAudio
    {
        private class UniversalAudioMonoBehaviour : MonoBehaviour // To give MonoBehaviour to a static class
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

        private static float _finalFadeInVolume = MusicManagerComponent.GetAudioSourceVolume();

        private static float _startFadeOutVolume = _finalFadeInVolume;

        private static float _finalFadeOutVolume;

        private static AudioSource _audioSource1, _audioSource2;

        public static SpitBarSoundController _spitBarSoundController;

        public static AnimalSoundController _animalSoundController;

        public static EatingController _eatingController;

        private const string
            PathFromSourcesForMusic = "Songs/"; // Inside folder "Resources", if there is a relative path, write it here

        private const string
            PathFromSourcesForSound = "SFX/"; // Inside folder "Resources", if there is a relative path, write it here

        private const float MinTimeBetweenQuackSteal = 2f;
        private const float MaxTimeBetweenQuackSteal = 4f;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Main initializations 

        //Now Initialize the variable (the instance _universalAudioMonoBehaviour)
        private static void Init(GameObject musicManagerGo)
        {
            // If the instance not exists the first time we call the static class
            //Add this script to the object
            _universalAudioMonoBehaviour ??= musicManagerGo.AddComponent<UniversalAudioMonoBehaviour>();
            _spitBarSoundController ??= musicManagerGo.AddComponent<SpitBarSoundController>();
            _animalSoundController ??= musicManagerGo.AddComponent<AnimalSoundController>();
            _eatingController ??= musicManagerGo.AddComponent<EatingController>();
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
                var keys = new List<string>(_audioClipTimerDictionary.Keys);
                foreach (var key in keys)
                {
                    InitDictionary(key);
                }
            }
        }

        // Overload to clean only the track whose name is clipName
        private static void InitDictionary(string clipName)
        {
            _audioClipTimerDictionary[clipName] = 0;
        }

        //Now, a simple function to initialize in one shot all the main stuff
        public static void InitAll(GameObject musicManagerGo)
        {
            // Call the initializers 
            Init(musicManagerGo);
            InitDictionary();
            GetAudioSources(musicManagerGo);
            SetTrueTimeOfFading();
            SetRightVolumes();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Getter and Setter of the variables

        public static SpitBarSoundController GetSpitBarSoundController()
        {
            return _spitBarSoundController;
        }
        
        public static AnimalSoundController GetAnimalSoundController()
        {
            return _animalSoundController;
        }

        private static void SetTrueTimeOfFading()
        {
            _timeOfFading = SceneManager.GetActiveScene().name == "MainMenu" ? 0 : 1;
        }

        private static void SetRightVolumes()
        {
            _finalFadeInVolume = _finalFadeInVolume.Equals(MusicManagerComponent.GetAudioSourceVolume())
                ? _finalFadeInVolume
                : MusicManagerComponent.GetAudioSourceVolume();
            _startFadeOutVolume = _finalFadeInVolume;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Now the methods/functions

        // To do the fading used in PlayMusic()...
        private static IEnumerator
            FadeTrack(string clipName, bool fromStart) // fromStart = true if the song has to begin from the start
        {
            SetTrueTimeOfFading();
            SetRightVolumes();

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

                    _audioSource2.volume = _finalFadeInVolume;
                    _audioSource1.volume = _finalFadeOutVolume;
                    _audioClipTimerDictionary[_audioSource1.clip.name] = _audioSource1.time;
                    _audioSource1.Stop();
                }
                else if (_audioSource2.isPlaying)
                {
                    // If it's the second AudioSource that's playing, do as above but swapped
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
                else
                {
                    _audioSource1.clip = Resources.Load<AudioClip>(PathFromSourcesForMusic + clipName);
                    _audioSource1.time = _audioClipTimerDictionary[clipName];
                    _audioSource1.Play();
                    while (timeElapsed < _timeOfFading)
                    {
                        _audioSource1.volume =
                            Mathf.Lerp(_startFadeInVolume, _finalFadeInVolume, timeElapsed / _timeOfFading);
                        timeElapsed += Time.deltaTime;
                        yield return null;
                    }

                    _audioSource1.volume = _finalFadeInVolume;
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

        public static void PlaySound(string clipName, Transform thisTransform, AnimalSoundController animalSoundController = null)
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
            var allAudioSources =
                Object.FindObjectsOfType(
                    typeof(AudioSource)) as AudioSource[]; //(AudioSource[])Object.FindObjectsOfType(typeof(AudioSource)); // casting as Object.FindObjectsOfType(typeof(AudioSource)) as AudioSource[] is much faster

            if (allAudioSources != null)
            {
                foreach (var audioS in allAudioSources)
                {
                    audioS.Stop();
                }
            }
        }

        private static void UniversalPlayClipAtPoint
        (AudioClip clip, Vector3 position,
            string audioMixerPath = "Mixers/GameAudioMixer")
        {
            var gameObject = new GameObject("One shot audio");
            gameObject.transform.position = position;
            var audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
            var audioMixer = Resources.Load(audioMixerPath) as AudioMixer;
            if (audioMixer != null)
            {
                var musicGroups = audioMixer.FindMatchingGroups("SoundMaster");
                audioSource.outputAudioMixerGroup = musicGroups[0];
            }

            audioSource.clip = clip;
            audioSource.spatialBlend = 1;
            audioSource.transform.position = position;
            audioSource.maxDistance =
                float.MaxValue; // Just to be sure that we can hear all the sounds at the same volume
            audioSource.volume =
                MusicManagerComponent
                    .GetSoundVolume();
            audioSource.Play();
            Object.Destroy(gameObject, clip.length *
                                       (Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));

        }


        private static IEnumerator EmitSound(string animalName, Transform thisTransform)
        {
            if (animalName.Equals("Mallard"))
            {
                animalName = "Duck";
            }
            var random = new Unity.Mathematics.Random((uint)DateTime.Now.Ticks);
            while (_animalSoundController.GetIsInStealingState())
            {
                var numberOfClip = random.NextInt(0, MusicManagerComponent.stringAndNumberDictionary[animalName]);
                switch (numberOfClip)
                {
                    case 0:
                        PlaySound(animalName, thisTransform);
                        break;
                    case < 10:
                        PlaySound(animalName + " " + "0" + numberOfClip, thisTransform);
                        break;
                    default:
                        PlaySound(animalName + " " + numberOfClip, thisTransform);
                        break;
                }
                yield return new WaitForSeconds(random.NextFloat(MinTimeBetweenQuackSteal, MaxTimeBetweenQuackSteal));
            }

            yield return null;
        }

        public static void PlayStealing(string animalName, Transform thisTransform)
        {
            _animalSoundController.SetIsInStealingState(true);
            _universalAudioMonoBehaviour.StartCoroutine(EmitSound(animalName, thisTransform));
        }
        
    }
}