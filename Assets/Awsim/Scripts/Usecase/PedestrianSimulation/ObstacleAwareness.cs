using UnityEngine;

namespace Awsim.Usecase.PedestrianSimulation
{
    /// <summary>
    /// Result of a single forward obstacle check.
    /// </summary>
    public struct AwarenessResult
    {
        /// <summary>True when an obstacle is within the awareness cast distance.</summary>
        public bool IsBlocked;

        /// <summary>Distance to the nearest obstacle in metres. <see cref="float.MaxValue"/> when clear.</summary>
        public float Distance;

        /// <summary>Surface normal of the obstacle hit. <see cref="Vector3.zero"/> when clear.</summary>
        public Vector3 ObstacleNormal;
    }

    /// <summary>
    /// Performs forward SphereCast obstacle detection for a pedestrian.
    /// </summary>
    public class ObstacleAwareness
    {
        /// <summary>Maximum lookahead distance for the SphereCast in metres.</summary>
        const float CastDistance = 3f;

        readonly float _radius;
        readonly LayerMask _layerMask;

        /// <param name="radius">SphereCast sphere radius, from <see cref="PedestrianBehaviorProfile.AwarenessRadius"/>.</param>
        /// <param name="layerMask">Layers to test against, from <see cref="PedestrianBehaviorProfile.ObstacleLayerMask"/>.</param>
        public ObstacleAwareness(float radius, LayerMask layerMask)
        {
            _radius = radius;
            _layerMask = layerMask;
        }

        /// <summary>
        /// Cast a sphere forward from <paramref name="position"/> in <paramref name="forward"/> direction.
        /// </summary>
        public AwarenessResult Check(Vector3 position, Vector3 forward)
        {
            var hit = Physics.SphereCast(
                position,
                _radius,
                forward,
                out var hitInfo,
                CastDistance,
                _layerMask,
                QueryTriggerInteraction.Ignore);

            return new AwarenessResult
            {
                IsBlocked = hit,
                Distance = hit ? hitInfo.distance : float.MaxValue,
                ObstacleNormal = hit ? hitInfo.normal : Vector3.zero
            };
        }
    }
}
