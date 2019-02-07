using UnityEditor;

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
        public static string ServerInfo => FoolWebClient.Instance.GetConnectedEndPoint(); 

        public enum ConnectionState
        {
            ConnectingGameServer,
            Connected,
            Disconnected,
            //TODO states
            //SearchingForRoom,
            //JoiningRoom,
            //ConnectedInRoom
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

                if (client != null && client.IsConnectingToGameServer)
                {
                    return ConnectionState.ConnectingGameServer;
                }

                //if (client == null) or anything else happened
                    return ConnectionState.Disconnected;
            }
        }

        public static void Update()
        {
            if (client != null && client.IsConnected)
            {
                client.Update();
            }
        }

        public static bool IsConnected => (client != null && FoolWebClient.Instance.IsConnected);

        private static FoolWebClient client;

        /// <summary>
        /// Call to connect game.
        /// </summary>
        public static void ConnectToGameServer(string ip, int port, string authToken)
        {
            FoolWebClient.Instance.Start(ip, port, authToken);
            client = FoolWebClient.Instance;
        }

        /// <summary>
        /// Call to disconnect from game.
        /// </summary>
        public static void Disconnect(string reason = null)
        {
            FoolWebClient.Instance.Disconnect(reason);
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
            ClientSendPackets.Send_LeaveRoom();
            LocalPlayer.IsInRoom = false;
        }


    }
}
