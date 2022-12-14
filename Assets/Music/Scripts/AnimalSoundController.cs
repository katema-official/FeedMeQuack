using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Music
{
    [RequireComponent(typeof(AudioSource))]
    public class AnimalSoundController : MonoBehaviour
    {
        private AudioSource _audioSource;
        [SerializeField] private string animalName;
        
        private const float MaxNumber = 150;
        private const float MinNumber = 10;
        
        private bool _isInSwimmingState;
        private bool _isInStealingState;
        
        public void SetIsInSwimmingState(bool isStateChanged)
        {
            _isInSwimmingState = isStateChanged;
        }

        public void SetIsInStealingState(bool isStateChanged)
        {
            _isInStealingState = isStateChanged;
        }

        public bool GetIsInStealingState()
        {
            return _isInStealingState;
        }

        private bool GetIsInSwimmingState()
        {
            return _isInSwimmingState;
        }
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = Resources.Load<AudioClip>("SFX/Swimming");
            _audioSource.spatialBlend = 1;
            _audioSource.maxDistance =
                10000000000; // Just to be sure that we can hear all the sounds at the same volume
            _audioSource.volume =
                MusicManagerComponent
                    .GetSoundVolume();
            _audioSource.loop = true;
            _audioSource.time = Random.Range(0, _audioSource.clip.length);
            var mixer = Resources.Load("Mixers/GameAudioMixer") as AudioMixer;
            if ( mixer != null)
            {
                _audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SoundMaster")[0];
            }
        }

        // Start is called before the first frame update
        private void Start()
        {
            _audioSource.Play();
            _audioSource.Pause();
        }

        // Update is called once per frame
        private void Update()
        {
            /*if (new Unity.Mathematics.Random((uint)DateTime.Now.Ticks).NextFloat(0, MaxNumber) >= MinNumber)
            {
                SetIsInStealingState(true);
                SetIsInStealingState(false);
            }*/

            if (GetIsInStealingState())
            {
                UniversalAudio.PlayStealing(animalName, transform);
            }

            if (GetIsInSwimmingState())
            {
                _audioSource.UnPause();
            }
            else
            {
                _audioSource.Pause();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) ||
                Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) && !_audioSource.isPlaying)
            {
                SetIsInSwimmingState(true);
                //SetIsInStealingState(true);
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) ||
                     Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) && _audioSource.isPlaying)
            {
                SetIsInSwimmingState(false);
                //SetIsInStealingState(false);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                UniversalAudio.PlaySound("Flying", gameObject.transform);
            }
            
        }
    }
}