namespace FuzzPhyte.Game
{
    using UnityEngine;
    using FuzzPhyte.Utility.Analytics;
    using UnityEngine.UI;
    using TMPro;

    public class FPUI_Clock : MonoBehaviour
    {
        public FP_Stat_GameClock TheClock;
        [SerializeField] FP_StatReporter clockReporter;
        public Image ClockBackDrop;
        public Image ClockOutline;
        public TextMeshProUGUI ClockText;
       
        [SerializeField]protected float MaxGameTimeSeconds=300;
        public delegate void ClockTimerDelegate();
        public ClockTimerDelegate TimerStart;
        protected bool _timerStarted;
        public ClockTimerDelegate TimerEnd;
        public ClockTimerDelegate TenSecondsLeft;
        protected bool _timerTenFinished;
        protected bool _timerEnded;

        public virtual void SetupClock(float timer)
        {
            if (TheClock != null && !_timerStarted)
            {
                MaxGameTimeSeconds = timer;
                UpdateVisualClockInfo(MaxGameTimeSeconds);
            }
            else
            {
                //don't want to modify the clock timer if the clock is already running as this system is just comparing this value to how much time has passed on our FP_Stat_GameClock
                //this works one way if we increase the amount (make sure we are never updating it with less value than we started with) but that doesn't really make any sense
                //if you wanted to update the timer value we'd have to write in additional logic to do the math
                Debug.LogError($"The clock has already started and/or you don't have a clock!");
            }
        }

        public virtual void Start()
        {
            if(TheClock!=null){
                var curTheme = TheClock.TheReporterDetails.StatTheme;
                ClockBackDrop.color = curTheme.MainColor;
                ClockOutline.color = curTheme.SecondaryColor;
                if(curTheme.FontSettings.Count>0)
                {
                    ClockText.fontSizeMax = curTheme.FontSettings[0].MaxSize;
                    ClockText.fontSizeMin = curTheme.FontSettings[0].MinSize;
                    ClockText.font = curTheme.FontSettings[0].Font;
                    ClockText.color = curTheme.FontSettings[0].FontColor;
                    //ClockText.autoSizeTextContainer = curTheme.FontSettings[0].UseAutoSizing;
                }else{
                    //ClockText.color = curTheme.FontPrimaryColor;
                }
            }
        }
        
        public virtual void ClockRunning()
        {
            if(TheClock!=null)
            {
                if(clockReporter==null)
                {
                    clockReporter = TheClock.ReturnStatReporter();
                }else{
                    var remainTime = MaxGameTimeSeconds-TheClock.AdjustDoubleTimeSecondsForPause();
                    UpdateVisualClockInfo(remainTime);
                    //int minutes = (int)(remainTime / 60f);
                    //int seconds = (int)(remainTime % 60f);
        
                    if(remainTime<=0&&!_timerEnded){
                        ClockText.text = "00.00";
                        _timerEnded = true;
                        TheClock.EndTimer();
                        TimerEnd?.Invoke();
                    }else{
                        if(remainTime<=10&&!_timerEnded && !_timerTenFinished)
                        {
                            TenSecondsLeft?.Invoke();
                            _timerTenFinished = true;
                        }
                    }
                }
            }
        }
        public virtual void LateUpdate()
        {
            if(TheClock.RunningClock)
            {
                if(!_timerStarted)
                {
                    _timerStarted = true;
                    TimerStart?.Invoke();
                }
                ClockRunning();
            } 
        }
        protected virtual void UpdateVisualClockInfo(float timeRemain)
        {
            int minutes = (int)(timeRemain / 60f);
            int seconds = (int)(timeRemain % 60f);
            ClockText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        public virtual void PauseClock()
        {
            if(TheClock!=null)
            {
                TheClock.PauseTimer();
            }
        }
        public virtual void ResumeClock()
        {
            if(TheClock!=null)
            {
                TheClock.UnPauseTimer();
            }
        }
    }
}
