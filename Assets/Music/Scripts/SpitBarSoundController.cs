using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Music
{
    [RequireComponent(typeof(AudioSource))]
    public class SpitBarSoundController : MonoBehaviour
    {
        /* With methods SetIsInSpittingState(bool isNewState) we can stop Spit(float maxTime, AudioSource audioSource) call */
        
        private AudioSource _audioSource;
        private bool _isInSpittingState;

        // Start is called before the first frame update
        private void Start()
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
        }
        
        public void SetIsInSpittingState(bool isNewState)
        {
            _isInSpittingState = isNewState;
        }

        public bool GetIsInSpittingState()
        {
            return _isInSpittingState;
        }

        public void Spit(float maxTime, AudioSource audioSource)
        {
            _audioSource = audioSource;
            _audioSource.clip = Resources.Load<AudioClip>("SFX/SpittingSoundUp");
            _audioSource.volume =
                MusicManagerComponent
                    .GetSoundVolume();
            var mixer = Resources.Load("Mixers/GameAudioMixer") as AudioMixer;
            _audioSource.pitch = Resources.Load<AudioClip>("SFX/SpittingSoundUp").length / maxTime;
            if ( mixer != null)
            {
                _audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SoundMaster")[0];
            }
            SetIsInSpittingState(true);
            _audioSource.Play();
            StartCoroutine(CheckSpitSoundClipState(maxTime));
        }

        private IEnumerator CheckSpitSoundClipState(float maxTime)
        {
            //if (GetIsInSpittingState())
            //{
            /*if (!_audioSource.isPlaying)
            {
                _audioSource.clip = Resources.Load<AudioClip>("SFX/SpittingSoundUp");*/

            //}
            while (GetIsInSpittingState())
            {

                if (_audioSource.time >= _audioSource.clip.length - 0.07)
                    //_audioSource.clip.Equals(Resources.Load<AudioClip>("SFX/SpittingSoundUp")))
                {
                    _audioSource.Stop();
                    SetIsInSpittingState(false);
                    UniversalAudio.PlaySound("SpitBreakSound", transform);
                    yield return null;
                    //_audioSource.clip = Resources.Load<AudioClip>("SFX/SpittingSoundDown");
                }

                yield return null;
                /*else if (_audioSource.time >= _audioSource.clip.length - 0.07 &&
                         _audioSource.clip.Equals(Resources.Load<AudioClip>("SFX/SpittingSoundDown")))
                {
                    _audioSource.Stop();
                    _audioSource.clip = Resources.Load<AudioClip>("SFX/SpittingSoundUp");
                    _audioSource.Play();
                    yield return null;
                }*/

            }

            _audioSource.Stop();
            UniversalAudio.PlaySound("SpitBreakSound", transform);
            yield return null;
            //}
            //yield return null;
        }

    }
}