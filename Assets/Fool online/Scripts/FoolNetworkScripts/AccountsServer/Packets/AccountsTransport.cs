

using System;
using System.Collections.Generic;
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
        public static bool IsConnected = false;
        public static bool IsConnecting = false;

        /// <summary>
        /// My client socket for sending data to server
        /// </summary>
        private static WebSocket mySocket;

        /// <summary>
        /// Messages buffered while waiting for connection
        /// </summary>
        private static Queue<XElement> _bufferedMessages = new Queue<XElement>();

        /// <summary>
        /// ip.
        /// Set by SetIpEndpoint method
        /// </summary>
        private static string _accountsServerIp;

        /// <summary>
        /// port.
        /// Set by SetIpEndpoint method
        /// </summary>
        private static int _accountsServerPort;

        /// <summary>
        /// Flag if SetIpEndpoint method was ever called
        /// </summary>
        private static bool _endPointSet = false;

        /// <summary>
        /// Sets ip and port of account server
        /// Should be called once before sending any data
        /// </summary>
        public static void SetIpEndpoint(string ip, int port)
        {
            _endPointSet = true;

            _accountsServerIp = ip;
            _accountsServerPort = port;
        }


        /// <summary>
        /// Connects to server then dends XML data
        /// </summary>
        public static void Send(XElement body)
        {
            if (!_endPointSet)
            {
                throw new Exception(
                    "Ip and port wasn't set. Call AccountsTransport.SetIpEndpoint method before sending any data.");
            }

            //Create and set up a new socket
            mySocket = WebSocketFactory.CreateInstance("ws://" + _accountsServerIp + ":" + _accountsServerPort);

            // buffer message
            _bufferedMessages.Enqueue(body);

            // not connected (first message)
            if (!IsConnected && !IsConnecting)
            {
                //Connect(); <- method was removed to make clear meaning of a 'SendBuferedMessages' method and 'IsConnecting' bool

                IsConnecting = true;

                //Init callbacks
                mySocket.OnOpen += SendBuferedMessages;
                mySocket.OnMessage += OnMessage;
                mySocket.OnError += OnError;
                mySocket.OnClose += OnClose;

                //Connect
                mySocket.Connect();
            }
            else // if already connected
            {
                SendBuferedMessages();
            }

        }

        /// <summary>
        /// Triggered on open.
        /// Immidiatelly sends messages
        /// </summary>
        private static void SendBuferedMessages()
        {
            IsConnected = true;
            IsConnecting = false;

            // send all the messages
            while (_bufferedMessages.Count > 0)
            {
                Debug.Log("Sending message " + _bufferedMessages.Peek().ToString());
                // get string
                string messageString = _bufferedMessages.Dequeue().ToString();
                // get bytes
                byte[] dataBytes = Encoding.Unicode.GetBytes(messageString);
                // send
                mySocket.Send(dataBytes);
            }
        }


        private static void OnMessage(byte[] data)
        {
            //parse response data
            string bodyString = Encoding.Unicode.GetString(data);
            XElement body = XElement.Parse(bodyString);
            Debug.Log("Got message " + body);

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
            IsConnected = false;
        }

    }
}
