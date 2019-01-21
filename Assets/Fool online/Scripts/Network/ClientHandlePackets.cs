using System.Collections.Generic;
using Evgen.Byffer;
using Fool_online.Scripts.InRoom;
using Fool_online.Scripts.Network.NetworksObserver;
using UnityEngine;

namespace Fool_online.Scripts.Network
{
    /// <summary>
    /// Recieves data from server.
    /// </summary>
    public class ClientHandlePackets
    {
        /// <summary>
        /// Pacet id's. Gets converted to long and send at beginning of each packet
        /// Ctrl+C, Ctrl+V between ServerSendPackets on server and ClientHandlePackets on client
        /// </summary>
        public enum SevrerPacketId
        {
            //Connection
            Information = 1,

            //ROOMS
            JoinRoomOk,
            FaliToJoinFullRoom,
            YouAreAlreadyInRoom,
            RoomData,
            OtherPlayerJoinedRoom,
            OtherPlayerLeftRoom,
            OtherPlayerGotReady,
            OtherPlayerGotNotReady,

            //GAMEPLAY
            StartGame,
            NextTurn,
            YouGotCardsFromTalon,
            EnemyGotCardsFromTalon,
            TalonData,
            DropCardOnTableOk,
            DropCardOnTableErrorNotYourTurn,
            DropCardOnTableErrorTableIsFull,
            DropCardOnTableErrorCantDropThisCard,
            OtherPlayerDropsCardOnTable,
            EndGame,
            EndGameGiveUp,
            OtherPlayerPassed,
            OtherPlayerPickedUpCards,
            OtherPlayerCoversCard,
            Beaten,
            DefenderPicksCards,
            EndGameFool
        }


        public static ByteBuffer PlayerBuffer;

        private delegate void Packet(byte[] data);

        /// <summary>
        /// Packet-handle_method paits
        /// </summary>
        private static Dictionary<long, Packet> _packets;
        private static long _packetLength;

        /// <summary>
        /// Adds packet id's to _packets dictionary
        /// </summary>
        private static void InitPackets()
        {
            _packets = new Dictionary<long, Packet>();

            _packets.Add((long)SevrerPacketId.Information, Packet_Information);

            //ROOMS
            _packets.Add((long)SevrerPacketId.JoinRoomOk, Packet_JoinRoomOk);
            _packets.Add((long)SevrerPacketId.FaliToJoinFullRoom, Packet_FaliToJoinFullRoom);
            _packets.Add((long)SevrerPacketId.YouAreAlreadyInRoom, Packet_YouAreAlreadyInRoom);
            _packets.Add((long)SevrerPacketId.RoomData, Packet_RoomData);
            _packets.Add((long)SevrerPacketId.OtherPlayerJoinedRoom, Packet_OtherPlayerJoinedRoom);
            _packets.Add((long)SevrerPacketId.OtherPlayerLeftRoom, Packet_OtherPlayerLeftRoom);
            _packets.Add((long)SevrerPacketId.OtherPlayerGotReady, Packet_OtherPlayerGotReady);
            _packets.Add((long)SevrerPacketId.OtherPlayerGotNotReady, Packet_OtherPlayerGotNotReady);

            //GAMEPLAY
            _packets.Add((long)SevrerPacketId.StartGame, Packet_StartGame);
            _packets.Add((long)SevrerPacketId.NextTurn, Packet_NextTurn);
            _packets.Add((long)SevrerPacketId.YouGotCardsFromTalon, Packet_YouGotCardsFromTalon);
            _packets.Add((long)SevrerPacketId.EnemyGotCardsFromTalon, Packet_EnemyGotCardsFromTalon);
            _packets.Add((long)SevrerPacketId.TalonData, Packet_TalonData);
            _packets.Add((long)SevrerPacketId.DropCardOnTableOk, Packet_DropCardOnTableOk);
            _packets.Add((long)SevrerPacketId.DropCardOnTableErrorNotYourTurn, Packet_DropCardOnTableErrorNotYourTurn);
            _packets.Add((long)SevrerPacketId.DropCardOnTableErrorTableIsFull, Packet_DropCardOnTableErrorTableIsFull);
            _packets.Add((long)SevrerPacketId.DropCardOnTableErrorCantDropThisCard, Packet_DropCardOnTableErrorCantDropThisCard);
            _packets.Add((long)SevrerPacketId.OtherPlayerDropsCardOnTable, Packet_OtherPlayerDropsCardOnTable);
            _packets.Add((long)SevrerPacketId.EndGame, Packet_EndGame);
            _packets.Add((long)SevrerPacketId.EndGameGiveUp, Packet_EndGameGiveUp);
            _packets.Add((long)SevrerPacketId.OtherPlayerPassed, Packet_OtherPlayerPassed);
            _packets.Add((long)SevrerPacketId.OtherPlayerPickedUpCards, Packet_OtherPlayerPickedUpCards);
            _packets.Add((long)SevrerPacketId.OtherPlayerCoversCard, Packet_OtherPlayerCoversCard);
            _packets.Add((long)SevrerPacketId.Beaten, Packet_Beaten);
            _packets.Add((long)SevrerPacketId.DefenderPicksCards, Packet_DefenderPicksCards);
            _packets.Add((long)SevrerPacketId.EndGameFool, Packet_EndGameFool);
        }

