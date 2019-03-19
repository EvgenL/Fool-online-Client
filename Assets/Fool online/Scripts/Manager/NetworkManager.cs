using System.Threading;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fool_online.Scripts.Manager
{
    /// <summary>
    /// Client-side logic of managing connection.
    /// Connects to server on awake.
    /// </summary>
    public class NetworkManager : MonoBehaviourFoolObserver
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


        [Header("Scene name that would be opened after login is succesful")]
        [SerializeField] private string _sceneOnAuthorized = "Main menu";
        [Header("Scene name that would be opened after connection lost")]
        [SerializeField] private string _sceneOnDisconnected = "Login register";
        [Header("Scene name for game")]
        [SerializeField] private string _sceneGameplay = "Gameplay";

        private void Start()
        {
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(_sceneOnDisconnected))
            {
                SceneManager.LoadScene(_sceneOnDisconnected);
            }
        }

        /// <summary>
        /// Connects to server. Called in scene 'Connecting to server.scene'
        /// </summary>
        public void ConnectToGameServer(string ip, int port, string authToken)
        {
            ConnectionState = FoolNetwork.connectionState;
            if (ConnectionState == FoolNetwork.ConnectionState.ConnectingGameServer)
            {
                return;
            }
            print("Trying to connect game server...");
            FoolNetwork.ConnectToGameServer(ip, port, authToken);
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
            if (FoolNetwork.LocalPlayer.IsInRoom)
            {
                FoolNetwork.LeaveRoom();
            }
            FoolNetwork.Disconnect();
        }


        public override void OnAuthorizedOk(long connectionId)
        {
            SceneManager.LoadScene(_sceneOnAuthorized);
        }

        //Observed callback
        public override void OnDisconnectedFromGameServer()
        {
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(_sceneOnDisconnected))
            {
                SceneManager.LoadScene(_sceneOnDisconnected);
            }
        }

        //Observed callback
        public override void OnJoinRoom()
        {
            SceneManager.LoadScene(_sceneGameplay);
        }
    }
}
