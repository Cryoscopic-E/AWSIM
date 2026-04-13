namespace Awsim.Usecase.PedestrianSimulation
{
    /// <summary>
    /// How a pedestrian reacts when an obstacle is detected in its path.
    /// </summary>
    public enum ObstacleResponse
    {
        /// <summary>
        /// Halt until the path is clear, then resume.
        /// </summary>
        StopAndWait,

        /// <summary>
        /// Deflect laterally around the obstacle and continue moving.
        /// </summary>
        SteerAround
    }
}
