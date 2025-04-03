namespace FuzzPhyte.Game.Samples
{
    using UnityEngine;
    using UnityEngine.UI;
    using FuzzPhyte.Utility;
    using UnityEngine.Events;
    using FuzzPhyte.Tools;
    using System.Collections.Generic;

    public class FPGameManager_ToolExample : FPGenericGameUtility
    {
        [Space]
        [Header("FP Game Manager Tool Example")]
        public FPGameUIMouseListener fPGameUIMouseListener;
        public List<FP_Tool<FP_MeasureToolData>> AllMeasureTools = new List<FP_Tool<FP_MeasureToolData>>();
        public Button TheMeasureToolButton;
        public UnityEvent OnUnityGamePausedEvent;
        public UnityEvent OnUnityGameUnPausedEvent;
       
        #region Overrides
        public override void Start()
        {
            base.Start();
            //register my listeners
            if (fPGameUIMouseListener != null)
            {
                /*
                for(int i = 0; i < AllMeasureTools.Count; i++)
                {
                    var aTool = AllMeasureTools[i];
                    IFPUIEventListener<FP_Tool<FP_MeasureToolData>> castedInterface = aTool as IFPUIEventListener<FP_Tool<FP_MeasureToolData>>;
                    if (castedInterface!=null)
                    {
                        //has the interface?
                        fPGameUIMouseListener.RegisterListener(castedInterface);
                    }
                  
                }
                */
                
            }
            else
            {
                Debug.LogError($"Probably need a mouse listener for our UI Events!");
            }
        }
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
        /// <summary>
        /// Public UI function to be called from the UI Button
        /// </summary>
        /// <param name="thePassedTool">GameObject that contains FP_Tool</param>
        public void UI2DToolButtonPushed(GameObject thePassedTool)
        {
            var theTool = thePassedTool.GetComponent<FP_Tool<FP_MeasureToolData>>();
            if (theTool != null)
            {
                if (AllMeasureTools.Contains(theTool))
                {
                    //set the current tool
                    fPGameUIMouseListener.SetCurrentEngagedData(theTool);
                    fPGameUIMouseListener.SetCurrentEngagedGameObject(theTool.gameObject);
                    //activate the tool with it's own data
                    theTool.Initialize(theTool.ToolData);
                    theTool.ActivateTool();
                    //register the tool 
                    IFPUIEventListener<FP_Tool<FP_MeasureToolData>> castedInterface = theTool as IFPUIEventListener<FP_Tool<FP_MeasureToolData>>;
                    if (castedInterface != null)
                    {
                        //tell our mouse listener to register the current tool
                        fPGameUIMouseListener.RegisterListener(castedInterface);
                        //I want to tell the tool to Unregister itself with the mouse listener when it's deactivated
                        theTool.OnDeactivated += (tool) =>
                        {
                            fPGameUIMouseListener.UnregisterListener(castedInterface);
                            fPGameUIMouseListener.ResetCurrentEngagedData();
                        };
                    }

                }
                else
                {
                    Debug.LogError($"The Tool {theTool} is not in the list of tools!");
                }
            }
            else
            {
                Debug.LogError($"The GameObject you passed me, {thePassedTool.name}, does not have a FP_Tool<FP_MeasureToolData> component on it!");
            }
        }
    }
}
