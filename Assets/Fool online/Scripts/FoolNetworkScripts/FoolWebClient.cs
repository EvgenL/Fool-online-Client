using System;
using System.Collections.Generic;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using HybridWebSocket;
using UnityEngine;

namespace Fool_online.Scripts.FoolNetworkScripts
{
    /// <summary>
    /// Connects to fool online server
    /// </summary>
    public class FoolWebClient : FoolObservable
    {
        //public static string ClientVersion = "1.2"; //todo implement version check

        private static FoolWebClient _instance;

        public static FoolWebClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FoolWebClient();
                }
                return _instance;
            }
        }

        private string LastIp;
        private int LastPort;

        private string myToken;

        public bool IsConnected = false;
        public bool IsConnectingToGameServer = false;

        /// <summary>
        /// Sometimes we recieve more than 1 message per frame. We buffer all the recieved messages
        /// and process them later on in Update()
        /// </summary>
        private readonly Queue<byte[]> bufferedRecievedMessages = new Queue<byte[]>();


        private WebSocket mySocket;


        public void Start(string ip, int port, string authToken)
        {
            //Observable
            OnTryingConnectToGameServer();

            myToken = authToken;

            //Create a connection
            ConnectToGameServer(ip, port);
        }

        /// <summary>
        /// This shuold be called on unity update callback used to handle data only from main thread
        /// </summary>
        public void Update()
        {
            while (bufferedRecievedMessages.Count > 0)
            {
                ClientHandlePackets.HandleData(bufferedRecievedMessages.Dequeue());
            }
        }

        /// <summary>
        /// Connects this player to game server
        /// </summary>
        public void ConnectToGameServer(string serverIp, int serverPort)
        {
            IsConnectingToGameServer = true;

            LastIp = serverIp;
            LastPort = serverPort;

            //if player socket exist
            if (mySocket != null)
            {
                //don't do anything if it's already connecter
                if (IsConnected)
                {
                    throw new Exception("Error: Trying to connect while already connecting.");
                }

                //Else if it wasn't connected then destroy connection
                Disconnect();
                return;
            }

            //Create and set up a new socket
            mySocket = WebSocketFactory.CreateInstance("ws://" + serverIp + ":" + serverPort);

            //Init callbacks
            mySocket.OnOpen += OnOpen;
            mySocket.OnMessage += OnMessage;
            mySocket.OnError += OnError;
            mySocket.OnClose += OnClose;

            mySocket.Connect();
        }

        private void OnOpen()
        {
            //observable
            OnConnectedToGameServer();

            IsConnectingToGameServer = false;
            IsConnected = true;

            //authorize with token
            ClientSendPackets.Send_Authorize(myToken);
        }

        private void OnMessage(byte[] data)
        {
            //Buffer this message 
            bufferedRecievedMessages.Enqueue(data);
        }

        private void OnError(string errormsg)
        {
            //throw new Exception(errormsg);
        }

        private void OnClose(WebSocketCloseCode closecode)
        {
            Disconnect(closecode.ToString());

        }

        /// <summary>
        /// Performs write to server stream
        /// </summary>
        /// <param name="data">data to write to server</param>
        public static void WriteToServer(byte[] data)
        {
            if (Instance.mySocket != null)
            {
                Instance.mySocket.Send(data);
            }
            else
            {
                throw new Exception("Error: Trying send data to server but not connected");
            }
        }

        public void Disconnect(string disconnectReason = null)
        {
            Debug.Log("Disconnected. " + disconnectReason);
            if (mySocket != null)
            {
                mySocket = null;
                IsConnected = false;
                IsConnectingToGameServer = false;

                //Observable
                OnDisconnectedFromGameServer(disconnectReason);
            }
        }

        public string GetConnectedEndPoint()
        {
            if (!IsConnected)
            {
                return "NOT CONNECTED";
            }

            return LastIp + ":" + LastPort;
        }
    
    }
}
