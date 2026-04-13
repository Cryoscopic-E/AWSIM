using UnityEngine;

namespace Awsim.Usecase.PedestrianSimulation.Profile
{
    /// <summary>
    /// Pedestrian follows an ordered list of waypoints.<br/>
    /// Waypoint Transforms are set on <see cref="PedestrianWalkerController"/> because
    /// ScriptableObjects cannot reference scene objects.
    /// </summary>
    [CreateAssetMenu(fileName = "WaypointPedestrianProfile",
        menuName = "AWSIM/Pedestrian/Waypoint Profile")]
    public class WaypointPedestrianProfile : PedestrianBehaviorProfile
    {
        /// <summary>When true, the pedestrian returns to the first waypoint after the last.</summary>
        [field: SerializeField]
        public bool Loop { get; private set; } = true;
    }
}
