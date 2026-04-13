using UnityEngine;
using Awsim.Entity;
using Awsim.Usecase.PedestrianSimulation.Profile;

namespace Awsim.Usecase.PedestrianSimulation
{
    /// <summary>
    /// Profile-driven NPC pedestrian controller.<br/>
    /// Resolves an <see cref="IPedestrianBehavior"/> from the assigned
    /// <see cref="PedestrianBehaviorProfile"/> at <see cref="Initialize"/> time,
    /// then ticks it every FixedUpdate via <see cref="OnFixedUpdate"/>.<br/>
    /// Obstacle awareness (including the ego vehicle) is evaluated each tick using a
    /// forward <see cref="ObstacleAwareness"/> SphereCast.
    /// </summary>
    [RequireComponent(typeof(Pedestrian))]
    public class PedestrianWalkerController : MonoBehaviour
    {
        [SerializeField] Pedestrian _pedestrian;
        [SerializeField] PedestrianBehaviorProfile _profile;

        [Header("Waypoint Behavior")]
        [Tooltip("Used when Profile is a WaypointPedestrianProfile.")]
        [SerializeField] Transform[] _waypoints;

        [Header("Crosswalk Behavior")]
        [Tooltip("Used when Profile is a CrosswalkPedestrianProfile.")]
        [SerializeField] Transform _crosswalkStart;
        [Tooltip("Used when Profile is a CrosswalkPedestrianProfile.")]
        [SerializeField] Transform _crosswalkEnd;

        IPedestrianBehavior _behavior;
        ObstacleAwareness _awareness;
        PedestrianBehaviorContext _context;
        Pose _lastPose;

        /// <summary>
        /// Initialise this controller. Must be called by the Scene class before the first Update.
        /// </summary>
        public void Initialize()
        {
            _pedestrian.Initialize();
            _lastPose = new Pose(transform.position, transform.rotation);
            _awareness = new ObstacleAwareness(_profile.AwarenessRadius, _profile.ObstacleLayerMask);

            _context = new PedestrianBehaviorContext
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Profile = _profile
            };

            _behavior = _profile switch
            {
                WaypointPedestrianProfile wp  => new WaypointBehavior(wp, _waypoints),
                WanderPedestrianProfile wr    => new WanderBehavior(wr),
                CrosswalkPedestrianProfile cp => new CrosswalkBehavior(cp, _crosswalkStart, _crosswalkEnd),
                IdlePedestrianProfile ip      => new IdleBehavior(ip),
                _                             => new IdleBehavior(ScriptableObject.CreateInstance<IdlePedestrianProfile>())
            };

            _behavior.Start(_context);
        }

        /// <summary>Forwards animation tick to the underlying <see cref="Pedestrian"/>.</summary>
        public void OnUpdate()
        {
            _pedestrian.OnUpdate();
        }

        /// <summary>Runs obstacle awareness and behavior tick, then writes the result to <see cref="Pedestrian.PoseInput"/>.</summary>
        public void OnFixedUpdate()
        {
            _context.Position = _pedestrian.PoseInput.position;
            _context.Rotation = _pedestrian.PoseInput.rotation;

            var awareness = _awareness.Check(transform.position, transform.forward);
            //Debug.LogWarning($"PEDESTRIAN::{_pedestrian.name} AWARE {awareness.ObstacleNormal} IS {(awareness.IsBlocked ? "BLOCKED":"FREE")}");
            if (awareness.IsBlocked)
            {
                switch (_profile.ObstacleResponseMode)
                {
                    case ObstacleResponse.StopAndWait:
                        _context.IsPaused = true;
                        _context.HasOverrideDirection = false;
                        _pedestrian.PoseInput = _lastPose;
                        return;

                    case ObstacleResponse.SteerAround:
                        _context.IsPaused = false;
                        _context.HasOverrideDirection = true;
                        _context.OverrideDirection = Vector3.Cross(Vector3.up, awareness.ObstacleNormal).normalized;
                        break;
                }
            }
            else
            {
                _context.IsPaused = false;
                _context.HasOverrideDirection = false;
            }
            
            var pose = _behavior.Tick(_context, Time.fixedDeltaTime);
            _pedestrian.PoseInput = pose;
            _lastPose = pose;
            _pedestrian.OnFixedUpdate();
        }

        void Reset()
        {
            _pedestrian = GetComponent<Pedestrian>();
        }
    }
}
