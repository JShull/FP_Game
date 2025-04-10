using System.Collections.Generic;
using UnityEngine;

namespace FuzzPhyte.Game
{
    public class FP_FootstepEventManager : MonoBehaviour
    {
        private static FP_FootstepEventManager _instance;

        public static FP_FootstepEventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<FP_FootstepEventManager>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("FootstepEventManager");
                        _instance = obj.AddComponent<FP_FootstepEventManager>();
                    }
                }
                return _instance;
            }
        }

        private List<FP_BaseFootstep> footstepListeners = new List<FP_BaseFootstep>();

        public void RegisterListener(FP_BaseFootstep listener)
        {
            if (!footstepListeners.Contains(listener))
            {
                footstepListeners.Add(listener);
            }
        }

        public void UnregisterListener(FP_BaseFootstep listener)
        {
            if (footstepListeners.Contains(listener))
            {
                footstepListeners.Remove(listener);
            }
        }

        public void TriggerFootstepEvent(FootstepEvent footstepEvent)
        {
            foreach (var listener in footstepListeners)
            {
                listener.ProcessEvent(footstepEvent);
            }
        }
    }
}
