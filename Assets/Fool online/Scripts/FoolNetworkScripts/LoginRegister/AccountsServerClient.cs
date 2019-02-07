using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Fool_online.Scripts.FoolNetworkScripts.LoginRegister;
using Fool_online.Scripts;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using HybridWebSocket;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Fool_online.Scripts.FoolNetworkScripts
{
    /// <summary>
    /// When clients enters the game he connects to Accounts Server which will
    /// validate client's account data and game version and then send him to
    /// GameServer
    /// </summary>
    static class AccountsServerClient
    {
        public static bool IsConnectingToAccountsServer = false;
        public static bool IsConnected = false;

        private static readonly Queue<byte[]> bufferedRecievedBytes = new Queue<byte[]>();

        private static WebSocket mySocket;

        public static void Start(string serverIp, int serverPort)
        {
            //Observable
            //todo OnTryingConnectToGameServer();

            var request = new UnityWebRequest();
            request.method = "POST";
            request.url = "http://" + serverIp + ":" + serverPort + "/";
            request.SetRequestHeader("Client-version", Application.version);
            request.SetRequestHeader("Login", "anonymous");
            request.SetRequestHeader("Username", "test");
            var op = request.SendWebRequest();

            request.downloadHandler = new DownloadHandlerBuffer();
            while (!op.isDone)
            {
                Debug.Log(request.downloadHandler.text);
            }
            

            /*IsConnectingToAccountsServer = true;

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

            mySocket.Connect();*/
        }

        private static void OnOpen()
        {
            //todo FoolNetworkObservableCallbacksWrapper.Instance.ConnectedToAccountsServer();
            Debug.Log("Connected to accounts server");
            IsConnectingToAccountsServer = false;
            IsConnected = true;

            //CheckVersion();
        }

        private static void OnMessage(byte[] data)
        {
            //Buffer this message 
            bufferedRecievedBytes.Enqueue(data);
        }

        private static void OnError(string errormsg)
        {
            //throw new Exception(errormsg);
        }

        private static void OnClose(WebSocketCloseCode closecode)
        {
            Disconnect(closecode.ToString());

        }

        public static void Disconnect(string disconnectReason = null)
        {
            Debug.Log("Disconnected. " + disconnectReason);
            if (mySocket != null)
            {
                mySocket = null;
                IsConnected = false;
                IsConnectingToAccountsServer = false;
                //Observable
                //todo FoolNetworkObservableCallbacksWrapper.Instance.DisconnectedFromAccountsServer(disconnectReason);
            }
        }
    }
}
