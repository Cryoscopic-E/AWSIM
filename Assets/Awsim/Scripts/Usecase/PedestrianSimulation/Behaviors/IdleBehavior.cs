using UnityEngine;
using Awsim.Usecase.PedestrianSimulation.Profile;
namespace Awsim.Usecase.PedestrianSimulation
{
    /// <summary>
    /// Pedestrian stands in place. Periodically applies a small rotational fidget
    /// so the rigidbody angular velocity drives the Animator's rotateSpeed parameter.
    /// </summary>
    internal class IdleBehavior : IPedestrianBehavior
    {
        readonly IdlePedestrianProfile _profile;

        Vector3 _holdPosition;
        Quaternion _baseRotation;
        float _fidgetTimer;
        float _fidgetInterval;
        float _fidgetElapsed;
        bool _isFidgeting;

        const float FidgetDuration = 0.5f;

        internal IdleBehavior(IdlePedestrianProfile profile)
        {
            _profile = profile;
        }

        void IPedestrianBehavior.Start(PedestrianBehaviorContext context)
        {
            _holdPosition = context.Position;
            _baseRotation = context.Rotation;
            _fidgetInterval = Random.Range(_profile.FidgetIntervalMin, _profile.FidgetIntervalMax);
            _fidgetTimer = 0f;
        }

        Pose IPedestrianBehavior.Tick(PedestrianBehaviorContext context, float deltaTime)
        {
            if (context.IsPaused)
                return new Pose(_holdPosition, _baseRotation);

            _fidgetTimer += deltaTime;

            if (!_isFidgeting && _fidgetTimer >= _fidgetInterval)
            {
                _isFidgeting = true;
                _fidgetElapsed = 0f;
                _fidgetTimer = 0f;
                _fidgetInterval = Random.Range(_profile.FidgetIntervalMin, _profile.FidgetIntervalMax);
            }

            var rotation = _baseRotation;
            if (_isFidgeting)
            {
                _fidgetElapsed += deltaTime;
                var angle = Mathf.Sin(_fidgetElapsed / FidgetDuration * Mathf.PI) * 15f;
                rotation = _baseRotation * Quaternion.AngleAxis(angle, Vector3.up);
                if (_fidgetElapsed >= FidgetDuration)
                {
                    _isFidgeting = false;
                    _fidgetElapsed = 0f;
                    rotation = _baseRotation;
                }
            }

            return new Pose(_holdPosition, rotation);
        }
    }
}
