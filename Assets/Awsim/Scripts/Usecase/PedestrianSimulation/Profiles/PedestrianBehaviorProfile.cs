using UnityEngine;

namespace Awsim.Usecase.PedestrianSimulation.Profile
{
    /// <summary>
    /// Shared parameters for all pedestrian behavior profiles.<br/>
    /// Subclass this to define a concrete behavior mode.
    /// </summary>
    public abstract class PedestrianBehaviorProfile : ScriptableObject
    {
        /// <summary>Minimum walk speed in m/s.</summary>
        [field: SerializeField, Min(0f)]
        public float SpeedMin { get; private set; } = 0.8f;

        /// <summary>Maximum walk speed in m/s.</summary>
        [field: SerializeField, Min(0f)]
        public float SpeedMax { get; private set; } = 1.4f;

        /// <summary>Minimum pause duration at each waypoint or destination in seconds.</summary>
        [field: SerializeField, Min(0f)]
        public float PauseDurationMin { get; private set; } = 0.5f;

        /// <summary>Maximum pause duration at each waypoint or destination in seconds.</summary>
        [field: SerializeField, Min(0f)]
        public float PauseDurationMax { get; private set; } = 2f;

        /// <summary>Probability [0,1] of an extended stop at each pause point.</summary>
        [field: SerializeField, Range(0f, 1f)]
        public float StoppingProbability { get; private set; } = 0.1f;

        /// <summary>Radius of the forward SphereCast used for obstacle detection in metres.</summary>
        [field: SerializeField, Min(0f)]
        public float AwarenessRadius { get; private set; } = 0.5f;

        /// <summary>
        /// Physics layers treated as obstacles.<br/>
        /// Should include pedestrian, static obstacle, and ego vehicle layers.
        /// </summary>
        [field: SerializeField]
        public LayerMask ObstacleLayerMask { get; private set; }

        /// <summary>How the pedestrian responds when an obstacle is detected.</summary>
        [field: SerializeField]
        public ObstacleResponse ObstacleResponseMode { get; private set; } = ObstacleResponse.StopAndWait;
    }
}
