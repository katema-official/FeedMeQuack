using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Music
{
    [RequireComponent(typeof(AudioSource))]
    public class FlySoundController : MonoBehaviour
    {
        /* With methods SetIsInSpittingState(bool isNewState) we can stop Spit(float maxTime, AudioSource audioSource) call */

        private AudioSource _audioSource;
        private bool _isInFlyingState;

        // Start is called before the first frame update
        private void Start()
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
        }

        public void SetIsInFlyingState(bool isNewState)
        {
            _isInFlyingState = isNewState;
        }

        public bool GetIsInFlyingState()
        {
            return _isInFlyingState;
        }

        public void Fly(float maxTime, AudioSource audioSource)
        {
            _audioSource = audioSource;
            _audioSource.clip = Resources.Load<AudioClip>("SFX/Flying");
            _audioSource.volume =
                MusicManagerComponent
                    .GetSoundVolume();
            var mixer = Resources.Load("Mixers/GameAudioMixer") as AudioMixer;
            _audioSource.pitch = Resources.Load<AudioClip>("SFX/Flying").length / maxTime;
            if (mixer != null)
            {
                _audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SoundMaster")[0];
            }

            SetIsInFlyingState(true);
            _audioSource.Play();
            StartCoroutine(CheckFlySoundClipState(maxTime));
        }

        private IEnumerator CheckFlySoundClipState(float maxTime)
        {
            while (GetIsInFlyingState())
            {

                if (_audioSource.time >= _audioSource.clip.length - 0.07)
                {
                    _audioSource.Stop();
                    SetIsInFlyingState(false);
                    yield return null;
                }

                yield return null;
            }
            
            _audioSource.Stop();
            yield return null;
        }
        
    }
}