using Evgen.Byffer;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;

namespace Fool_online.Scripts.FoolNetworkScripts
{
    public static class ClientSendPackets
    {
        /// <summary>
        /// Packet id's. Gets converted to long and send at beginning of each packet
        /// Ctrl+C, Ctrl+V between ServerHandlePackets on server and ClientSendPackets on client
        /// </summary>
        private enum ClientPacketId
        {
            NewAccount = 1,
            Login,
            ThankYou,
            
            

            //ROOMS
            CreateRoom,
            RefreshRoomList,
            JoinRandom,
            GiveUp,
            GetReady,
            GetNotReady,

            //GAMEPLAY
            DropCardOnTable,
            Pass,
            CoverCardOnTable,
        }


        /// <summary>
        /// Sends byte array to server.
        /// </summary>
        /// <param name="data">bytes to send</param>
        private static void SendDataToServer(byte[] data)
        {
            if (FoolTcpClient.Instance == null || !FoolTcpClient.Instance.IsConnected)
            {
                FoolNetworkObservableCallbacksWrapper.Instance.DisconnectedFromGameServer("Ошибка при отправке сообщения на сервер.");
            }

            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong(data.Length);
            buffer.WriteBytes(data);
            FoolTcpClient.WriteToStream(buffer.ToArray());
            buffer = null;
        }

        /// <summary>
        /// Sends message containing only packet id
        /// </summary>
        private static void SendOnlyPacketId(ClientPacketId packetId)
        {
            ByteBuffer buffer = new ByteBuffer();

            buffer.WriteLong((long)packetId);

            SendDataToServer(buffer.ToArray());
        }

        /// <summary>
        /// Sends user account to server on registration.
        /// </summary>
        public static void Send_NewAccount(string username, string password, string email)
        {
            ByteBuffer buffer = new ByteBuffer();

            buffer.WriteLong((long)ClientPacketId.NewAccount); //=1
            buffer.WriteString(username);
            buffer.WriteString(password);
            buffer.WriteString(email);

            SendDataToServer(buffer.ToArray());
        }

        /// <summary>
        /// Tutorial function //TODO remove
        /// </summary>
        public static void Send_ThankYou()
        {
            ByteBuffer buffer = new ByteBuffer();

            buffer.WriteLong((long)ClientPacketId.ThankYou);
            buffer.WriteString("Hello! I'm on server now.");

            SendDataToServer(buffer.ToArray());
        }



        /// <summary>
        /// Sent when cover some card on table
        /// </summary>
        public static void Send_CreateRoom(int maxPlayers, int deckSize)
        {
            ByteBuffer buffer = new ByteBuffer();

            //Write packet id
            buffer.WriteLong((long)ClientPacketId.CreateRoom);

            //write max players
            buffer.WriteInteger(maxPlayers);
            //write deck size
            buffer.WriteInteger(deckSize);

            SendDataToServer(buffer.ToArray());
        }

        /// <summary>
        /// Sent when cover some card on table
        /// </summary>
        public static void Send_RefreshRoomList()
        {
            SendOnlyPacketId(ClientPacketId.RefreshRoomList);
        }

        /// <summary>
        /// I want to join any room without parameters
        /// </summary>
        public static void Send_JoinRandom()
        {
            SendOnlyPacketId(ClientPacketId.JoinRandom);
        }

        /// <summary>
        /// I want to give up a game
        /// </summary>
        public static void Send_GiveUp()
        {
            SendOnlyPacketId(ClientPacketId.GiveUp);
        }

        /// <summary>
        /// Sent when i click 'ready' button in room
        /// </summary>
        public static void Send_GetReady()
        {
            SendOnlyPacketId(ClientPacketId.GetReady);
        }

        /// <summary>
        /// Sent when i click 'ready' button in room
        /// </summary>
        public static void Send_GetNotReady()
        {
            SendOnlyPacketId(ClientPacketId.GetNotReady);
        }

        /// <summary>
        /// Sent when i click drop card on table
        /// </summary>
        public static void Send_DropCardOnTable(string cardCode)
        {
            ByteBuffer buffer = new ByteBuffer();

            //Add packet id
            buffer.WriteLong((long)ClientPacketId.DropCardOnTable);
            //Add card
            buffer.WriteString(cardCode);

            SendDataToServer(buffer.ToArray());
        }

        /// <summary>
        /// Sent when i click pass button in room
        /// </summary>
        public static void Send_Pass()
        {
            SendOnlyPacketId(ClientPacketId.Pass);
        }

        /// <summary>
        /// Sent when cover some card on table
        /// </summary>
        public static void Send_CoverCardOnTable(string cardCodeOnTable, string cardCodeDropped)
        {
            ByteBuffer buffer = new ByteBuffer();

            //Write packet id
            buffer.WriteLong((long)ClientPacketId.CoverCardOnTable);

            //write card on table
            buffer.WriteString(cardCodeOnTable);
            //write card dropped
            buffer.WriteString(cardCodeDropped);

            SendDataToServer(buffer.ToArray());
        }
    }
}
