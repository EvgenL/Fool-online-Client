namespace Fool_online.Scripts.FoolNetworkScripts
{
    /// <summary>
    /// This would be serialized and then sent to server on login
    /// </summary>
    public class JsonLogin
    {
        public string ClientVersion;

        /// <summary>
        /// anonymous, oauth, by-email, etc
        /// </summary>
        public string LoginMethod;

        /// <summary>
        /// unique userId
        /// </summary>
        public string UserId;

        /// <summary>
        /// unique user's email adress
        /// </summary>
        public string Email; 

    }
}