        /// <summary>
        /// Handles recieved packet. Called by FoolTcpClient Update->if(_shouldHandleData)
        /// Handles data sent by client represented by an array of bytes.
        /// </summary>
        /// <param name="data">bytes of data</param>
        public static void HandleData(byte[] data)
        {
            //init messages if first call
            if (_packets == null)
            {
                InitPackets();
            }

            byte[] buffer;
            buffer = (byte[]) data.Clone();

            if (PlayerBuffer == null)
            {
                PlayerBuffer = new ByteBuffer();
            }

            PlayerBuffer.WriteBytes(buffer);

            //if not sending any data
            if (PlayerBuffer.Count() == 0)
            {
                PlayerBuffer.Clear();
                return;
            }

            //if we got packet
            //Read packet length
            if (PlayerBuffer.Length() >= 8)
            {
                _packetLength = PlayerBuffer.ReadLong(false);

                if (_packetLength <= 0)
                {
                    PlayerBuffer.Clear();
                    return; //Packet is not complete!
                }
            }

            //Reding a packet
            while (_packetLength > 0 && _packetLength <= PlayerBuffer.Length() - 8)
            {
                if (_packetLength <= PlayerBuffer.Length() - 8)
                {
                    PlayerBuffer.ReadLong(); //Reads out packet id to skip read position
                    data = PlayerBuffer.ReadBytes((int) _packetLength); //Gets full package length

                    HandleDataPackets(data);
                }

                _packetLength = 0;

                if (PlayerBuffer.Length() >= 8)
                {
                    _packetLength = PlayerBuffer.ReadLong(false);

                    if (_packetLength <= 0)
                    {
                        PlayerBuffer.Clear();
                        return; 
                    }
                }
            }
        }

        /// <summary>
        /// Called by HandleData
        /// Proceeds data by calling Packet_MethodName methods
        /// </summary>
        private static void HandleDataPackets(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer(); //using it only to convert data[0-7] to long var
            buffer.WriteBytes(data);

            //Reading out the packet id
            long packetId = buffer.ReadLong();
            //delete buffer from memory
            buffer.Dispose(); 
            buffer = null;

            Packet packet;
            //Try find function tied to this packet id
            if (_packets.TryGetValue(packetId, out packet))
            {
                //Call method tied to a Packet by InitPackets() method
                packet.Invoke(data);
            }
        }


        ////////////////////////////DATA PACKETS////////////////////////////
        ////////////////////////////DATA PACKETS////////////////////////////
        ////////////////////////////DATA PACKETS////////////////////////////

        /// <summary>
        /// Handles SevrerPacketId.Information
        /// </summary>
        private static void Packet_Information(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //skip read pakcet id
            buffer.ReadLong();

            //Read my conncetion id
            long connectionId = buffer.ReadLong();
            FoolNetwork.LocalPlayer.ConnectionId = connectionId;

            //Read message
            string msg = buffer.ReadString();

            Debug.Log($"Connected. Your connection id = " + connectionId + ". Server says: " + msg);

            ClientSendPackets.Send_ThankYou();
        }

        /// <summary>
        /// Handles SevrerPacketId.Information
        /// </summary>
        private static void Packet_CallMethod(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            long packetId = buffer.ReadLong();

            long methodId = buffer.ReadLong();

            Debug.Log("got Packet_CallMethod");
        }


        /// <summary>
        /// Handles SevrerPacketId.JoinRoomOk
        /// </summary>
        private static void Packet_JoinRoomOk(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            long packetId = buffer.ReadLong();

            long roomId = buffer.ReadLong();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.JoinRoom(roomId);

            Debug.Log("You joined room: " + roomId);
        }

        /// <summary>
        /// Handles SevrerPacketId.FaliToJoinFullRoom
        /// </summary>
        private static void Packet_FaliToJoinFullRoom(byte[] data)
        {
            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.FailedToJoinFullRoom();

            Debug.Log("Failed to join room: it's full");
        }
        
