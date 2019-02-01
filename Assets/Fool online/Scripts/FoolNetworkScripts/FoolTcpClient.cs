using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEditor;
using UnityEngine;

namespace Fool_online.Scripts.FoolNetworkScripts
{
    /// <summary>
    /// Connects to fool online server
    /// </summary>
//TODO implement heartbeat
    public class FoolTcpClient : FoolNetworkObservable
    {
        //public static string ClientVersion = "1.2"; //todo implement version check

        private static FoolTcpClient _instance;

        public static FoolTcpClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FoolTcpClient();
                }
                return _instance;
            }
        }
        //TODO load from resources
        //public string ServerIP = "127.0.0.1"; //localhost
        //public string ServerIP = "51.75.236.170"; //french
        public string ServerIP = "192.168.0.22"; //my pc
        public int ServerPort = 5055;
        public bool IsConnected = false;
        public bool IsConnectingToGameServer = false;

        //Flag: is data ready to be handled by main thread
        private byte[] recievedBytes;

        /// <summary>
        /// Sometimes we recieve more than 1 message per frame. We buffer all the recieved messages
        /// </summary>
        private Queue<byte[]> bufferedRecievedBytes = new Queue<byte[]>();


        private TcpClient PlayerSocket;
        private NetworkStream MyStream;

        private byte[] asynchByteBuffer;

        public void Start()
        {
            //Observable
            OnTryingConnectToGameServer();

            //Create a connection
            ConnectToGameServer();
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
        public void ConnectToGameServer()
        {
            IsConnectingToGameServer = true;

            //if player socket exist
            if (PlayerSocket != null)
            {
                //don't do anything if it's already connecter
                if (PlayerSocket.Connected || IsConnected)
                {
                    return;
                }

                //Else if it wasn't connected then destroy connection
                Disconnect();
                return;
            }

            //Create and set up a new socket
            PlayerSocket = new TcpClient
            {
                ReceiveBufferSize = 4096,
                SendBufferSize = 4096,
                NoDelay = true
            };
            asynchByteBuffer = new byte[PlayerSocket.SendBufferSize + PlayerSocket.ReceiveBufferSize]; //= new byte[8192];

            try
            {
                //todo availableServers = AvailableServerSearch.GetAvailableServers();
            }
            catch 
            {
                return;
            }


            try
            {

                //Unity web builds currently don't support multithreaded solution sadly
#if UNITY_WEBGL || UNITY_EDITOR_WIN

                //Singlethreaded
                PlayerSocket.Connect(ServerIP, ServerPort);
                ProcessConnection();
                UnityMainThreadDispatcher.Instance().StartCoroutine(RecieveDataInCoroutine());

#else

                //Multithreaded
                PlayerSocket.BeginConnect(ServerIP, ServerPort, OnConnectCallback, null);

#endif
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Disconnect();
            }
        }


#if UNITY_WEBGL || UNITY_EDITOR_WIN
        /// <summary>
        /// Singlethreaded
        /// Coroutine for managing recieving of data
        /// </summary>
        /// <returns></returns>
        private IEnumerator RecieveDataInCoroutine()
        {
            while (IsConnected)
            {
                try
                {
                    //if got some data
                    if (MyStream.DataAvailable)
                    {
                        //Get data size
                        int packetLength = MyStream.Read(asynchByteBuffer, 0, asynchByteBuffer.Length);
                        //process data
                        ProcessData(packetLength);
                    }
                    
                }
                catch //if disconnected then exit
                {
                    Disconnect("Потеряна связь с серверм");
                    yield break;
                }
                
                yield return null;
            }
        }
#else
        /// <summary>
        /// Multithreaded
        /// Callback that fires after client is connected
        /// </summary>
        /// <param name="result"></param>
        private void OnConnectCallback(IAsyncResult result)
        {
            try
            {
                PlayerSocket.EndConnect(result);

                ProcessConnection();

                MyStream.BeginRead(asynchByteBuffer, 0, asynchByteBuffer.Length, OnRecieveDataCallback, null);
            }
            catch (Exception e)
            {
                Debug.Log(e);

                //Здесь мы знаем что сервер неактивен

                Disconnect("Сервер недоступен");
            }
            finally
            {
                IsConnectingToGameServer = false;
            }
        }

        /// <summary>
        /// Multithreaded
        /// Callback for recieving data from connection
        /// </summary>
        /// <param name="result">Asynch result of operation </param>
        private void OnRecieveDataCallback(IAsyncResult result)
        {
            try
            {
                //Get data size
                int packetLength = MyStream.EndRead(result);
                //process data
                ProcessData(packetLength);
            }
            catch //if disconnected then exit
            {
                Disconnect("Потеряна связь с серверм");
                return;
            }

            //if was disconneted exit
            if (PlayerSocket == null)
            {
                return;
            }

            //Read another pack of data
            MyStream.BeginRead(asynchByteBuffer, 0, asynchByteBuffer.Length, OnRecieveDataCallback, null);
        
        }
#endif
        /// <summary>
        /// Singlethreaded/Multithreaded
        /// Callback for recieving data from connection
        /// </summary>
        private void ProcessData(int packetLength)
        {
            recievedBytes = new byte[packetLength];

            //Copy data to my buffer
            Buffer.BlockCopy(asynchByteBuffer, 0, recievedBytes, 0, packetLength);

            //Buffer this message 
            bufferedRecievedBytes.Enqueue(recievedBytes);

            //if we are not getting any data from server, we are disconneting
            if (packetLength == 0)
            {
                throw new Exception("Lost connection.");
            }
        }

        /// <summary>
        /// Singlethreaded/Multithreaded
        /// Sets all status and resource variables
        /// when conmnected by any (singlethreaded or multithreaded) method
        /// Sends callback to observers
        /// </summary>
        private void ProcessConnection()
        {
            //if connection was destroyed while trying to connect
            if (PlayerSocket.Connected == false)
            {
                throw new Exception("Not connected!"); //get disconnected
            }

            //Observable
            FoolNetworkObservableCallbacksWrapper.Instance.ConnectedToGameServer();

            IsConnected = true;
            //Set socket up
            PlayerSocket.NoDelay = true;
            MyStream = PlayerSocket.GetStream();
        }

        /// <summary>
        /// Performs write to server stream
        /// </summary>
        /// <param name="data">data to write to server</param>
        public static void WriteToStream(byte[] data)
        {
            Instance.MyStream.Write(data, 0, data.Length);
        }

        public void Disconnect(string disconnectReason = null)
        {
            Debug.Log("Disconnect. " + disconnectReason);
            if (PlayerSocket != null)
            {
                PlayerSocket.Close();
                PlayerSocket = null;
                IsConnected = false;
                IsConnectingToGameServer = false;
                //Observable
                FoolNetworkObservableCallbacksWrapper.Instance.DisconnectedFromGameServer(disconnectReason);
            }
        }
    
    }
}
