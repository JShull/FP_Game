namespace FuzzPhyte.Game
{
    using UnityEngine;
    using FuzzPhyte.Utility.FPSystem;
    using FuzzPhyte.SystemEvent;
    using FuzzPhyte.Utility;
    using System.Collections.Generic;
    using System;
    using TMPro;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using UnityEngine.Audio;
    using System.Collections;

    public abstract class FPGenericGameUtility : FPSystemBase<FP_Data>, IFPGame<FP_Data, FPEvent>
    {
        [Tooltip("Clock Reference in Scene")]
        public FPUI_Clock GameClock;
        [Header("Standard Game End Events")]
        public UnityEvent TenSecondsToGoEvent;
        public UnityEvent OnGameFinishedUnityEvent;
        public UnityEvent FadeEndEvent;
        public UnityEvent OnScoreAddedEvent;
        #region Audio Variables
        
        #region Delegates and Events
        public delegate void GameUtilityManagerDataEvent(FP_Data data);
        public delegate void GameUtilityManagerSpecialEvent(FPEvent dataEvent);
        public delegate void GameUtilityManagerEvent();
        public event GameUtilityManagerDataEvent OnGameSetupEvent;
        public event GameUtilityManagerSpecialEvent OnGameSpecialEvent;
        public event GameUtilityManagerEvent OnGameStartedEvent;
        public event GameUtilityManagerEvent OnGameFinishedEvent;
        public event GameUtilityManagerEvent OnGamePausedEvent;
        public event GameUtilityManagerEvent OnGameResumedEvent;
        public event GameUtilityManagerEvent OnGameResetEvent;
        public event GameUtilityManagerEvent OnGameMuteEvent;
        public event GameUtilityManagerEvent OnGameUnMuteEvent;
        #endregion
        protected float lastAudioMixerValue;
        [Header("Audio Related")]
        [Tooltip("Make sure the master - volume parameter is exposed! - Right Click on Volume in the Mixer Window")]
        public AudioMixer MainMasterMixer;
        [Tooltip("This is probably the master")]
        public AudioMixerGroup OverallMixerMain;
        public AudioMixerGroup UIMixer;
        public AudioMixerGroup Countdown;
        public AudioMixerGroup GameOverAudioMixerGroup;
        public List<AudioClip> RandomEndWinSounds = new List<AudioClip>();
        public AudioSource GameOverAudio;
        public AudioSource GameTenSecondsAudio;
        [Tooltip("All button clicky sounds")]
        public AudioSource UIButtonAudio;
        #endregion
        #region UI Related
        [Header("UI Related")]
        [Tooltip("The UI overlay for time/score/start etc.")]
        public Canvas OverlayDetailsCanvas;
        public Image FadePanel;
        public Button MuteButton;
        public Button UnMuteButton;
        public Slider AudioLevelSlider;
        public GameObject FinalScoreParent;
        public TextMeshProUGUI FINALScoreText;
        public TextMeshProUGUI ScoreText;
        #endregion
        #region Private Game State Variables
        [Header("Protected Game State Variables")]
        [SerializeField] protected float proximityValue;
        [SerializeField] protected float RunningScore;
        [SerializeField] protected bool pausedGame=false;
        public bool PausedGame { get => pausedGame;}
        [Header("Random Game Seed")]
        [SerializeField] protected uint mainSeed;
        protected Unity.Mathematics.Random gameRNG;
        protected bool accumulateScore;
        protected bool _gameOverStarted;
        protected bool dataInitialized;
        #endregion
        
        public override void Start()
        {
            if(GameClock!=null)
            {
                GameClock.TimerStart += OnClockStart;
                GameClock.TimerEnd += OnClockEnd;
                GameClock.TenSecondsLeft+= OnClockTenSecondsLeft;
            }
            gameRNG = new Unity.Mathematics.Random(mainSeed);
            lastAudioMixerValue=0;
            //Audio setup
            if(UIButtonAudio!=null)
            {
                UIButtonAudio.outputAudioMixerGroup=UIMixer;
            }
            if(GameOverAudio!=null)
            {
                GameOverAudio.outputAudioMixerGroup=GameOverAudioMixerGroup;
            }
            if(GameTenSecondsAudio!=null)
            {
                GameTenSecondsAudio.outputAudioMixerGroup=Countdown;
            }
        }
        
        public virtual void Update()
        {

        }
        public virtual void FixedUpdate()
        {
            if(!dataInitialized){
                return;
            }
            
            if(pausedGame)
            {
                return;
            }
            if(accumulateScore)
            {
                //score update mechanism
                RunningScore+= (float)Math.Round(proximityValue*Time.fixedDeltaTime,2);
                ScoreText.text = RunningScore.ToString("#0");
            }else
            {
                if(_gameOverStarted)
                {
                    //data loop for ending the game
                    _gameOverStarted=false;
                    dataInitialized = false;
                }
            }
        }
        public virtual void OnScoreUpdated(float score)
        {
            RunningScore += score;
            OnScoreAddedEvent?.Invoke();
        }
        public virtual void OnScoreRenderUpdate()
        {
            ScoreText.text = RunningScore.ToString("#0");
        }
        public virtual void OnClockStart()
        {
            accumulateScore=true;
        }
        public virtual void OnClockEnd()
        {
            accumulateScore=false;
            _gameOverStarted=true;
            GameOverAudio.Play();
            GameTenSecondsAudio.Stop();
            StartCoroutine(LerpTransparency());
        }
        public virtual void OnClockTenSecondsLeft()
        {
            TenSecondsToGoEvent?.Invoke();
            GameTenSecondsAudio.Play();
            //assign random audio from the RandomEndWinSounds too the AudioSource GameOverAudio
            GameOverAudio.clip = RandomEndWinSounds[gameRNG.NextInt(0,RandomEndWinSounds.Count)];
        }
        public virtual void ProcessEvent(FPEvent eventData)
        {
            OnGameSpecialEvent?.Invoke(eventData);
        }
        public virtual void PauseEngine()
        {
            pausedGame=true;
            if(GameClock!=null)
            {
                GameClock.PauseClock();
            }
            OnGamePausedEvent?.Invoke();
        }
        public virtual void ResumeEngine()
        {
            pausedGame=false;
            if(GameClock!=null)
            {
                GameClock.ResumeClock();
            }
            OnGameResumedEvent?.Invoke();
        }

        public virtual void ResetEngine()
        {
            //throw new NotImplementedException();
            OnGameResetEvent?.Invoke();
        }

        public virtual void SetupEngine(FP_Data Data)
        {
            //throw new NotImplementedException();
            OnGameSetupEvent?.Invoke(Data);
        }

        /// <summary>
        /// The start of the game
        /// </summary>
        public virtual void StartEngine()
        {
            //throw new NotImplementedException();
            OnGameStartedEvent?.Invoke();
        }

        /// <summary>
        /// The end of the game
        /// </summary>
        public virtual void StopEngine()
        {
            //throw new NotImplementedException();
            OnGameFinishedEvent?.Invoke();
            OnGameFinishedUnityEvent.Invoke();
        }
        public virtual IEnumerator LerpTransparency()
        {
            FadePanel.gameObject.SetActive(true);
           
            yield return new WaitForSecondsRealtime(2f);
            
            Color startColor = FadePanel.color;
            Color endColor = startColor;
            endColor.a = 1f; // Set the target transparency (alpha) to 1

            float elapsedTime = 0f;
            float duration = 3f; // Duration of the fade effect in seconds
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                FadePanel.color = Color.Lerp(startColor, endColor, t);
                yield return null; // Wait for the next frame
            }
            FinalScoreParent.SetActive(true);
            FINALScoreText.text = RunningScore.ToString("#0");
            // Ensure the final color is set
            FadePanel.color = endColor;
            FadeEndEvent?.Invoke();
            //any GO specific needs can be processed here
        }

        #region UI Functions
        /// <summary>
        /// Called from the UI Mute Icon
        /// </summary>
        /// <param name="mute"></param>
        public virtual void MuteAudio(bool mute)
        {
            Debug.LogWarning($"Mute Button, {OverallMixerMain.name} Mute: {mute}");
            if(mute)
            {
                MainMasterMixer.SetFloat(OverallMixerMain.name,-80);
                
                MuteButton.gameObject.SetActive(false);
                UnMuteButton.gameObject.SetActive(true);
                AudioLevelSlider.interactable=false;
                OnGameMuteEvent?.Invoke();
            }else
            {
                MainMasterMixer.SetFloat(OverallMixerMain.name,lastAudioMixerValue);
                //MainMasterMixer.SetFloat("WindmillMasterAudio",lastAudioMixerValue);
                MuteButton.gameObject.SetActive(true);
                UnMuteButton.gameObject.SetActive(false);
                AudioLevelSlider.interactable=true;
                //reset the slider to the last value based on decibels
                AudioLevelSlider.value = Mathf.InverseLerp(-80,0,lastAudioMixerValue);
                OnGameUnMuteEvent?.Invoke();
            }
        }
        public virtual void SliderAudioLevel(float level)
        {
            //map decibels to the audio mixer from a slider that's 0-1
            var mappedValue = Mathf.Lerp(-80,0,level);
            lastAudioMixerValue = mappedValue;
            MainMasterMixer.SetFloat(OverallMixerMain.name,lastAudioMixerValue);
            //MainMasterMixer.SetFloat("WindmillMasterAudio",lastAudioMixerValue);
        }
        /// <summary>
        /// Play some sound triggered by UI button event
        /// </summary>
        /// <param name="clip"></param>
        public virtual void UIButtonClicked(AudioClip clip)
        {
            if(UIButtonAudio!=null)
            {
                UIButtonAudio.clip = clip;
                UIButtonAudio.Play();
            }
        }
        #endregion
    }
}
