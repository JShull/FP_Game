namespace FuzzPhyte.Game.Samples
{
    using FuzzPhyte.Tools;
    using FuzzPhyte.Utility.FPSystem;
    using FuzzPhyte.Tools.Samples;

    /// <summary>
    /// This class will work for any "Measure" tool that extends the FP_Tool class with a FP_MeasureToolData
    /// We need this as part of our rect transform that's basically now listening to these UI events
    /// </summary>
    public class FPGameUIMouseListener : FP_UIInputListener<FP_Tool<FP_MeasureToolData>>
    {
        /// <summary>
        /// Called from a UI Item like a Button
        /// </summary>
        /// <param name="theTool"></param>
        public void UI_ToolActivated(FP_Tool<FP_MeasureToolData> theTool)
        {
            SetCurrentEngagedData(theTool);
            SetCurrentEngagedGameObject(theTool.gameObject);
        }
    }
}
