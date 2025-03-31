using UnityEngine;

namespace FuzzPhyte.Game
{
    public interface IFPGame<T,A> {
        public void SetupEngine(T Data);
        public void StartEngine();
        public void PauseEngine();
        public void ResumeEngine();
        public void ResetEngine();
        public void StopEngine();
        public void ProcessEvent(A eventData);
        public System.Collections.IEnumerator LerpTransparency();
    }
    /// <summary>
    /// Static class for game functions and data
    /// </summary>
    public static class FPGameUtilityData
    {
    
    }
}
