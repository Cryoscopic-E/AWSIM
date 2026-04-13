using UnityEngine;

namespace Awsim.Usecase.PedestrianSimulation.Profile
{
    /// <summary>
    /// Pedestrian crosses from a start point to an end point.<br/>
    /// Start and end Transforms are set on <see cref="PedestrianWalkerController"/> because
    /// ScriptableObjects cannot reference scene objects.
    /// </summary>
    [CreateAssetMenu(fileName = "CrosswalkPedestrianProfile",
        menuName = "AWSIM/Pedestrian/Crosswalk Profile")]
    public class CrosswalkPedestrianProfile : PedestrianBehaviorProfile
    {
        /// <summary>When true, the pedestrian walks back after reaching the end point.</summary>
        [field: SerializeField]
        public bool ReturnAfterCrossing { get; private set; } = true;

        /// <summary>
        /// Probability [0,1] that the pedestrian crosses via a randomised diagonal offset
        /// (±30–60° from the straight A→B line) instead of walking straight.
        /// </summary>
        [field: SerializeField, Range(0f, 1f)]
        public float JaywalkingProbability { get; private set; } = 0f;
    }
}
