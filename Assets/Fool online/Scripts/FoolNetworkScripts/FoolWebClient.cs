using System;
using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using HybridWebSocket;
using UnityEngine;

namespace Fool_online.Scripts.FoolNetworkScripts
{
    /// <summary>
    /// Connects to fool online server
    /// </summary>
//TODO implement heartbeat
    public class FoolWebClient : FoolNetworkObservable
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

        //TODO load from file
        //public string ServerIP = "127.0.0.1"; //localhost
        public string ServerIP = "51.75.236.170"; //french
        public string LocalServerIP = "192.168.0.22"; //my pc
        public int ServerPort = 5055;

        public bool IsConnected = false;
        public bool IsConnectingToGameServer = false;

        //Flag: is data ready to be handled by main thread
        private byte[] recievedBytes;

        /// <summary>
        /// Sometimes we recieve more than 1 message per frame. We buffer all the recieved messages
        /// and process them later on in Update()
        /// </summary>
        private readonly Queue<byte[]> bufferedRecievedBytes = new Queue<byte[]>();


        private WebSocket mySocket;

        //private TcpClient PlayerSocket;
        //private NetworkStream MyStream;

        public void Start()
        {
            //Observable
            OnTryingConnectToGameServer();

            //Create a connection
            ConnectToGameServer(ServerIP, ServerPort);
        }

        /// <summary>
        /// This shuold be called on unity update callback used to handle data only from main thread
        /// </summary>
        public void Update()
        {
            while (bufferedRecievedBytes.Count > 0)
            {
                ClientHandlePackets.HandleData(bufferedRecievedBytes.Dequeue());
            }
        }

        /// <summary>
        /// Connects this player to game server
        /// </summary>
        public void ConnectToGameServer(string serverIp, int serverPort)
        {
            IsConnectingToGameServer = true;

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
            FoolNetworkObservableCallbacksWrapper.Instance.ConnectedToGameServer();
            IsConnectingToGameServer = false;
            IsConnected = true;
        }

        private void OnMessage(byte[] data)
        {
            //Buffer this message 
            bufferedRecievedBytes.Enqueue(data);
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
                FoolNetworkObservableCallbacksWrapper.Instance.DisconnectedFromGameServer(disconnectReason);
            }
        }

        public string GetServerInfo()
        {
            if (!IsConnected)
            {
                return "NOT CONNECTED";
            }

            return ServerIP + ":" + ServerPort;
        }
    
    }
}
