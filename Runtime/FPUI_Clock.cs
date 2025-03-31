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
        public float MaxGameTimeSeconds=300;
        public delegate void ClockTimerDelegate();
        public ClockTimerDelegate TimerStart;
        private bool _timerStarted;
        public ClockTimerDelegate TimerEnd;
        public ClockTimerDelegate TenSecondsLeft;
        private bool _timerTenFinished;
        private bool _timerEnded;


        public void Start()
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
                    ClockText.color = curTheme.FontPrimaryColor;
                }
            }
        }
        public void ClockRunning()
        {
            if(TheClock!=null)
            {
                if(clockReporter==null){
                    clockReporter = TheClock.ReturnStatReporter();
                    
                }else{
                    var remainTime = MaxGameTimeSeconds-TheClock.AdjustDoubleTimeSecondsForPause();
                    int minutes = (int)(remainTime / 60f);
                    int seconds = (int)(remainTime % 60f);
        
                    if(remainTime<=0&&!_timerEnded){
                        ClockText.text = "00.00";
                        _timerEnded = true;
                        TheClock.EndTimer();
                        TimerEnd?.Invoke();
                    }else{
                        ClockText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                        if(remainTime<=10&&!_timerEnded && !_timerTenFinished)
                        {
                            TenSecondsLeft?.Invoke();
                            _timerTenFinished = true;
                        }
                    }
                }
            }
        }
        public void LateUpdate()
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
        public void PauseClock()
        {
            if(TheClock!=null)
            {
                TheClock.PauseTimer();
            }
        }
        public void ResumeClock()
        {
            if(TheClock!=null)
            {
                TheClock.UnPauseTimer();
            }
        }
    }
}
