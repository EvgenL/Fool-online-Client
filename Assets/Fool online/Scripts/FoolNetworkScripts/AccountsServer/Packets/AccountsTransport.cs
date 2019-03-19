
//DEFINES
#define TEST_MODE_LOCALHOST // if defined, will connect to localhost

using System.Text;
using System.Xml.Linq;
using Fool_online.Plugins;
using Fool_online.Scripts.Manager;
using UnityEngine;

namespace Assets.Fool_online.Scripts.FoolNetworkScripts.AccountsServer
{
    /// <summary>
    /// transport-layer for sending accounts data to server
    /// </summary>
    static class AccountsTransport
    {
        private static string LoginServerIp = "51.75.236.170";
        private static int LoginServerPort = 5054;

        public static bool IsConnectingToAccountsServer = false;
        public static bool IsConnected = false;

        /// <summary>
        /// My client socket for sending data to server
        /// </summary>
        private static WebSocket mySocket;

        /// <summary>
        /// Data to send
        /// </summary>
        private static XElement bufferedBody;
        

        /// <summary>
        /// Connects to server then dends XML data
        /// </summary>
        public static void Send(XElement body)
        {
#if TEST_MODE_LOCALHOST
            string LoginServerIp = "127.0.0.1";
#else
            string LoginServerIp = LoginServerIp;
#endif
            //Create and set up a new socket
            mySocket = WebSocketFactory.CreateInstance("ws://" + LoginServerIp + ":" + LoginServerPort);

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
        private static void SendBufferedBody()
        {
            byte[] data = Encoding.Unicode.GetBytes(bufferedBody.ToString());

            mySocket.Send(data);
        }


        private static void OnMessage(byte[] data)
        {
            //parse response data
            string bodyString = Encoding.Unicode.GetString(data);
            XElement body = XElement.Parse(bodyString);

            // get result
            XElement result = body.GetChildElement("Result");
            if (result.Value == "Error")
            {
                XElement error = body.GetChildElement("ErrorInfo");
                //todo proper error handling
                string code = error.GetChildElement("Code").Value;
                string codeString = error.GetChildElement("CodeString").Value;
                string message = error.GetChildElement("Message").Value;
                
                Debug.LogError($"Recieved error! Code: {code}\n"
                + $"{codeString}." + $" Message: {message}");
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
                string gameServerIp = loginData.GetChildElement("GameServerIp").Value;
                int gameServerPort = int.Parse(loginData.GetChildElement("GameServerPort").Value);
                string token = loginData.GetChildElement("Token").Value;

                Debug.Log("Got token: " + token);

                Debug.Log("Anon login OK. Connecting to game server: " + gameServerIp + ":" + gameServerPort);

                NetworkManager.Instance.ConnectToGameServer(gameServerIp, gameServerPort, token);
                return;
            }

        }

        private static void OnError(string errormsg)
        {
            Debug.Log("Accounts server connection error:\n" + errormsg);
            //todo show error msg
            //throw new Exception(errormsg);
        }

        private static void OnClose(WebSocketCloseCode closecode)
        {
            Debug.Log("Accounts server connection closed:\n" + closecode);
            mySocket = null;
        }

    }
}
