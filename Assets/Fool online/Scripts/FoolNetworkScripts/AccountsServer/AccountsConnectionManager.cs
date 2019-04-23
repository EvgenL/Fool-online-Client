



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


        [SerializeField] private string _accountsServerIp = "51.75.236.170";
        [SerializeField] private int _accountsServerPort = 5054;

        [Header("If check - connect to 127.0.0.1")]
        [SerializeField] private bool _testModeLocalhost = true;

        public static bool IsConnectingToAccountsServer = false;
        public static bool IsConnected = false;


        /// <summary>
        /// My client socket for sending data to server
        /// </summary>
        private WebSocket mySocket;

        /// <summary>
        /// Data to send
        /// </summary>
        private XElement bufferedBody;

        // Start is called before the first frame update
        void Start()
        {
            // Initialize AccountsTransport with server endpoint
            string ip = _testModeLocalhost ? "127.0.0.1" : _accountsServerIp;
            AccountsTransport.SetIpEndpoint(ip, _accountsServerPort);


            //StartCoroutine(//todo CheckVersion(LoginServerIp, LoginServerPort));
        }

        /*

                /// <summary>
        /// Connects to server then dends XML data
        /// </summary>
        private void ConnectAndSend(string serverIp, int serverPort, XElement body)
        {
            //Create and set up a new socket
            mySocket = WebSocketFactory.CreateInstance("ws://" + serverIp + ":" + serverPort);

            //Init callbacks
            mySocket.OnOpen += SendBufferedBody;
            mySocket.OnMessage += OnMessage;
            mySocket.OnError += OnError;
            mySocket.OnClose += OnClose;
            bufferedBody = body;

            //Connect
            mySocket.Connect();
        }

        /// <summary>
        /// Triggered on open to send request immdiatelly
        /// </summary>
        private void SendBufferedBody()
        {
            byte[] data = Encoding.Unicode.GetBytes(bufferedBody.ToString());

            mySocket.Send(data);
            
        }

        private void OnMessage(byte[] data)
        {
            //parse response data
            string bodyString = Encoding.Unicode.GetString(data);
            XElement body = XElement.Parse(bodyString);

            //check for errors
            XElement error = body.GetChildElement("Error");
            if (error != null)
            {
                //todo proper error handling. show message on screen
                Debug.LogError("Recieved error!\n" + error.GetChildElement("Info").Value);
                return;
            }

            //check for version check data
            XElement versionCheck = body.GetChildElement("VersionCheck");
            if (versionCheck != null && versionCheck.Value == "OK")
            {
                Debug.Log("Recieved versionCheck OK\n" + versionCheck.ToString());
            }

            //check for login data
            XElement loginData = body.GetChildElement("LoginData");
            if (loginData != null)
            {
                //read server data
                string gameServerIp = body.GetChildElement("GameServerIp").Value;
                int gameServerPort = int.Parse(body.GetChildElement("GameServerPort").Value);
                string token = body.GetChildElement("Token").Value;

                Debug.Log("Got token: " + token);

                Debug.Log("Anon login OK. Connecting to game server: " + gameServerIp + ":" + gameServerPort);

                NetworkManager.Instance.ConnectToGameServer(gameServerIp, gameServerPort, token);
                return;
            }

        }

        private void OnError(string errormsg)
        {
            Debug.Log("Accounts server connection error:\n" + errormsg, this);
            //todo show error msg
            //throw new Exception(errormsg);
        }

        private void OnClose(WebSocketCloseCode closecode)
        {
            Debug.Log("Accounts server connection closed:\n" + closecode, this);
            mySocket = null;
        }
        
             */
    }
}
