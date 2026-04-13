using UnityEngine;

namespace Awsim.Usecase.PedestrianSimulation
{
    /// <summary>
    /// Implemented by each concrete pedestrian behavior (Waypoint, Wander, Crosswalk, Idle).
    /// </summary>
    internal interface IPedestrianBehavior
    {
        /// <summary>Called once when the behavior is first activated.</summary>
        void Start(PedestrianBehaviorContext context);

        /// <summary>
        /// Called every FixedUpdate tick.<br/>
        /// Returns the desired <see cref="Pose"/> (position + rotation) for this tick.<br/>
        /// Implementations must honour <see cref="PedestrianBehaviorContext.IsPaused"/> —
        /// when true, return <c>new Pose(context.Position, context.Rotation)</c> unchanged.
        /// </summary>
        Pose Tick(PedestrianBehaviorContext context, float deltaTime);
    }
}
