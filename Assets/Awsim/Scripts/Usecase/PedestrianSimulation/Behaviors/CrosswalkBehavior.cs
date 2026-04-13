using UnityEngine;
using Awsim.Usecase.PedestrianSimulation.Profile;
namespace Awsim.Usecase.PedestrianSimulation
{
    /// <summary>
    /// Pedestrian crosses from a start Transform to an end Transform.<br/>
    /// Optionally jaywalks (randomised diagonal path) and optionally returns.
    /// </summary>
    internal class CrosswalkBehavior : IPedestrianBehavior
    {
        /// <summary>Distance threshold to consider the destination reached.</summary>
        const float ArrivalThreshold = 0.15f;

        /// <summary>Minimum jaywalking angle offset in degrees.</summary>
        const float JaywalkAngleMin = 30f;

        /// <summary>Maximum jaywalking angle offset in degrees.</summary>
        const float JaywalkAngleMax = 60f;

        readonly CrosswalkPedestrianProfile _profile;
        readonly Transform _startPoint;
        readonly Transform _endPoint;

        Vector3 _actualTarget;
        float _currentSpeed;
        float _pauseTimer;
        bool _isPausing;
        bool _finished;
        bool _goingToEnd;

        internal CrosswalkBehavior(CrosswalkPedestrianProfile profile, Transform startPoint, Transform endPoint)
        {
            _profile = profile;
            _startPoint = startPoint;
            _endPoint = endPoint;
        }

        void IPedestrianBehavior.Start(PedestrianBehaviorContext context)
        {
            _goingToEnd = true;
            _currentSpeed = Random.Range(_profile.SpeedMin, _profile.SpeedMax);
            _actualTarget = ComputeTarget(_startPoint.position, _endPoint.position);
        }

        Pose IPedestrianBehavior.Tick(PedestrianBehaviorContext context, float deltaTime)
        {
            if (context.IsPaused || _finished)
                return new Pose(context.Position, context.Rotation);

            if (_isPausing)
            {
                _pauseTimer -= deltaTime;
                if (_pauseTimer <= 0f)
                {
                    _isPausing = false;
                    if (!_profile.ReturnAfterCrossing)
                    {
                        _finished = true;
                        return new Pose(context.Position, context.Rotation);
                    }
                    _goingToEnd = !_goingToEnd;
                    var from = _goingToEnd ? _startPoint.position : _endPoint.position;
                    var to = _goingToEnd ? _endPoint.position : _startPoint.position;
                    _actualTarget = ComputeTarget(from, to);
                    _currentSpeed = Random.Range(_profile.SpeedMin, _profile.SpeedMax);
                }
                return new Pose(context.Position, context.Rotation);
            }

            var toTarget = _actualTarget - context.Position;
            toTarget.y = 0f;

            if (toTarget.magnitude < ArrivalThreshold)
            {
                var duration = Random.Range(_profile.PauseDurationMin, _profile.PauseDurationMax);
                _isPausing = true;
                _pauseTimer = duration;
                return new Pose(context.Position, context.Rotation);
            }

            var moveDir = context.HasOverrideDirection ? context.OverrideDirection : toTarget.normalized;
            var newPosition = context.Position + moveDir * _currentSpeed * deltaTime;
            var newRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            return new Pose(newPosition, newRotation);
        }

        /// <summary>
        /// Computes the crossing target. If jaywalking, offsets the path ±30–60° from the
        /// straight <paramref name="from"/>→<paramref name="to"/> line.
        /// </summary>
        Vector3 ComputeTarget(Vector3 from, Vector3 to)
        {
            if (Random.value < _profile.JaywalkingProbability)
            {
                var sign = Random.value > 0.5f ? 1f : -1f;
                var angle = Random.Range(JaywalkAngleMin, JaywalkAngleMax) * sign;
                var baseDir = (to - from).normalized;
                var offsetDir = Quaternion.AngleAxis(angle, Vector3.up) * baseDir;
                return from + offsetDir * Vector3.Distance(from, to);
            }
            return to;
        }
    }
}
