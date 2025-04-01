namespace FuzzPhyte.Game.Samples
{
    using UnityEngine;
    using UnityEngine.UI;
    
    using UnityEngine.Events;

    public class FPGameManager_ToolExample : FPGenericGameUtility
    {
        [Space]
        [Header("FP Game Manager Tool Example")]
        public Button TheMeasureToolButton;
        public UnityEvent OnUnityGamePausedEvent;
        public UnityEvent OnUnityGameUnPausedEvent;


        #region Overrides
        public override void FixedUpdate()
        {
            if(!dataInitialized)
            {
                return;
            }
            if(pausedGame)
            {
                return;
            }
            if(accumulateScore)
            {
                //score time multiplier can be applied here    
            }
            else
            {
                if(_gameOverStarted)
                {
                    //data loop for ending the game
                    _gameOverStarted=false;
                    dataInitialized = false;
                }
            }
        }
        /// <summary>
        /// We want to setup some custom things for our Tool Example that extends the base game manager
        /// </summary>
        public override void StartEngine()
        {
            GameClock.TheClock.StartClockReporter();
            base.StartEngine();
           
            if (TheMeasureToolButton != null)
            {
               TheMeasureToolButton.interactable = true;
            }
            // we want to stop make sure our overview UI isn't blocking our Tools requirements (OnDrag etc)
        }
        public override void OnClockEnd()
        {
            base.OnClockEnd();
            StopEngine();
        }
        /// <summary>
        /// Override the stop engine to disable our tool button
        /// </summary>
        public override void StopEngine()
        {
            base.StopEngine();
            if (TheMeasureToolButton != null)
            {
                TheMeasureToolButton.interactable = false;
            }
        }
        public override void PauseEngine()
        {
            base.PauseEngine();
            OnUnityGamePausedEvent?.Invoke();
        }
        public override void ResumeEngine()
        {
            base.ResumeEngine();
            OnUnityGameUnPausedEvent?.Invoke();
        }
        #endregion
    }
}
