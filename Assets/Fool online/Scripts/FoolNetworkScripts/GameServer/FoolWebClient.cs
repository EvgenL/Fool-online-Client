using System;
using System.Collections.Generic;
using Assets.Fool_online.Scripts.FoolNetworkScripts;
using Assets.Fool_online.Scripts.FoolNetworkScripts.GameServer;
using Fool_online.Plugins;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;

namespace Fool_online.Scripts.FoolNetworkScripts
{
    /// <summary>
    /// Connects to fool online server
    /// </summary>
    public class FoolWebClient : FoolObservable
    {
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

        private static string LastIp;
        private static int LastPort;

        private static string myToken;

        public static bool IsConnected = false;
        public static bool IsConnectingToGameServer = false;

        /// <summary>
        /// Sometimes we recieve more than 1 message per frame. We buffer all the recieved messages
        /// and process them later on in Update()
        /// </summary>
        private static readonly Queue<byte[]> bufferedRecievedMessages = new Queue<byte[]>();


        private static WebSocket mySocket;

        /// <summary>
        /// Initialize conenction between game server and client
        /// </summary>
        public static void ConnectToGameServer(string ip, int port, string authToken)
        {
            if (IsConnectingToGameServer)
            {
                Debug.LogWarning("Trying to connect while already connecting. Aborting.");
                return;
            }

            //Observable
            OnTryingConnectToGameServer();
            
            //Create a connection
            IsConnectingToGameServer = true;

            // buffer connection data
            LastIp = ip;
            LastPort = port;
            myToken = authToken;

            //if player socket exist
            if (mySocket != null)
            {
                //don't do anything if it's already connecter
                if (IsConnected)
                {
                    throw new Exception("Error: Trying to connect while already connected.");
                }

                throw new Exception("Socket exists but not connected. This should not happen");
                //Else if it wasn't connected then destroy connection
                Disconnect();
                return;
            }

            //Create and set up a new socket
            mySocket = WebSocketFactory.CreateInstance("ws://" + ip + ":" + port);

            //Init callbacks
            mySocket.OnOpen += OnOpen;
            mySocket.OnMessage += OnMessage;
            mySocket.OnError += OnError;
            mySocket.OnClose += OnClose;

            // connect
            mySocket.Connect();
        }

        /// <summary>
        /// Connect using last succesfull ip end point
        /// </summary>
        public static void ReconnectToGameServer()
        {
            ConnectToGameServer(LastIp, LastPort, myToken);
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
        
        private static void OnOpen()
        {
            // Notify ConnectionWatchdog that connection is ok
            ConnectionWatchdog.OnOpen();

            // observable
            FoolObservable.OnConnectedToGameServer();

            IsConnectingToGameServer = false;
            IsConnected = true;

            //authorize with token
            ClientSendPackets.Send_Authorize(myToken);
        }

        private static void OnMessage(byte[] data)
        {
            //Buffer this message 
            bufferedRecievedMessages.Enqueue(data);
        }

        private static void OnError(string errormsg)
        {
            Debug.LogWarning("OnError: " + errormsg);
        }

        private static void OnClose(WebSocketCloseCode closecode)
        {
            Disconnect(closecode.ToString());

            // Notify ConnectionWatchdog that connection was lost
            ConnectionWatchdog.OnClose(closecode);
        }

        /// <summary>
        /// Performs write to server stream
        /// </summary>
        /// <param name="data">data to write to server</param>
        public static void WriteToServer(byte[] data)
        {
            if (IsConnected && mySocket != null)
            {
                mySocket.Send(data);
            }
            else
            {
                // observable
                FoolObservable.OnSendError();
                UnityEngine.Debug.LogWarning("Can't send data to server: Not connected.");

                FoolNetwork.Reconnect();
            }
        }

        public static void Disconnect(string disconnectReason = null)
        {
            Debug.Log("Disconnected. " + disconnectReason);
            if (mySocket != null)
            {
                // close socket if was open
                // (true if Disconnect() called by user's will)
                // (false if connection was suddenly lost)
                if (mySocket.GetState() == WebSocketState.Open)
                {
                    mySocket.Close(WebSocketCloseCode.Normal);
                }

                mySocket = null;
                IsConnected = false;
                IsConnectingToGameServer = false;

                //Observable
                FoolObservable.OnDisconnectedFromGameServer(disconnectReason); //вызывает исключение
            }
        }

        public static string GetConnectedEndPoint()
        {
            if (!IsConnected)
            {
                return "NOT CONNECTED";
            }

            return LastIp + ":" + LastPort;
        }
    
    }
}
