namespace FuzzPhyte.Game.Samples
{
    using System.Linq;
    using FuzzPhyte.Utility.FPSystem;
    using UnityEngine;
    public class FPGame_BootStrap_ToolExample:FPBootStrapper<FPGameManagerExampleData>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static new void InitializeAfterAwake()
        {
            Debug.LogWarning($"Running the FPGame Bootstrap Example! {Time.time} and {Time.frameCount}");
            var gameManagerSystems = Object.FindObjectsByType<FPGameManager_ToolExample>(FindObjectsSortMode.None).ToList();
            Debug.LogWarning($"Game Manager Example Systems Found: {gameManagerSystems.Count}");
            foreach (var initializer in gameManagerSystems)
            {
                initializer.Initialize(true);
            }
        }
    }
}
