using UnityEngine;

namespace Awsim.Usecase.PedestrianSimulation.Profile
{
    /// <summary>
    /// Pedestrian wanders to random destinations within a radius of its spawn point.
    /// </summary>
    [CreateAssetMenu(fileName = "WanderPedestrianProfile",
        menuName = "AWSIM/Pedestrian/Wander Profile")]
    public class WanderPedestrianProfile : PedestrianBehaviorProfile
    {
        /// <summary>Maximum distance from spawn point when choosing a new destination.</summary>
        [field: SerializeField, Min(0f)]
        public float WanderRadius { get; private set; } = 5f;

        /// <summary>Force a new destination if the pedestrian has not arrived within this many seconds.</summary>
        [field: SerializeField, Min(0f)]
        public float DestinationTimeout { get; private set; } = 10f;
    }
}
