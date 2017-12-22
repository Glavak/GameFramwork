namespace GameFramework
{
    public class NetworkPlayer : NetworkFile
    {
        /// <summary>
        /// Player's username. Can change any time, not guaranteed to be unique
        /// </summary>
        public string Username { get; }
    }
}
