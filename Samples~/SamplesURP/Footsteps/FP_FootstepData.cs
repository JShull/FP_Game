using UnityEngine;
namespace FuzzPhyte.Game
{
    using System.Collections.Generic;
    using FuzzPhyte.SystemEvent;
    using FuzzPhyte.Utility;

    [CreateAssetMenu(fileName = "FootstepData", menuName = "FuzzPhyte/Audio/FootstepData", order = 1)]
    public class FP_FootstepData : FP_Data
    {
        public List<AudioClip> footstepClips = new List<AudioClip>();
        public string GroundType;
        // Audio parameters
        public float volume = 1.0f;
        public float pitch = 1.0f;
        public float dryMix = 1.0f;
        public float spatialBlend = 0f;

        /// <summary>
        /// Will return a random clip from the list of clips
        /// </summary>
        /// <returns></returns>
        public AudioClip GetRandomClip()
        {
            if (footstepClips == null || footstepClips.Count == 0)
                return null;

            int index = Random.Range(0, footstepClips.Count);
            return footstepClips[index];
        }
    }
    /// <summary>
    /// Event for Footsteps - some combination of a raycast event and time event will set this up to be passed over
    /// </summary>
    public class FootstepEvent : FPEvent
    {
        [Tooltip("The tag of the ground that our system reports back on")]
        public string GroundType;
        [Tooltip("Will be set to the time since game started based on when this was called")]
        private float timeOfEventSince;
        [Tooltip("Event Name")]
        public string EventName;
        public FootstepEvent(string eventName, string groundTag)
        {
            EventName = eventName;
            GroundType = groundTag;
            timeOfEventSince = Time.realtimeSinceStartup;
        }

        public override void Execute(object data = null)
        {
            
        }

        /// <summary>
        /// Will return true/false based on time between playing step sounds
        /// </summary>
        /// <param name="intervalSeconds"></param>
        /// <returns></returns>
        public bool IsOlderThan(float intervalSeconds, float lastTimePlayedStep)
        {
            return timeOfEventSince - lastTimePlayedStep > intervalSeconds;
        }
    }
}
