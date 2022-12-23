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

        private bool _isInSwimmingState = false;
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
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = Resources.Load<AudioClip>("SFX/Swimming");
            _audioSource.spatialBlend = 0;
            //_audioSource.maxDistance =
                //float.MaxValue; // Just to be sure that we can hear all the sounds at the same volume
            _audioSource.volume =
                MusicManagerComponent
                    .GetSoundVolume() * (0.006f);
            _audioSource.loop = true;
            _audioSource.time = Random.Range(0, _audioSource.clip.length);
            
            var mixer = Resources.Load("Mixers/GameAudioMixer") as AudioMixer;
            if (mixer != null)
            {
                _audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SoundMaster")[0];
            }
            _audioSource.Play();
            _audioSource.Pause();
        }

        // Update is called once per frame
        private void Update()
        {
            //_audioSource.transform.position = transform.position;
            /*if (new Unity.Mathematics.Random((uint)DateTime.Now.Ticks).NextFloat(0, MaxNumber) >= MinNumber)
            {
                SetIsInStealingState(true);
            }*/
            /*if (Input.GetKeyDown(KeyCode.Q))
            {
                UniversalAudio.PlaySound("Flying", transform);
            }*/

        }

        public void Swim()
        {
            if (GetIsInSwimmingState() == true) return;

            _audioSource.UnPause();
            SetIsInSwimmingState(true);
        }
        
        public void UnSwim()
        {
            if(GetIsInSwimmingState() == false) return;

            _audioSource.Pause();
            SetIsInSwimmingState(false);
        }
    }
}