        /// <summary>
        /// Handles SevrerPacketId.YouAreAlreadyInRoom
        /// </summary>
        private static void Packet_YouAreAlreadyInRoom(byte[] data)
        {
            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.YouAreAlreadyInRoom();

            Debug.Log("got Packet_YouAreAlreadyInRoom");
        }

        /// <summary>
        /// Handles SevrerPacketId.RoomData
        /// </summary>
        private static void Packet_RoomData(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Read packet id
            long packetId = buffer.ReadLong();

            //Read players count
            int playersCount = buffer.ReadInteger();

            //Init player array
            List<long> playerIdsInRoom = new List<long>();
            //Init chair dict
            Dictionary<int, long> slots = new Dictionary<int, long>();

            //Read players
            for (int i = 0; i < playersCount; i++)
            {
                //Read player id
                long playerId = buffer.ReadLong();
                playerIdsInRoom.Add(playerId);
                //Read slots number
                int slotN = buffer.ReadInteger();
                slots.Add(slotN, playerIdsInRoom[i]);
                
                //if that part of data is about out player then set InRoomSlotNumber to read value
                if (playerId == FoolNetwork.LocalPlayer.ConnectionId)
                {
                    FoolNetwork.LocalPlayer.InRoomSlotNumber = slotN;
                }
            }

            //Read maxPlayers
            int maxPlayers = buffer.ReadInteger();

            StaticRoomData.ConnectedPlayersCount = playersCount;
            StaticRoomData.PlayerIds = playerIdsInRoom;
            StaticRoomData.OccupiedSlots = slots;
            StaticRoomData.MaxPalyers = maxPlayers;

            StaticRoomData.Players = new PlayerInRoom[maxPlayers];

            for (int i = 0; i < maxPlayers; i++)
            {
                if (slots.ContainsKey(i))
                {
                    PlayerInRoom pl = new PlayerInRoom(slots[i]);
                    StaticRoomData.Players[i] = pl;
                }
            }

            StaticRoomData.DataReady = true;

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.RoomDataUpdated();

        }


        /// <summary>
        /// Handles SevrerPacketId.OtherPlayerJoinedRoom
        /// </summary>
        private static void Packet_OtherPlayerJoinedRoom(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read id of newly joined player
            long joinedPlayerId = buffer.ReadLong();

            //Read slotN of newly joined player
            int slotN = buffer.ReadInteger();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.OtherPlayerJoinedRoom(joinedPlayerId, slotN);
        }

        /// <summary>
        /// Handles SevrerPacketId.Packet_OtherPlayerLeftRoom
        /// </summary>
        private static void Packet_OtherPlayerLeftRoom(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read id of player who left
            long leftPlayerId = buffer.ReadLong();

            //Read slotN of newly joined player
            int slotN = buffer.ReadInteger();

            Debug.Log("Player left: " + leftPlayerId);

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.OtherPlayerLeftRoom(leftPlayerId, slotN);
        }

        /// <summary>
        /// Handles SevrerPacketId.Packet_OtherPlayerLeftRoom
        /// </summary>
        private static void Packet_OtherPlayerGotReady(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read id of player
            long playerId = buffer.ReadLong();

            //Read slotN of player
            int slotN = buffer.ReadInteger();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.OtherPlayerGotReady(playerId, slotN);
        }

        /// <summary>
        /// Handles SevrerPacketId.Packet_OtherPlayerLeftRoom
        /// </summary>
        private static void Packet_OtherPlayerGotNotReady(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read id of player
            long playerId = buffer.ReadLong();

            //Read slotN of player
            int slotN = buffer.ReadInteger();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.OtherPlayerGotNotReady(playerId, slotN);
        }


        /// <summary>
        /// Handles SevrerPacketId.Packet_YouGotCards
        /// </summary>
        private static void Packet_YouGotCardsFromTalon(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read cards number
            int cardsN = buffer.ReadInteger();

            //Read cards
            string[] cards = new string[cardsN];
            for (int i = 0; i < cardsN; i++)
            {
                cards[i] = buffer.ReadString();
            }

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.YouGotCardsFromTalon(cards);
        }

        /// <summary>
        /// Handles SevrerPacketId.Packet_EnemyGotCards
        /// </summary>
        private static void Packet_EnemyGotCardsFromTalon(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read player id
            long playerId = buffer.ReadLong();
            //Read player cardsN
            int cardsN = buffer.ReadInteger();
            //Read slotN of player
            int slotN = buffer.ReadInteger();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.EnemyGotCardsFromTalon(playerId, slotN, cardsN);
        }

        private static void Packet_StartGame(byte[] data)
        {
            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.StartGame();
        }

