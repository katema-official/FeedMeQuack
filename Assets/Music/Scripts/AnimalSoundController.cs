using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Music
{
    [RequireComponent(typeof(AudioSource))]
    public class AnimalSoundController : MonoBehaviour
    {
        /* With methods SetIsInSwimmingState(bool isStateChanged), SetIsInStealingState(bool isStateChanged) and SetIsInEatingState(bool isStateChanged), we can
         choose if Swim(AudioSource audioSource) should play the swimming sound */
        private AudioSource _audioSource;

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

        public bool GetIsInSwimmingState()
        {
            return _isInSwimmingState;
        }
        
        // Start is called before the first frame update
        private void Start()
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
        }

        // Update is called once per frame
        private void Update()
        {
            /*if (new Unity.Mathematics.Random((uint)DateTime.Now.Ticks).NextFloat(0, MaxNumber) >= MinNumber)
            {
                SetIsInStealingState(true);
            }*/
            if (Input.GetKeyDown(KeyCode.Q))
            {
                UniversalAudio.PlaySound("Flying", transform);
            }
            
        }

        public void Swim(AudioSource audioSource)
        {
            _audioSource = audioSource;
            _audioSource.clip = Resources.Load<AudioClip>("SFX/Swimming");
            _audioSource.spatialBlend = 1;
            _audioSource.maxDistance =
                float.MaxValue; // Just to be sure that we can hear all the sounds at the same volume
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
            _audioSource.Play();
            _audioSource.Pause();
            SetIsInSwimmingState(true);
            StartCoroutine(CheckSwimmingState());
        }
        
        private IEnumerator CheckSwimmingState()
        {
            if (GetIsInSwimmingState())
            {
                _audioSource.UnPause();
                
                while (GetIsInSwimmingState())
                {
                    yield return null;
                }
                
                _audioSource.Pause();
                yield return null;
            }
            yield return null;
        }
    }
}