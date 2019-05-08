



using System.Text;
using System.Xml.Linq;
using Assets.Fool_online.Scripts.FoolNetworkScripts;
using Assets.Fool_online.Scripts.FoolNetworkScripts.AccountsServer;
using Fool_online.Plugins;
using UnityEngine;
using NetworkManager = Fool_online.Scripts.Manager.NetworkManager;


namespace Fool_online.Scripts.FoolNetworkScripts
{
    /// <summary>
    /// Connects to accounts server.
    /// Checks version and initializes AccountsTransport
    /// TODO mostly not used for now. Functions handled by AccountsTransport class
    /// </summary>
    public class AccountsConnectionManager : MonoBehaviour
    {


        #region Singleton
        public static AccountsConnectionManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        #endregion

    }
}
