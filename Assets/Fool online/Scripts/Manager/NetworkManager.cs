using Fool_online.Scripts.Network;
using Fool_online.Scripts.Network.NetworksObserver;
using UnityEngine.SceneManagement;

namespace Fool_online.Scripts
{
    /// <summary>
    /// Client-side logic of managing connection.
    /// Connects to server on awake.
    /// </summary>
    public class NetworkManager : MonoBehaviourFoolNetworkObserver
    {
        /// <summary>
        /// Current state of connection
        /// </summary>
        public FoolNetwork.ConnectionState ConnectionState = FoolNetwork.ConnectionState.ConnectingGameServer;

        #region Singleton

        public static NetworkManager Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
                return;
            }
        }

#endregion


        private void Start()
        {
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Connecting to server"))
            {
                SceneManager.LoadScene("Connecting to server");
            }
        }

        /// <summary>
        /// Connects to server. Called in scene 'Connecting to server.scene'
        /// </summary>
        public void Connect()
        {
            ConnectionState = FoolNetwork.connectionState;
            if (ConnectionState == FoolNetwork.ConnectionState.ConnectingGameServer)
            {
                return;
            }
            print("Trying to connect...");
            FoolNetwork.ConnectToGameServer();
        }

        /// <summary>
        /// MonoBehaviourUpdate passed to FoolNetwork ticks
        /// </summary>
        private void Update()
        {
            ConnectionState = FoolNetwork.connectionState;
            FoolNetwork.Update();
        }

        /// <summary>
        /// If application was closed we are sending to server signal that we have disconnected.
        /// </summary>
        private void OnApplicationQuit()
        {
            print("Quitting application. Disconneting from server.");
            FoolNetwork.Disconnect();
        }

        //Observed callback
        public override void OnConnectedToGameServer()
        {
            print("I feel OnConnectedToGameServer.....");
        }

        //Observed callback
        public override void OnDisconnectedFromGameServer()
        {
            print("I feel OnDisconnectedFromGameServer.....");

            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Connecting to server"))
            {
                SceneManager.LoadScene("Connecting to server");
            }
        }

        //Observed callback
        public override void OnJoinRoom()
        {
            SceneManager.LoadScene("Gameplay");
        }

        //Observed callback
        public override void OnRoomData()
        {
            print("NetworkManager: OnRoomData");
        }
    }
}
