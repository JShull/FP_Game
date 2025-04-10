using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzPhyte.Game
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class FP_BaseFootstep : MonoBehaviour, IFPGame<FP_FootstepData, FootstepEvent>
    {
        [SerializeField]
        protected FP_FootstepData footstepData;
        protected AudioSource audioSource;
        protected bool _setup;
        protected bool _footstepsEnabled;
        public float FootstepInterval = 0.5f;
        protected float _lastTimePlayedStep = 0f;

        public virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if(footstepData!= null)
            {
                SetupEngine(footstepData);
            }
            else
            {
                _setup = false;
            }
        }

        public virtual void ProcessEvent(FootstepEvent eventData)
        {
            if (footstepData.GroundType == eventData.GroundType && eventData.IsOlderThan(FootstepInterval, _lastTimePlayedStep))
            {
                PlaySound();
            }
        }
        public virtual void SetupEngine(FP_FootstepData Data)
        {
            audioSource.volume = Data.volume;
            audioSource.pitch = Data.pitch;
            audioSource.spatialBlend = Data.spatialBlend;
            _setup = true;
        }

        public virtual void StartEngine()
        {
            _footstepsEnabled = true;
        }

        protected virtual void PlaySound()
        {
            if (_setup && _footstepsEnabled)
            {
                audioSource.clip = footstepData.GetRandomClip();
                audioSource.Play();
                _lastTimePlayedStep= Time.realtimeSinceStartup;
            }
        }

        public void ResetEngine()
        {
            _footstepsEnabled = true;
        }
        public void StopEngine()
        {
            _footstepsEnabled = false;
        }
        protected virtual void OnEnable()
        {
            FP_FootstepEventManager.Instance.RegisterListener(this);
        }

        protected virtual void OnDisable()
        {
            FP_FootstepEventManager.Instance.UnregisterListener(this);
        }

        public void PauseEngine()
        {
            //throw new System.NotImplementedException();
            _footstepsEnabled= false;
        }

        public void ResumeEngine()
        {
            //throw new System.NotImplementedException();
            _footstepsEnabled = true;
        }
        
    }
}
