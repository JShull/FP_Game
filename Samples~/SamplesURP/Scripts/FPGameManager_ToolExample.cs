namespace FuzzPhyte.Game.Samples
{
    using UnityEngine;
    using UnityEngine.UI;
    using FuzzPhyte.Utility;
    using UnityEngine.Events;
    using FuzzPhyte.Tools;
    using System.Collections.Generic;
    using FuzzPhyte.Tools.Connections;
    using FuzzPhyte.Tools.Samples;

    public class FPGameManager_ToolExample : FPGenericGameUtility
    {
        [Space]
        [Header("FP Game Manager Tool Example")]
        protected GameObject currentGameObjectTool;
        public FPGameUIMouseListener fPGameUIMouseListenerLeftScreen;
        public FPGameUIMouseListener fPGameUIMouseListenerRightScreen;
        public FPGameUIMouseListenerMove fPGameUIMouseListenerPartsMove;
        public FPGameUIMouseListenerAttach fpGameUIMouseListenerAttachTool;
        public FPGameUIMouseListenerDetach fpGameUIMouseListenerDetachTool;
        public List<FP_Tool<FP_MeasureToolData>> AllMeasureToolsLeft = new List<FP_Tool<FP_MeasureToolData>>();
        public List<FP_Tool<FP_MeasureToolData>> AllMeasureToolsRight = new List<FP_Tool<FP_MeasureToolData>>();
        public List<FP_Tool<PartData>> AllMoveParts = new List<FP_Tool<PartData>>();
        public List<FP_Tool<FP_AttachToolData>> AllAttachTools = new List<FP_Tool<FP_AttachToolData>>();
        public List<FP_Tool<FP_DetachToolData>> AllDetachTools = new List<FP_Tool<FP_DetachToolData>>();
        public Button TheMeasureTool2DButton;
        public Button TheMeasureTool3DButton;
        public Button TheMovePanToolButton;
        public Button The2DRemoveLinesButton;
        public Button The3DRemoveLinesButton;
        public Button TheAttachButton;
        public Button TheDetachButton;
        public List<GameObject> AllButtonSelectIcons = new List<GameObject>();
        public FP_PanMove TheMoveRotateTool;
        public Slider TheForwardZPlaneSlider;
        protected float sliderLastPos;
        public int SliderMax = 6;
        //public Image ButtonSelectionIcon;
        public UnityEvent OnUnityGamePausedEvent;
        public UnityEvent OnUnityGameUnPausedEvent;
        protected bool useUnityEventsPlane;

        #region Overrides
        public override void Start()
        {
            base.Start();
            useUnityEventsPlane = false;
            TheForwardZPlaneSlider.maxValue = SliderMax;
            TheForwardZPlaneSlider.value = SliderMax * 0.5f;
            sliderLastPos = TheForwardZPlaneSlider.value;
            if (TheMoveRotateTool == null)
            {
                Debug.LogError($"Need to assign my Move Tool so I can adjust the Forward Plane");
            }
        }
        public override void FixedUpdate()
        {
            if (!dataInitialized)
            {
                return;
            }
            if (pausedGame)
            {
                return;
            }
            if (accumulateScore)
            {
                //score time multiplier can be applied here    
            }
            else
            {
                if (_gameOverStarted)
                {
                    //data loop for ending the game
                    _gameOverStarted = false;
                    dataInitialized = false;
                }
            }
        }
        /// <summary>
        /// We want to setup some custom things for our Tool Example that extends the base game manager
        /// </summary>
        public override void StartEngine()
        {
            TheMoveRotateTool.UpdateBoundsRemote(SliderMax * 0.5f);
            GameClock.TheClock.StartClockReporter();
            base.StartEngine();
            useUnityEventsPlane = true;
            TurnOffOnButtons(true);
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
            TurnOffOnButtons(false);
            ForceResetPreviousTool();
        }
        public override void PauseEngine()
        {
            base.PauseEngine();
            ResetIcons();
            foreach (var tool in AllMeasureToolsLeft)
            {
                tool.DeactivateToolFromUI();
            }
            foreach (var tool in AllMeasureToolsRight)
            {
                tool.DeactivateToolFromUI();
            }
            foreach (var tool in AllMoveParts)
            {
                tool.DeactivateToolFromUI();
            }
            foreach (var tool in AllAttachTools)
            {
                tool.DeactivateToolFromUI();
            }
            foreach (var tool in AllDetachTools)
            {
                tool.DeactivateToolFromUI();
            }
            //buttons
            TurnOffOnButtons(false);
            OnUnityGamePausedEvent?.Invoke();
        }
        public override void ResumeEngine()
        {
            base.ResumeEngine();
            //buttons
            TurnOffOnButtons(true);
            OnUnityGameUnPausedEvent?.Invoke();
        }
        protected void TurnOffOnButtons(bool status)
        {
            if (TheMeasureTool2DButton != null)
            {
                TheMeasureTool2DButton.interactable = status;
            }
            if (TheMeasureTool3DButton != null)
            {
                TheMeasureTool3DButton.interactable = status;
            }
            if (TheMovePanToolButton != null)
            {
                TheMovePanToolButton.interactable = status;
            }
            if (The2DRemoveLinesButton != null)
            {
                The2DRemoveLinesButton.interactable = status;
            }
            if(The3DRemoveLinesButton!= null)
            {
                The3DRemoveLinesButton.interactable= status;
            }
            if(TheAttachButton != null)
            {
                TheAttachButton.interactable = status;
            }
            if(TheDetachButton != null)
            {
                TheDetachButton.interactable = status;
            }
            if(TheForwardZPlaneSlider != null)
            {
                //always disable the slider
                TheForwardZPlaneSlider.interactable = false;
            }
        }
        public void ResetIcons()
        {
            foreach (var gO in AllButtonSelectIcons)
            {
                gO.SetActive(false);
            }
        }
        #endregion
        protected void ForceResetPreviousTool()
        {
            if (currentGameObjectTool != null)
            {
                //we had a previous tool and it might be in some sort of 'state' 
                //we need to deactivate it 
                IFPTool castToolInterface = currentGameObjectTool.GetComponent<IFPTool>();
                castToolInterface.ForceDeactivateTool();
            }
        }
        /// <summary>
        /// Public UI function to be called from the UI Button
        /// </summary>
        /// <param name="thePassedTool">GameObject that contains FP_Tool</param>
        public void UI2DToolButtonPushed(GameObject thePassedTool)
        {
            //check if we had a previous gameObject tool
            ForceResetPreviousTool();
            var theTool = thePassedTool.GetComponent<FP_Tool<FP_MeasureToolData>>();
            fPGameUIMouseListenerLeftScreen.ActivateListener();
            fPGameUIMouseListenerPartsMove.DeactivateListener();
            fPGameUIMouseListenerRightScreen.DeactivateListener();
            fpGameUIMouseListenerAttachTool.DeactivateListener();
            fpGameUIMouseListenerDetachTool.DeactivateListener();
            if (theTool != null)
            {
                if (AllMeasureToolsLeft.Contains(theTool))
                {
                    //set the current tool
                    currentGameObjectTool = thePassedTool;
                    fPGameUIMouseListenerLeftScreen.UI_ToolActivated(theTool);
                    theTool.Initialize(theTool.ToolData);

                    //register the tool 
                    IFPUIEventListener<FP_Tool<FP_MeasureToolData>> castedInterface = theTool as IFPUIEventListener<FP_Tool<FP_MeasureToolData>>;
                    if (castedInterface != null)
                    {
                        //tell our mouse listener to register the current tool
                        theTool.OnActivated += (tool) =>
                        {
                            fPGameUIMouseListenerLeftScreen.RegisterListener(castedInterface);
                        };
                        //I want to tell the tool to Unregister itself with the mouse listener when it's deactivated
                        theTool.OnDeactivated += (tool) =>
                        {
                            fPGameUIMouseListenerLeftScreen.UnregisterListener(castedInterface);
                            fPGameUIMouseListenerLeftScreen.ResetCurrentEngagedData();
                        };
                        //
                    }
                    theTool.ActivateTool();
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
            ForceResetPreviousTool();
            var theTool = thePassedTool.GetComponent<FP_Tool<FP_MeasureToolData>>();
            fPGameUIMouseListenerLeftScreen.DeactivateListener();
            fPGameUIMouseListenerPartsMove.DeactivateListener();
            fPGameUIMouseListenerRightScreen.ActivateListener();
            fpGameUIMouseListenerAttachTool.DeactivateListener();
            fpGameUIMouseListenerDetachTool.DeactivateListener();
            if (theTool != null)
            {
                if (AllMeasureToolsRight.Contains(theTool))
                {
                    //set the current tool
                    currentGameObjectTool = thePassedTool;
                    fPGameUIMouseListenerRightScreen.UI_ToolActivated(theTool);
                    theTool.Initialize(theTool.ToolData);

                    //register the tool 
                    IFPUIEventListener<FP_Tool<FP_MeasureToolData>> castedInterface = theTool as IFPUIEventListener<FP_Tool<FP_MeasureToolData>>;
                    if (castedInterface != null)
                    {
                        //tell our mouse listener to register the current tool
                        theTool.OnActivated += (tool) =>
                        {
                            fPGameUIMouseListenerRightScreen.RegisterListener(castedInterface);
                        };

                        //I want to tell the tool to Unregister itself with the mouse listener when it's deactivated
                        theTool.OnDeactivated += (tool) =>
                        {
                            fPGameUIMouseListenerRightScreen.UnregisterListener(castedInterface);
                            fPGameUIMouseListenerRightScreen.ResetCurrentEngagedData();
                        };
                    }
                    theTool.ActivateTool();
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
            ForceResetPreviousTool();
            var theTool = thePassedTool.GetComponent<FP_Tool<PartData>>();
            fPGameUIMouseListenerLeftScreen.DeactivateListener();
            fPGameUIMouseListenerPartsMove.ActivateListener();
            fPGameUIMouseListenerRightScreen.DeactivateListener();
            fpGameUIMouseListenerAttachTool.DeactivateListener();
            fpGameUIMouseListenerDetachTool.DeactivateListener();
            if (theTool != null)
            {
                if (AllMoveParts.Contains(theTool))
                {
                    currentGameObjectTool = thePassedTool;
                    //set the current tool
                    fPGameUIMouseListenerPartsMove.UI_ToolActivated(theTool);
                    theTool.Initialize(theTool.ToolData);
                    //register the tool 
                    IFPUIEventListener<FP_Tool<PartData>> castedInterface = theTool as IFPUIEventListener<FP_Tool<PartData>>;
                    if (castedInterface != null)
                    {
                        //tell our mouse listener to register the current tool
                        theTool.OnActivated += (tool) =>
                        {
                            fPGameUIMouseListenerPartsMove.RegisterListener(castedInterface);
                        };
                        //I want to tell the tool to Unregister itself with the mouse listener when it's deactivated
                        theTool.OnDeactivated += (tool) =>
                        {
                            fPGameUIMouseListenerPartsMove.UnregisterListener(castedInterface);
                            fPGameUIMouseListenerPartsMove.ResetCurrentEngagedData();
                        };
                    }
                    //
                    theTool.ActivateTool();
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
        /// Called from the UI button to activate the Attach/Weld Tool
        /// </summary>
        /// <param name="thePassedTool"></param>
        public void UIAttachToolButtonPushed(GameObject thePassedTool)
        {
            ForceResetPreviousTool();
            var theTool = thePassedTool.GetComponent<FP_Tool<FP_AttachToolData>>();
            fPGameUIMouseListenerLeftScreen.DeactivateListener();
            fPGameUIMouseListenerPartsMove.DeactivateListener();
            fPGameUIMouseListenerRightScreen.DeactivateListener();
            fpGameUIMouseListenerAttachTool.ActivateListener();
            fpGameUIMouseListenerDetachTool.DeactivateListener();
            if (theTool != null)
            {
                if (AllAttachTools.Contains(theTool))
                {
                    currentGameObjectTool = thePassedTool;
                    //set the current tool
                    fpGameUIMouseListenerAttachTool.UI_ToolActivated(theTool);
                    theTool.Initialize(theTool.ToolData);
                    //register the tool 
                    IFPUIEventListener<FP_Tool<FP_AttachToolData>> castedInterface = theTool as IFPUIEventListener<FP_Tool<FP_AttachToolData>>;
                    if (castedInterface != null)
                    {
                        //tell our mouse listener to register the current tool
                        theTool.OnActivated += (tool) =>
                        {
                            fpGameUIMouseListenerAttachTool.RegisterListener(castedInterface);
                        };
                        //I want to tell the tool to Unregister itself with the mouse listener when it's deactivated
                        theTool.OnDeactivated += (tool) =>
                        {
                            fpGameUIMouseListenerAttachTool.UnregisterListener(castedInterface);
                            fpGameUIMouseListenerAttachTool.ResetCurrentEngagedData();
                        };
                    }
                    //
                    theTool.ActivateTool();
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
        /// Called from the UI Button to activate the detach/grind tool
        /// </summary>
        /// <param name="thePassedTool"></param>
        public void UIDetachToolButtonPushed(GameObject thePassedTool)
        {
            ForceResetPreviousTool();
            var theTool = thePassedTool.GetComponent<FP_Tool<FP_DetachToolData>>();
            fPGameUIMouseListenerLeftScreen.DeactivateListener();
            fPGameUIMouseListenerPartsMove.DeactivateListener();
            fPGameUIMouseListenerRightScreen.DeactivateListener();
            fpGameUIMouseListenerAttachTool.DeactivateListener();
            fpGameUIMouseListenerDetachTool.ActivateListener();
            if (theTool != null)
            {
                if (AllDetachTools.Contains(theTool))
                {
                    currentGameObjectTool = thePassedTool;
                    //set the current tool
                    fpGameUIMouseListenerDetachTool.UI_ToolActivated(theTool);
                    theTool.Initialize(theTool.ToolData);
                    //register the tool 
                    IFPUIEventListener<FP_Tool<FP_DetachToolData>> castedInterface = theTool as IFPUIEventListener<FP_Tool<FP_DetachToolData>>;
                    if (castedInterface != null)
                    {
                        //tell our mouse listener to register the current tool
                        theTool.OnActivated += (tool) =>
                        {
                            fpGameUIMouseListenerDetachTool.RegisterListener(castedInterface);
                        };
                        //I want to tell the tool to Unregister itself with the mouse listener when it's deactivated
                        theTool.OnDeactivated += (tool) =>
                        {
                            fpGameUIMouseListenerDetachTool.UnregisterListener(castedInterface);
                            fpGameUIMouseListenerDetachTool.ResetCurrentEngagedData();
                        };
                    }
                    //
                    theTool.ActivateTool();
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
        public void UISliderZPlaneChange(float sliderValue)
        {
            var curSliderValue = sliderValue;
            if (curSliderValue > sliderLastPos)
            {
                //going up or moving the Z Plane forward on the Z
                TheMoveRotateTool.UpdateForwardPlaneLocation(1, true, useUnityEventsPlane);
            }
            else
            {
                TheMoveRotateTool.UpdateForwardPlaneLocation(-1, true, useUnityEventsPlane);
            }
            sliderLastPos = curSliderValue;
        }
        public void UI2DToolRemoveLines(GameObject thePassedTool)
        {
            var theTool = thePassedTool.GetComponent<FP_Tool<FP_MeasureToolData>>();
            if (theTool != null)
            {
                if (AllMeasureToolsLeft.Contains(theTool))
                {
                    //fPGameUIMouseListenerLeftScreen.UI_ToolActivated(theTool);
                    IFPUIEventListener<FP_Tool<FP_MeasureToolData>> castedInterface = theTool as IFPUIEventListener<FP_Tool<FP_MeasureToolData>>;
                    castedInterface.ResetVisuals();
                    PauseEngine();
                    ResumeEngine();
                } 
            }
        }
        public void UI3DToolRemoveLines(GameObject thePassedTool)
        {
            var theTool = thePassedTool.GetComponent<FP_Tool<FP_MeasureToolData>>();
            if (theTool != null)
            {
                if (AllMeasureToolsRight.Contains(theTool))
                {
                    //fPGameUIMouseListenerLeftScreen.UI_ToolActivated(theTool);
                    IFPUIEventListener<FP_Tool<FP_MeasureToolData>> castedInterface = theTool as IFPUIEventListener<FP_Tool<FP_MeasureToolData>>;
                    castedInterface.ResetVisuals();
                    PauseEngine();
                    ResumeEngine();
                }
            }
        }
    }
}
