using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Music
{
    [RequireComponent(typeof(AudioSource))]
    public class SpitBarSoundController : MonoBehaviour
    {
        private AudioSource _audioSource;
        private bool _isInSpittingState;
        private const float PitchToNormalizeTime = 1.5f;// Will be Resources.Load<AudioClip>("SFX/SpittingSoundUp").length / GetSpitBarTimeForCharging() 

        // Start is called before the first frame update
        private void Start()
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
            _audioSource.clip = Resources.Load<AudioClip>("SFX/SpittingSoundUp");
            _audioSource.volume =
                MusicManagerComponent
                    .GetSoundVolume();
            _audioSource.pitch = PitchToNormalizeTime;
            var mixer = Resources.Load("Mixers/GameAudioMixer") as AudioMixer;
            if ( mixer != null)
            {
                _audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SoundMaster")[0];
            }
        }

// Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SetIsInSpittingState(true);
            }
            
            if (Input.GetKeyUp(KeyCode.Space))
            {
                SetIsInSpittingState(false);
            }
            Spit();
        }

        public void SetIsInSpittingState(bool isNewState)
        {
            _isInSpittingState = isNewState;
        }

        private bool GetIsInSpittingState()
        {
            return _isInSpittingState;
        }

        private void Spit()
        {
            StartCoroutine(CheckSpitSoundClipState());
        }

        private IEnumerator CheckSpitSoundClipState()
        {
            if (GetIsInSpittingState())
            {
                if (!_audioSource.isPlaying)
                {
                    _audioSource.clip = Resources.Load<AudioClip>("SFX/SpittingSoundUp");
                    _audioSource.Play();
                }

                if (_audioSource.time >= _audioSource.clip.length - 0.07 &&
                    _audioSource.clip.Equals(Resources.Load<AudioClip>("SFX/SpittingSoundUp")))
                {
                    _audioSource.Stop();
                    _audioSource.clip = Resources.Load<AudioClip>("SFX/SpittingSoundDown");
                    _audioSource.Play();
                    yield return null;
                }
                else if (_audioSource.time >= _audioSource.clip.length - 0.07 &&
                         _audioSource.clip.Equals(Resources.Load<AudioClip>("SFX/SpittingSoundDown")))
                {
                    _audioSource.Stop();
                    _audioSource.clip = Resources.Load<AudioClip>("SFX/SpittingSoundUp");
                    _audioSource.Play();
                    yield return null;
                }
                
            }
            else if (!GetIsInSpittingState() && _audioSource.isPlaying)
            {
                _audioSource.Stop();
                UniversalAudio.PlaySound("SpitBreakSound", transform);
                yield break;
            }

            yield return null;
        }
        
    }
}