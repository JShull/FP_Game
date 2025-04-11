namespace FuzzPhyte.Game.Samples
{
    using FuzzPhyte.Tools;
    using FuzzPhyte.Utility.FPSystem;
    using FuzzPhyte.Tools.Samples;
    using FuzzPhyte.Tools.Connections;

    public class FPGameUIMouseListenerDetach : FP_UIInputListener<FP_Tool<FP_DetachToolData>>
    {
        /// <summary>
        /// Called from a UI Item like a Button
        /// </summary>
        /// <param name="theTool"></param>
        public void UI_ToolActivated(FP_Tool<FP_DetachToolData> theTool)
        {
            SetCurrentEngagedData(theTool);
            SetCurrentEngagedGameObject(theTool.gameObject);
        }
    }
}
