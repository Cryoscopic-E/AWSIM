using UnityEngine;

namespace Awsim.Usecase.PedestrianSimulation.Profile
{
    /// <summary>
    /// Pedestrian stands in place and occasionally fidgets (small rotational shift).
    /// </summary>
    [CreateAssetMenu(fileName = "IdlePedestrianProfile",
        menuName = "AWSIM/Pedestrian/Idle Profile")]
    public class IdlePedestrianProfile : PedestrianBehaviorProfile
    {
        /// <summary>Minimum seconds between fidget animations.</summary>
        [field: SerializeField, Min(0f)]
        public float FidgetIntervalMin { get; private set; } = 3f;

        /// <summary>Maximum seconds between fidget animations.</summary>
        [field: SerializeField, Min(0f)]
        public float FidgetIntervalMax { get; private set; } = 8f;
    }
}
