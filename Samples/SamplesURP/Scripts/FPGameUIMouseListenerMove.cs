namespace FuzzPhyte.Game.Samples
{
    using FuzzPhyte.Tools;
    using FuzzPhyte.Utility.FPSystem;
    using FuzzPhyte.Tools.Connections;
    public class FPGameUIMouseListenerMove : FP_UIInputListener<FP_Tool<PartData>>
    {
        /// <summary>
        /// Called from a UI Item like a Button
        /// </summary>
        /// <param name="theTool"></param>
        public void UI_ToolActivated(FP_Tool<PartData> theTool)
        {
            SetCurrentEngagedData(theTool);
            SetCurrentEngagedGameObject(theTool.gameObject);
        }
    }
}
