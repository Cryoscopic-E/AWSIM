using UnityEngine;
using Awsim.Usecase.PedestrianSimulation.Profile;
namespace Awsim.Usecase.PedestrianSimulation
{
    /// <summary>
    /// Pedestrian advances through an ordered array of waypoints, pausing at each arrival.
    /// Speed is re-sampled from the profile range at the start of every movement segment.
    /// </summary>
    internal class WaypointBehavior : IPedestrianBehavior
    {
        /// <summary>Distance threshold to consider a waypoint reached.</summary>
        const float ArrivalThreshold = 0.15f;

        /// <summary>Extra pause duration added when StoppingProbability triggers.</summary>
        const float ExtendedPauseMin = 2f;

        /// <summary>Upper bound of the extended pause range in seconds.</summary>
        const float ExtendedPauseMax = 5f;

        readonly WaypointPedestrianProfile _profile;
        readonly Transform[] _waypoints;

        int _currentIndex;
        float _currentSpeed;
        float _pauseTimer;
        bool _isPausing;
        bool _finished;

        internal WaypointBehavior(WaypointPedestrianProfile profile, Transform[] waypoints)
        {
            _profile = profile;
            _waypoints = waypoints;
        }

        void IPedestrianBehavior.Start(PedestrianBehaviorContext context)
        {
            _currentIndex = 0;
            _finished = false;
            _currentSpeed = Random.Range(_profile.SpeedMin, _profile.SpeedMax);
        }

        Pose IPedestrianBehavior.Tick(PedestrianBehaviorContext context, float deltaTime)
        {
            if (context.IsPaused || _finished || _waypoints == null || _waypoints.Length == 0)
                return new Pose(context.Position, context.Rotation);

            if (_isPausing)
            {
                _pauseTimer -= deltaTime;
                if (_pauseTimer <= 0f)
                    _isPausing = false;
                return new Pose(context.Position, context.Rotation);
            }

            var target = _waypoints[_currentIndex].position;
            var toTarget = target - context.Position;
            toTarget.y = 0f;

            if (toTarget.magnitude < ArrivalThreshold)
            {
                AdvanceWaypoint();
                BeginPause();
                return new Pose(context.Position, context.Rotation);
            }

            var moveDir = context.HasOverrideDirection ? context.OverrideDirection : toTarget.normalized;
            var newPosition = context.Position + moveDir * _currentSpeed * deltaTime;
            var newRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            return new Pose(newPosition, newRotation);
        }

        void AdvanceWaypoint()
        {
            _currentIndex++;
            if (_currentIndex >= _waypoints.Length)
            {
                if (_profile.Loop)
                    _currentIndex = 0;
                else
                    _finished = true;
            }
        }

        void BeginPause()
        {
            var duration = Random.Range(_profile.PauseDurationMin, _profile.PauseDurationMax);
            if (Random.value < _profile.StoppingProbability)
                duration += Random.Range(ExtendedPauseMin, ExtendedPauseMax);

            _isPausing = true;
            _pauseTimer = duration;
            _currentSpeed = Random.Range(_profile.SpeedMin, _profile.SpeedMax);
        }
    }
}
