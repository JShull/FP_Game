namespace FuzzPhyte.Game.Samples
{
    using UnityEngine;
    using UnityEngine.UI;
    using FuzzPhyte.Utility;
    using UnityEngine.Events;
    using FuzzPhyte.Tools;
    using System.Collections.Generic;
    using FuzzPhyte.Tools.Connections;
    using FuzzPhyte.UI;

    public class FPGameManager_ToolExample : FPGenericGameUtility
    {
        [Space]
        [Header("FP Game Manager Tool Example")]
        public FPGameUIMouseListener fPGameUIMouseListenerLeftScreen;
        public FPGameUIMouseListener fPGameUIMouseListenerRightScreen;
        public FPGameUIMouseListenerMove fPGameUIMouseListenerPartsMove;
        public List<FP_Tool<FP_MeasureToolData>> AllMeasureToolsLeft = new List<FP_Tool<FP_MeasureToolData>>();
        public List<FP_Tool<FP_MeasureToolData>> AllMeasureToolsRight = new List<FP_Tool<FP_MeasureToolData>>();
        public List<FP_Tool<PartData>> AllPartsToolRight = new List<FP_Tool<PartData>>();
        public Button TheMeasureTool2DButton;
        public Button TheMeasureTool3DButton;
        public Button TheMovePanToolButton;
        public UnityEvent OnUnityGamePausedEvent;
        public UnityEvent OnUnityGameUnPausedEvent;
       
        #region Overrides
        public override void Start()
        {
            base.Start();
            
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
           
            if (TheMeasureTool2DButton != null)
            {
               TheMeasureTool2DButton.interactable = true;
            }
            if(TheMeasureTool3DButton!=null)
            {
                TheMeasureTool3DButton.interactable = true;
            }
            if(TheMovePanToolButton!=null)
            {
                TheMovePanToolButton.interactable=true;
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
            if (TheMeasureTool2DButton != null)
            {
                TheMeasureTool2DButton.interactable = false;
            }
            if(TheMeasureTool3DButton!=null)
            {
                TheMeasureTool3DButton.interactable = false;
            }
            if(TheMovePanToolButton!=null)
            {
                TheMovePanToolButton.interactable=false;
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
                if (AllMeasureToolsLeft.Contains(theTool))
                {
                    //set the current tool
                    fPGameUIMouseListenerLeftScreen.SetCurrentEngagedData(theTool);
                    fPGameUIMouseListenerLeftScreen.SetCurrentEngagedGameObject(theTool.gameObject);
                    //activate the tool with it's own data
                    theTool.Initialize(theTool.ToolData);
                    theTool.ActivateTool();
                    //register the tool 
                    IFPUIEventListener<FP_Tool<FP_MeasureToolData>> castedInterface = theTool as IFPUIEventListener<FP_Tool<FP_MeasureToolData>>;
                    if (castedInterface != null)
                    {
                        //tell our mouse listener to register the current tool
                        fPGameUIMouseListenerLeftScreen.RegisterListener(castedInterface);
                        //I want to tell the tool to Unregister itself with the mouse listener when it's deactivated
                        theTool.OnDeactivated += (tool) =>
                        {
                            fPGameUIMouseListenerLeftScreen.UnregisterListener(castedInterface);
                            fPGameUIMouseListenerLeftScreen.ResetCurrentEngagedData();
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
        /// <summary>
        /// Public UI function to be called from the UI Button
        /// </summary>
        /// <param name="thePassedTool">GameObject that contains FP_Tool</param>
        public void UI3DToolButtonPushed(GameObject thePassedTool)
        {
            var theTool = thePassedTool.GetComponent<FP_Tool<FP_MeasureToolData>>();
            if (theTool != null)
            {
                if (AllMeasureToolsRight.Contains(theTool))
                {
                    //set the current tool
                    fPGameUIMouseListenerRightScreen.SetCurrentEngagedData(theTool);
                    fPGameUIMouseListenerRightScreen.SetCurrentEngagedGameObject(theTool.gameObject);
                    //activate the tool with it's own data
                    theTool.Initialize(theTool.ToolData);
                    theTool.ActivateTool();
                    //register the tool 
                    IFPUIEventListener<FP_Tool<FP_MeasureToolData>> castedInterface = theTool as IFPUIEventListener<FP_Tool<FP_MeasureToolData>>;
                    if (castedInterface != null)
                    {
                        //tell our mouse listener to register the current tool
                        fPGameUIMouseListenerRightScreen.RegisterListener(castedInterface);
                        //I want to tell the tool to Unregister itself with the mouse listener when it's deactivated
                        theTool.OnDeactivated += (tool) =>
                        {
                            fPGameUIMouseListenerRightScreen.UnregisterListener(castedInterface);
                            fPGameUIMouseListenerRightScreen.ResetCurrentEngagedData();
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
        /// <summary>
        /// Called from the UI Button to activate the Move/Pan Tool
        /// </summary>
        /// <param name="thePassedTool"></param>
        public void UIMovePanToolButtonPushed(GameObject thePassedTool)
        {
            var theTool = thePassedTool.GetComponent<FP_Tool<PartData>>();
            if (theTool != null)
            {
                if (AllPartsToolRight.Contains(theTool))
                {
                    //set the current tool
                    fPGameUIMouseListenerPartsMove.SetCurrentEngagedData(theTool);
                    fPGameUIMouseListenerPartsMove.SetCurrentEngagedGameObject(theTool.gameObject);
                    //activate the tool with it's own data
                    theTool.Initialize(theTool.ToolData);
                    theTool.ActivateTool();
                    //register the tool 
                    IFPUIEventListener<FP_Tool<PartData>> castedInterface = theTool as IFPUIEventListener<FP_Tool<PartData>>;
                    if (castedInterface != null)
                    {
                        //tell our mouse listener to register the current tool
                        fPGameUIMouseListenerPartsMove.RegisterListener(castedInterface);
                        //I want to tell the tool to Unregister itself with the mouse listener when it's deactivated
                        theTool.OnDeactivated += (tool) =>
                        {
                            fPGameUIMouseListenerPartsMove.UnregisterListener(castedInterface);
                            fPGameUIMouseListenerPartsMove.ResetCurrentEngagedData();
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
