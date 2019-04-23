using Fool_online.Scripts.Manager;
using UnityEngine;

namespace Fool_online.Scripts.FoolNetworkScripts
{
    /// <summary>
    /// Main fool online networking class
    /// </summary>
    public class FoolNetwork
    {
        public enum DisconnectReason
        {
            ServerUnreachable,
            OldClientVersion
        }

        /// <summary>
        /// Me
        /// </summary>
        public static FoolPlayer LocalPlayer = new FoolPlayer();

        /// <summary>
        /// If was disconnected then theres would be buffered a reason description
        /// </summary>
        public static string DisconnectReasonText = null;

        /// <summary>
        /// If was disconnected then theres would be buffered a reason type
        /// </summary>
        public static DisconnectReason disconnectReason;

        /// <summary>
        /// Gets info of server to which we did connected
        /// </summary>
        public static string ServerInfo => FoolWebClient.GetConnectedEndPoint(); 

        public enum ConnectionState
        {
            ConnectingGameServer,
            Connected,
            Disconnected,
            //TODO states
            //SearchingForRoom,
            //JoiningRoom,
            //InRoom
        }

        /// <summary>
        /// Gets current state of connection
        /// </summary>
        public static ConnectionState connectionState
        {
            get
            {
                if (IsConnected)
                {
                    return ConnectionState.Connected;
                }

                if (client != null && FoolWebClient.IsConnectingToGameServer)
                {
                    return ConnectionState.ConnectingGameServer;
                }

                //if (client == null) or anything else happened
                    return ConnectionState.Disconnected;
            }
        }

        /// <summary>
        /// Should be tied to unity's update function and called every frame
        /// Used to synchronise with uinity main thread
        /// </summary>
        public static void Update()
        {
            if (client != null && FoolWebClient.IsConnected)
            {
                client.Update();
            }
        }

        public static bool IsConnected => (client != null && FoolWebClient.IsConnected);

        private static FoolWebClient client;

        /// <summary>
        /// Call to connect game.
        /// </summary>
        public static void ConnectToGameServer(string ip, int port, string authToken)
        {
            FoolWebClient.ConnectToGameServer(ip, port, authToken);
            client = FoolWebClient.Instance;
        }

        /// <summary>
        /// Tries to reconnect if connection was lost
        /// </summary>
        public static void Reconnect()
        {
            Debug.Log("Trying to reconnect...");

            if (FoolWebClient.IsConnected)
            {
                Debug.Log("Already connected!");
                return;
            }

            FoolWebClient.ReconnectToGameServer();
        }

        /// <summary>
        /// Call to disconnect from game.
        /// </summary>
        public static void Disconnect(string reason = null)
        {
            FoolWebClient.Disconnect(reason);
        }

        /// <summary>
        /// Call to join any room with no matchmaking.
        /// </summary>
        public static void JoinRandom()
        {
            ClientSendPackets.Send_JoinRandom();
        }

        public static void JoinRoom(long roomId)
        {
            ClientSendPackets.Send_JoinRoom(roomId);
            //todo show 'loading'
        }

        /// <summary>
        /// Give up a game.
        /// </summary>
        public static void GiveUp()
        {
            ClientSendPackets.Send_GiveUp();
            LocalPlayer.IsInRoom = false;
        }

        public static void LeaveRoom()
        {
            if (StaticRoomData.IsPlaying)
            {
                ClientSendPackets.Send_GiveUp();
            }
            else
            {
                ClientSendPackets.Send_LeaveRoom();
            }
            LocalPlayer.IsInRoom = false;
        }


    }
}
