using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Music
{
    public class AnimalSoundController : MonoBehaviour
    {
        private AudioSource[] _audioSources = new AudioSource[4];

        private const string AudioMixerPath = "Mixers/GameAudioMixer";
        private string _animalName = "Mallard";

        private float _flyTime = 1.5f;
        private float _spitTime = 1.5f;
        
        private const float MinTimeBetweenQuackSteal = 2f;
        private const float MaxTimeBetweenQuackSteal = 4f;
        
        private bool _isInSwimmingState;
        private bool _isInStealingState;
        private bool _isInFlyingState;
        private bool _isInSpittingState;
        private bool _isInEatingState;
        private bool _isInAnimalCall;

        private const bool IsEnemy = false;
        private string _eatSoundName;

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
        
        public void SetIsInFlyingState(bool isNewState)
        {
            _isInFlyingState = isNewState;
        }

        public bool GetIsInFlyingState()
        {
            return _isInFlyingState;
        }

        public void SetIsInSpittingState(bool isNewState)
        {
            _isInSpittingState = isNewState;
        }

        public bool GetIsInSpittingState()
        {
            return _isInSpittingState;
        }
        
        public void SetIsInEatingState(bool isStateChanged)
        {
            _isInEatingState = isStateChanged;
        }

        public bool GetIsInEatingState()
        {
            return _isInEatingState;
        }
        
        public void SetIsInAnimalCall(bool isNewState)
        {
            _isInAnimalCall = isNewState;
        }

        public bool GetIsInAnimalCall()
        {
            return _isInAnimalCall;
        }

        private void Awake()
        {
            if (_animalName.Equals("Mallard"))
            {
                _animalName = "Duck";
            }

            if (!IsEnemy)
            {
                _eatSoundName = "PlayerEating";
            }
            else
            {
                _eatSoundName = "EnemyEating";
            }
        }

        // Start is called before the first frame update
        private void Start()
        {
            for (var i = 0; i < _audioSources.Length; i++)
            {
                _audioSources[i] = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
                _audioSources[i].spatialBlend = 1;
                _audioSources[i].maxDistance = float.MaxValue;

                var audioMixer = Resources.Load(AudioMixerPath) as AudioMixer;
                if (audioMixer != null)
                {
                    var musicGroups = audioMixer.FindMatchingGroups("SoundMaster");
                    _audioSources[i].outputAudioMixerGroup = musicGroups[0];
                }

                switch (i)
                {
                    case 0:
                        _audioSources[i].clip = Resources.Load<AudioClip>("SFX/Swimming");
                        _audioSources[i].loop = true;
                        _audioSources[i].volume = MusicManagerComponent.GetSoundVolume() * (0.006f);
                        _audioSources[i].time = Random.Range(0, _audioSources[i].clip.length);
                        _audioSources[i].Play();
                        _audioSources[i].Pause();
                        break;
                    
                    case 1:
                        _audioSources[i].clip = Resources.Load<AudioClip>("SFX/Flying");
                        _audioSources[i].volume = MusicManagerComponent.GetSoundVolume();
                        break;
                    
                    case 2:
                        _audioSources[i].clip = Resources.Load<AudioClip>("SFX/SpittingSoundUp");
                        _audioSources[i].volume = MusicManagerComponent.GetSoundVolume() * (0.5f);
                        break;
                    
                    case 3:
                        _audioSources[i].clip = Resources.Load<AudioClip>("SFX/" + _eatSoundName);
                        _audioSources[i].volume = MusicManagerComponent.GetSoundVolume();
                        _audioSources[i].loop = true;
                        _audioSources[i].Play();
                        _audioSources[i].Pause();
                        break;
                }
            }
        }

        public void Swim()
        {
            if (GetIsInSwimmingState() == true) return;

            _audioSources[0].UnPause();
            SetIsInSwimmingState(true);
        }
        
        public void UnSwim()
        {
            if(GetIsInSwimmingState() == false) return;

            _audioSources[0].Pause();
            SetIsInSwimmingState(false);
        }
        
        public void Fly(float flyTime)
        {
            if (GetIsInFlyingState() == true) return;
            _audioSources[1].pitch = Resources.Load<AudioClip>("SFX/Flying").length / flyTime;
            _audioSources[1].Play();
            SetIsInFlyingState(true);
        }
        
        public void UnFly()
        {
            if(GetIsInFlyingState() == false) return;

            _audioSources[1].Stop();
            SetIsInFlyingState(false);
        }
        
        public void Spit(float maxTime)
        {
            _audioSources[2].pitch = Resources.Load<AudioClip>("SFX/SpittingSoundUp").length / maxTime;
            SetIsInSpittingState(true);
            _audioSources[2].Play();
            StartCoroutine(CheckSpitSoundClipState());
        }

        private IEnumerator CheckSpitSoundClipState()
        {
            while (GetIsInSpittingState())
            {

                if (_audioSources[2].time >= _audioSources[2].clip.length - 0.07)
                {
                    _audioSources[2].Stop();
                    SetIsInSpittingState(false);
                    UniversalAudio.PlaySound("SpitBreakSound", transform);
                    yield return null;
                }

                yield return null;
            }

            _audioSources[2].Stop();
            UniversalAudio.PlaySound("SpitBreakSound", transform);
            yield return null;
        }
        
        private IEnumerator EmitSound(string animalName, Transform thisTransform)
        {
            var random = new Unity.Mathematics.Random((uint)DateTime.Now.Ticks);
            while (GetIsInStealingState())
            {
                var numberOfClip = random.NextInt(0, MusicManagerComponent.stringAndNumberDictionary[animalName]);
                switch (numberOfClip)
                {
                    case 0:
                        UniversalAudio.PlaySound(animalName, thisTransform);
                        break;
                    case < 10:
                        UniversalAudio.PlaySound(animalName + " " + "0" + numberOfClip, thisTransform);
                        break;
                    default:
                        UniversalAudio.PlaySound(animalName + " " + numberOfClip, thisTransform);
                        break;
                }
                yield return new WaitForSeconds(random.NextFloat(MinTimeBetweenQuackSteal, MaxTimeBetweenQuackSteal));
            }

            yield return null;
        }

        public void PlayStealing(string animalName, Transform thisTransform)
        {
            SetIsInStealingState(true);
            StartCoroutine(EmitSound(animalName, thisTransform));
        }

        public void AnimalCall()
        {
            if (_isInAnimalCall)
            {
                var numberOfClip = new Unity.Mathematics.Random((uint)DateTime.Now.Ticks).NextInt(0, MusicManagerComponent.stringAndNumberDictionary[_animalName]);
                switch (numberOfClip)
                {
                    case 0:
                        UniversalAudio.PlaySound(_animalName, transform);
                        break;
                    case < 10:
                        UniversalAudio.PlaySound(_animalName + " " + "0" + numberOfClip, transform);
                        break;
                    default:
                        UniversalAudio.PlaySound(_animalName + " " + numberOfClip, transform);
                        break;
                }
            }
        }

        public void Eat()
        {
            if (GetIsInEatingState() == true) return;

            _audioSources[3].UnPause();
            SetIsInEatingState(true);
        }
        
        public void UnEat()
        {
            if(GetIsInEatingState() == false) return;

            _audioSources[3].Pause();
            SetIsInEatingState(false);
        }
        
    }
}