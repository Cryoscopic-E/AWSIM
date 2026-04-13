using UnityEngine;
using Awsim.Usecase.PedestrianSimulation.Profile;
using System;
namespace Awsim.Usecase.PedestrianSimulation
{
    /// <summary>
    /// Mutable state passed to <see cref="IPedestrianBehavior"/> each tick.
    /// Updated by <see cref="PedestrianWalkerController"/> before calling Tick().
    /// </summary>
    [Serializable]
    public class PedestrianBehaviorContext
    {
        /// <summary>Current world-space position of the pedestrian.</summary>
        public Vector3 Position;

        /// <summary>Current world-space rotation of the pedestrian.</summary>
        public Quaternion Rotation;

        /// <summary>Profile driving this pedestrian's behavior.</summary>
        public PedestrianBehaviorProfile Profile;

        /// <summary>
        /// True when the obstacle response is StopAndWait and the path is blocked.
        /// Behaviors should return the current pose unchanged when true.
        /// </summary>
        public bool IsPaused;

        /// <summary>
        /// When <see cref="HasOverrideDirection"/> is true, behaviors should use this
        /// direction instead of their computed movement direction (SteerAround response).
        /// </summary>
        public Vector3 OverrideDirection;

        /// <summary>Whether <see cref="OverrideDirection"/> should be used this tick.</summary>
        public bool HasOverrideDirection;
    }
}