        private static void Packet_NextTurn(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read player id
            long firstPlayerId = buffer.ReadLong();
            //Read player slotN
            int slotN = buffer.ReadInteger();
            //Read def player id
            long defendingPlayerId = buffer.ReadLong();
            //Read def player slotN
            int defSlotN = buffer.ReadInteger();
            //Read turn number
            int turnN = buffer.ReadInteger();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.NextTurn(firstPlayerId, slotN, defendingPlayerId, defSlotN, turnN);
        }

        /// <summary>
        /// Handles SevrerPacketId.Packet_TalonData
        /// </summary>
        private static void Packet_TalonData(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read talon len
            int talonLength = buffer.ReadInteger();

            //Read trump card
            string trumpCard = buffer.ReadString();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.TalonData(talonLength, trumpCard);
        }

        private static void Packet_DropCardOnTableOk(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //read card
            string cardCode = buffer.ReadString();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.DropCardOnTableOk(cardCode);
        }

        private static void Packet_DropCardOnTableErrorNotYourTurn(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //read card
            string cardCode = buffer.ReadString();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.DropCardOnTableErrorNotYourTurn(cardCode);
        }

        private static void Packet_DropCardOnTableErrorTableIsFull(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //read card
            string cardCode = buffer.ReadString();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.DropCardOnTableErrorTableIsFull(cardCode);
        }

        private static void Packet_DropCardOnTableErrorCantDropThisCard(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //read card
            string cardCode = buffer.ReadString();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.DropCardOnTableErrorCantDropThisCard(cardCode);
        }

        private static void Packet_OtherPlayerDropsCardOnTable(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read player who drop
            long playerId = buffer.ReadLong();
            //Read his slot
            int slotN = buffer.ReadInteger();
            //read card
            string cardCode = buffer.ReadString();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.OtherPlayerDropsCardOnTable(playerId, slotN, cardCode);
        }

        private static void Packet_EndGame(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read player who is fool
            long foolPlayerId = buffer.ReadLong();
            //Read rewards count
            int rewardsN = buffer.ReadInteger();
            //read rewards
            Dictionary<long, int> rewards = new Dictionary<long, int>(rewardsN);
            for (int i = 0; i < rewardsN; i++)
            {
                long player = buffer.ReadLong();
                int reward = buffer.ReadInteger();
                rewards.Add(player, reward);
            }

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.EndGame(foolPlayerId, rewards);
        }

        private static void Packet_EndGameGiveUp(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read player who is fool
            long foolPlayerId = buffer.ReadLong();
            //Read rewards count
            int rewardsN = buffer.ReadInteger();
            //read rewards
            Dictionary<long, int> rewards = new Dictionary<long, int>(rewardsN);
            for (int i = 0; i < rewardsN; i++)
            {
                long player = buffer.ReadLong();
                int reward = buffer.ReadInteger();
                rewards.Add(player, reward);
            }

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.EndGameGiveUp(foolPlayerId, rewards);
        }

        private static void Packet_OtherPlayerPassed(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read player who passed
            long passedPlayerId = buffer.ReadLong();
            //Read slotN
            int slotN = buffer.ReadInteger();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.OtherPlayerPassed(passedPlayerId, slotN);
        }

        private static void Packet_OtherPlayerPickedUpCards(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read player who picked
            long pickedPlayerId = buffer.ReadLong();
            //Read slotN
            int slotN = buffer.ReadInteger();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.OtherPlayerPickedUpCards(pickedPlayerId, slotN);
        }

        private static void Packet_OtherPlayerCoversCard(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read player who covered
            long coveredPlayerId = buffer.ReadLong();
            //Read his slotN
            int slotN = buffer.ReadInteger();

            //read card on table
            string cardOnTableCode = buffer.ReadString();
            //read card dropped
            string cardDroppedCode = buffer.ReadString();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance
                .OtherPlayerCoversCard(coveredPlayerId, slotN, cardOnTableCode, cardDroppedCode);
        }

        private static void Packet_Beaten(byte[] data)
        {
            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.Beaten();
        }

        private static void Packet_DefenderPicksCards(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read player who picks
            long playerId = buffer.ReadLong();
            //Read his slotN
            int slotN = buffer.ReadInteger();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.DefenderPicksCards(playerId, slotN);
        }
        
        private static void Packet_EndGameFool(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            //Skip packetId
            buffer.ReadLong();

            //Read player who fool
            long foolPlayerId = buffer.ReadLong();

            //Invoke callback on observers
            FoolNetworkObservableCallbacksWrapper.Instance.EndGameFool(foolPlayerId);
        }
    }
}
