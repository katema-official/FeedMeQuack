using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Music
{
    public class EatingController : MonoBehaviour
    {
        private AudioSource _audioSource;
        private bool _isInEatingState;

        private void Start()
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
        }
        
        public void SetIsInEatingState(bool isStateChanged)
        {
            _isInEatingState = isStateChanged;
        }

        public bool GetIsInEatingState()
        {
            return _isInEatingState;
        }


        public void Eat(AudioSource audioSource)
        {
            _audioSource = audioSource;
            _audioSource.clip = Resources.Load<AudioClip>("SFX/PlayerEating");
            _audioSource.spatialBlend = 1;
            _audioSource.maxDistance =
                float.MaxValue; // Just to be sure that we can hear all the sounds at the same volume
            _audioSource.volume =
                MusicManagerComponent
                    .GetSoundVolume();
            _audioSource.loop = true;
            var mixer = Resources.Load("Mixers/GameAudioMixer") as AudioMixer;
            if (mixer != null)
            {
                _audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SoundMaster")[0];
            }
            
            _audioSource.Play();
            _audioSource.Pause();
            SetIsInEatingState(true);
            StartCoroutine(CheckEatingState());
        }

        private IEnumerator CheckEatingState()
        {
            if (GetIsInEatingState())
            {
                _audioSource.UnPause();

                while (GetIsInEatingState())
                {
                    yield return null;
                }

                _audioSource.Stop();
                yield return null;
            }

            yield return null;
        }

    }
}