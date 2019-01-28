namespace Fool_online.Scripts.Network
{
    /// <summary>
    /// Main fool online networking class
    /// </summary>
    public class FoolNetwork
    {
        /// <summary>
        /// Me
        /// </summary>
        public static FoolPlayer LocalPlayer = new FoolPlayer();

        /// <summary>
        /// If was disconnected then theres would be a reason
        /// </summary>
        public static string DisconnectReason = null;

        /// <summary>
        /// For debugging
        /// </summary>
        public static bool ConnectToLocalhost = false;

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

        public static bool IsConnected => (client != null && FoolTcpClient.Instance.IsConnected);

        private static FoolTcpClient client;

        /// <summary>
        /// Call to connect game.
        /// </summary>
        public static void ConnectToGameServer()
        {
            FoolTcpClient.Instance.Start();
            client = FoolTcpClient.Instance;
        }

        /// <summary>
        /// Call to disconnect from game.
        /// </summary>
        public static void Disconnect(string reason = null)
        {
            FoolTcpClient.Instance.Disconnect(reason);
        }

        /// <summary>
        /// Call to join any room with no matchmaking.
        /// </summary>
        public static void JoinRandom()
        {
            ClientSendPackets.Send_JoinRandom();
        }

        /// <summary>
        /// Give up a game.
        /// </summary>
        public static void GiveUp()
        {
            ClientSendPackets.Send_GiveUp();
            LocalPlayer.IsInRoom = false;
        }


    }
}
