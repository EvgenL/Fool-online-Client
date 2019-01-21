﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Xml;
using Fool_online.Scripts.Network.NetworksObserver;
using UnityEngine;

namespace Fool_online.Scripts.Network
{
    /// <summary>
    /// Connects to foll online server
    /// </summary>
//TODO implement heartbeat
    public class FoolTcpClient : FoolNetworkObservable
    {
        private const string CONFIG_FILE_NAME = "client.config";

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

        public string ServerIP = "51.75.236.170"; //TODO load from resources
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
            ConnectToGameServer();
            //Observable
            OnTryingConnectToGameServer();
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
                PlayerSocket.Close();
                PlayerSocket = null;
                return;
            }

            //Create and set up a new socket
            PlayerSocket = new System.Net.Sockets.TcpClient();
            PlayerSocket.ReceiveBufferSize = 4096;
            PlayerSocket.SendBufferSize = 4096;
            PlayerSocket.NoDelay = true;
            asynchByteBuffer = new byte[PlayerSocket.SendBufferSize + PlayerSocket.ReceiveBufferSize]; //= new byte[8192];

            try
            {
                //read from config file
                
                PlayerSocket.BeginConnect(ServerIP, ServerPort, OnConnectCallback, null);
            }
            catch (Exception e)
            {
                Debug.LogError(e);

            }
            finally
            {
                IsConnectingToGameServer = false;
            }
        }

        [Obsolete("This method in not used anymore")]
        private void ReadConfigFile()
        {
            FileStream clientconfig = null;
            try
            {
                //read file
                clientconfig = File.Open(CONFIG_FILE_NAME, FileMode.Open);
                StreamReader sr = new StreamReader(clientconfig);
                string xmlstring = sr.ReadToEnd();
                clientconfig.Close();

                //parse as xml
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlstring);

                XmlNodeList gameservers = doc.DocumentElement.SelectNodes("/Connection/Servers/Gameserver");

                foreach (XmlNode gameserver in gameservers)
                {
                    if (gameserver.Attributes["name"].Value == "default")
                    {
                        ServerIP = gameserver.ChildNodes[0].InnerText; //ip
                        ServerPort = int.Parse(gameserver.ChildNodes[1].InnerText); //port
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                clientconfig.Close();
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Callback that loops while the client is connected
        /// </summary>
        /// <param name="result"></param>
        private void OnConnectCallback(IAsyncResult result)
        {
            try
            {
                PlayerSocket.EndConnect(result);

                //if connection was destroyed while trying to connect
                if (PlayerSocket.Connected == false)
                {
                    return; //get disconnected
                }

                //Observable
                //OnConnectedToGameServer();
                FoolNetworkObservableCallbacksWrapper.Instance.ConnectedToGameServer();

                IsConnected = true;
                //Set socket up
                PlayerSocket.NoDelay = true;
                MyStream = PlayerSocket.GetStream();
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
        /// Callback for recieving data from connection
        /// </summary>
        /// <param name="result">Asynch result of operation </param>
        private void OnRecieveDataCallback(IAsyncResult result)
        {
            try
            {
                //Get data size
                int packetLength = MyStream.EndRead(result);
                recievedBytes = new byte[packetLength];

                //Copy data to my buffer
                Buffer.BlockCopy(asynchByteBuffer, 0, recievedBytes, 0, packetLength);

                //Buffer this message 
                bufferedRecievedBytes.Enqueue(recievedBytes);

                //if we are not getting any data from server, we are disconneting
                if (packetLength == 0)
                {
                    Disconnect("Сервер недоступен");
                    return;
                }
            }
            catch(Exception e) //if disconnected then exit
            {
                Disconnect("Сервер закрыт");
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
                //Observable
                FoolNetworkObservableCallbacksWrapper.Instance.DisconnectedFromGameServer(disconnectReason);
            }
        }
    
    }
}
