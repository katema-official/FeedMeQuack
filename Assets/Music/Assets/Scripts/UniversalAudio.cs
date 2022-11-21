using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Music.Assets.Scripts // To change correctly
{
    [RequireComponent(typeof(AudioSource))] // We're dealing with AudioSource. It's better to specify it 
    public class UniversalAudio : MonoBehaviour
    {
        public static UniversalAudio instance; // Variable reference for the class to use instance at the occurrence (Singleton)

        private Dictionary<string, float>
            _audioClipTimerDictionary; // Keys are clips name, Values are the seconds where
        // the clip will start

        // The more timeOfFading is, the longer the time to reach volume finalFadeVolume is
        [SerializeField] private float timeOfFading = 1f;
        
        [SerializeField] private float startFadeInVolume;
        
        [SerializeField] private float startFadeOutVolume = 1f;
        
        [SerializeField] private float finalFadeInVolume = 1f;
        
        [SerializeField] private float finalFadeOutVolume;

        private AudioSource _audioSource1, _audioSource2;
        
        private readonly string _pathFromSourcesForMusic = "Songs/"; // Inside folder "Resources", if there is a relative path, write it here
        
        private readonly string _pathFromSourcesForSound = "SFX/";

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Getter and Setter of the Serialized variables, so that we can change them from OptionsMenu through Singleton instance

        private void CreateAudioSources()
        {
            // Create the AS
            _audioSource1 = gameObject.AddComponent<AudioSource>();
            _audioSource2 = gameObject.AddComponent<AudioSource>();

            // To enable looping 
            _audioSource1.loop = true;
            _audioSource2.loop = true;
        }

        public void SetTimeOfFading(float newTime)
        {
            timeOfFading = newTime;
        }

        public float GetTimeOfFading()
        {
            return timeOfFading;
        }

        public void SetStarFadeInVolume(float newFadeInStartVolume)
        {
            startFadeInVolume = newFadeInStartVolume;
        }

        public float GetStarFadeInVolume()
        {
            return startFadeInVolume;
        }

        public void SetFinalFadeInVolume(float newFadeInFinalVolume)
        {
            finalFadeInVolume = newFadeInFinalVolume;
        }

        public float GetFinalFadeInVolume()
        {
            return finalFadeInVolume;
        }

        public void SetStartFadeOutVolume(float newFadeOutStartVolume)
        {
            startFadeOutVolume = newFadeOutStartVolume;
        }

        public float GetStartFadeOutVolume()
        {
            return startFadeOutVolume;
        }

        public void SetFinalFadeOutVolume(float newFadeOutFinalVolume)
        {
            finalFadeOutVolume = newFadeOutFinalVolume;
        }

        public float GetFinalFadeOutVolume()
        {
            return finalFadeOutVolume;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public IEnumerator
            FadeTrack(string clipName, bool fromStart) // fromStart = true if the song has to begin from the start
        {
            float timeElapsed = 0; // Initialize a counter

            if (!fromStart) // If the track has to resume from the point of interruption
            {
                if (_audioSource1.isPlaying) // If the first AudioSource is playing something...
                {
                    // It's the same of (AudioClip)Resources.Load(pathFromSourcesForMusic + clipName, typeof(AudioClip));
                    _audioSource2.clip = Resources.Load<AudioClip>(_pathFromSourcesForMusic + clipName);

                    _audioSource2.time =
                        _audioClipTimerDictionary[clipName]; // (The incoming track starts from the value
                    // displayed in _audioClipTimerDictionary)
                    _audioSource2.Play();

                    while
                        (timeElapsed <
                         timeOfFading) // ...bring its volume progressively to 0, while the second AudioSource does a Fade in
                    {
                        // Mathf.Lerp does a linear interpolation. It's the simplest way
                        _audioSource2.volume =
                            Mathf.Lerp(startFadeInVolume, finalFadeInVolume, timeElapsed / timeOfFading);
                        _audioSource1.volume = Mathf.Lerp(startFadeOutVolume, finalFadeOutVolume,
                            timeElapsed / timeOfFading);
                        timeElapsed += Time.deltaTime;
                        yield return null;
                    }

                    _audioSource1.Stop();
                }
                else // If it's the second AS that's playing, do as above but swapped
                {
                    _audioSource1.clip = Resources.Load<AudioClip>(_pathFromSourcesForMusic + clipName);
                    _audioSource1.time = _audioClipTimerDictionary[clipName];
                    _audioSource1.Play();
                    while (timeElapsed < timeOfFading)
                    {
                        _audioSource1.volume =
                            Mathf.Lerp(startFadeInVolume, finalFadeInVolume, timeElapsed / timeOfFading);
                        _audioSource2.volume = Mathf.Lerp(startFadeOutVolume, finalFadeOutVolume,
                            timeElapsed / timeOfFading);
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

        // Initialize dictionary
        public void InitDictionary()
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
        public void InitDictionary(string clipName)
        {
            _audioClipTimerDictionary[clipName] = 0;
        }

        public void PlaySound(string clipName, Transform thisTransform)
        {
            if (clipName.Equals(
                    "GameOver")) // The GameOver is treated as a Sound that obliterate all other AudioSources
            {
                StopAllMusic();
            }

            // Create a temporary AS that will die at the end of the sound 
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>(_pathFromSourcesForSound + clipName), thisTransform.position);
        }

        public void StopAllMusic() // To stop all the AS
        {
            AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];

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

        private IEnumerator
            UpdateTime() // Iterator to update every deltaTime the value of the clips in the _audioClipTimerDictionary
            // if they exist and if an AS is playing the considered clip
        {
            if (_audioSource1.isPlaying && _audioClipTimerDictionary.ContainsKey(_audioSource1.clip.name))
            {
                _audioClipTimerDictionary[_audioSource1.clip.name] += Time.deltaTime;

                // To create a loop of the songs, before reaching the length of the clip, we have to start the song all over again,
                // so we have to update _audioClipTimerDictionary but it's not enough: we have also to impose directly that the
                // AS position of the clip is at the beginning again
                if (_audioClipTimerDictionary[_audioSource1.clip.name] >=
                    Resources.Load<AudioClip>(_pathFromSourcesForMusic + _audioSource1.clip.name).length - 0.01) // 0.01 only for superstition
                {
                    _audioClipTimerDictionary[_audioSource1.clip.name] = 0;

                }
            }

            // As above, but with the other AS
            if (_audioSource2.isPlaying && _audioClipTimerDictionary.ContainsKey(_audioSource2.clip.name))
            {
                _audioClipTimerDictionary[_audioSource2.clip.name] += Time.deltaTime;

                if (_audioClipTimerDictionary[_audioSource2.clip.name] >=
                    Resources.Load<AudioClip>(_pathFromSourcesForMusic + _audioSource2.clip.name).length - 0.01)
                {
                    _audioClipTimerDictionary[_audioSource2.clip.name] = 0;

                }

            }

            yield return null;
        }

        public void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            CreateAudioSources();
            
        }

        private void Start()
        {
            InitDictionary();
        }

        private void Update()
        {
            // To keep everytime enable
            StartCoroutine(UpdateTime());

            // Just to try something
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                PlaySound("Bird", transform);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                PlaySound("Swan", transform);
                StartCoroutine(FadeTrack("Menu", false));
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                StartCoroutine(FadeTrack("Swimming", false));
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                StartCoroutine(FadeTrack("Combat", true));
            }
        }
    }
}