using UnityEngine;
using Awsim.Usecase.PedestrianSimulation.Profile;

namespace Awsim.Usecase.PedestrianSimulation
{
    /// <summary>
    /// Pedestrian wanders to random destinations within <see cref="WanderPedestrianProfile.WanderRadius"/>
    /// of its spawn point, pausing briefly between moves.
    /// </summary>
    internal class WanderBehavior : IPedestrianBehavior
    {
        /// <summary>Distance threshold to consider a destination reached.</summary>
        const float ArrivalThreshold = 0.2f;

        /// <summary>Extra pause duration when StoppingProbability triggers.</summary>
        const float ExtendedPauseMin = 2f;

        /// <summary>Upper bound of the extended pause range in seconds.</summary>
        const float ExtendedPauseMax = 5f;

        readonly WanderPedestrianProfile _profile;

        Vector3 _anchor;
        Vector3 _destination;
        float _currentSpeed;
        float _pauseTimer;
        float _destinationTimer;
        bool _isPausing;

        internal WanderBehavior(WanderPedestrianProfile profile)
        {
            _profile = profile;
        }

        void IPedestrianBehavior.Start(PedestrianBehaviorContext context)
        {
            _anchor = context.Position;
            _currentSpeed = Random.Range(_profile.SpeedMin, _profile.SpeedMax);
            PickNewDestination();
        }

        Pose IPedestrianBehavior.Tick(PedestrianBehaviorContext context, float deltaTime)
        {
            if (context.IsPaused)
                return new Pose(context.Position, context.Rotation);

            if (_isPausing)
            {
                _pauseTimer -= deltaTime;
                if (_pauseTimer <= 0f)
                {
                    _isPausing = false;
                    PickNewDestination();
                    _currentSpeed = Random.Range(_profile.SpeedMin, _profile.SpeedMax);
                }
                return new Pose(context.Position, context.Rotation);
            }

            _destinationTimer += deltaTime;
            if (_destinationTimer >= _profile.DestinationTimeout)
                PickNewDestination();

            var toDestination = _destination - context.Position;
            toDestination.y = 0f;

            if (toDestination.magnitude < ArrivalThreshold)
            {
                var duration = Random.Range(_profile.PauseDurationMin, _profile.PauseDurationMax);
                if (Random.value < _profile.StoppingProbability)
                    duration += Random.Range(ExtendedPauseMin, ExtendedPauseMax);
                _isPausing = true;
                _pauseTimer = duration;
                return new Pose(context.Position, context.Rotation);
            }

            var moveDir = (context.HasOverrideDirection && context.OverrideDirection.sqrMagnitude > 1e-6f)
                ? context.OverrideDirection
                : toDestination.normalized;
            var newPosition = context.Position + moveDir * _currentSpeed * deltaTime;
            var newRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            return new Pose(newPosition, newRotation);
        }

        void PickNewDestination()
        {
            var angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            var radius = Random.Range(0f, _profile.WanderRadius);
            _destination = _anchor + new Vector3(
                Mathf.Cos(angle) * radius,
                0f,
                Mathf.Sin(angle) * radius);
            _destinationTimer = 0f;
        }
    }
}
