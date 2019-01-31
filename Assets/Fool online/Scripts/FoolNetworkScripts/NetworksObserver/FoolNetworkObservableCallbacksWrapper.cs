using System.Collections.Generic;
using Fool_online.Scripts.InRoom;
using Fool_online.Scripts.InRoom.CardsScripts;
using Fool_online.Scripts.Manager;
using Fool_online.Ui.Mainmenu;
using UnityEngine;

namespace Fool_online.Scripts.FoolNetworkScripts.NetworksObserver
{
    /// <summary>
    /// IHateSuchLongClassNames
    /// Implementation of FoolNetworkObservable.
    /// Call methods by FoolNetworkObservableCallbacks.Instance to invoke callbacks on observer
    /// This basically is a wrapper who does things needed to be done before sending callbacks to observers.
    /// Used mostly by ClientHandlePackets
    /// </summary>
    public class FoolNetworkObservableCallbacksWrapper : FoolNetworkObservable
    {

        #region Thread-safe Singleton

        private static object padlock = new object();

        public static FoolNetworkObservableCallbacksWrapper Instance
        {
            get
            {
                lock (padlock)
                {
                if (_instance == null)
                {
                    _instance = new FoolNetworkObservableCallbacksWrapper();
                }

                return _instance;
                }
            }
        }

        private static FoolNetworkObservableCallbacksWrapper _instance;

        #endregion

        /// <summary>
        /// Called by handlePackets on joining room
        /// </summary>
        /// <param name="roomId">roomId</param>
        public void JoinRoom(long roomId)
        {
            //Set player status to inRoom
            FoolNetwork.LocalPlayer.IsInRoom = true;
            FoolNetwork.LocalPlayer.RoomId = roomId;

            //Observable
            OnJoinRoom();
        }

        /// <summary>
        /// Called by handlePackets or client logic on leaving room
        /// </summary>
        public void LeaveRoom()
        {
            //Set player status to inRoom
            FoolNetwork.LocalPlayer.IsInRoom = false;
            FoolNetwork.LocalPlayer.RoomId = 0;

            //Observable
            OnLeaveRoom();
        }

        /// <summary>
        /// Called by handlePackets on trying join full room
        /// </summary>
        public void FailedToJoinFullRoom()
        {
            //Set player status to inRoom
            FoolNetwork.LocalPlayer.IsInRoom = false;
            FoolNetwork.LocalPlayer.RoomId = 0;

            //Observable
            OnFailedToJoinFullRoom();
        }

        /// <summary>
        /// Called by handlePackets on trying join same room
        /// </summary>
        public void YouAreAlreadyInRoom()
        {
            //Observable
            OnYouAreAlreadyInRoom();
        }

        /// <summary>
        /// Called on connected
        /// </summary>
        public void ConnectedToGameServer()
        {
            //Observable
            OnConnectedToGameServer();
        }

        /// <summary>
        /// Called by FoolTcpClient on leave server
        /// </summary>
        /// <param name="reason">reason if was kicked by server</param>
        public void DisconnectedFromGameServer(string reason = null)
        {
            //Set player status to inRoom
            FoolNetwork.DisconnectReason = reason;
            FoolNetwork.Disconnect();

            //Observable
            OnDisconnectedFromGameServer();
        }

        public void RoomDataUpdated()
        {
            //Observable
            OnRoomData();
        }

        public void OtherPlayerJoinedRoom(long joinedPlayerId, int slotN)
        {
            StaticRoomData.ConnectedPlayersCount++;
            string s = "StaticRoomData.OccupiedSlots = {";
            foreach (var slot in StaticRoomData.OccupiedSlots)
            {
                s += $" [slot {slot.Key}, player {slot.Value}] ";
            }
            s += "}";
            Debug.Log("OtherPlayerJoinedRoom");
            Debug.Log(s);

            StaticRoomData.OccupiedSlots.Add(slotN, joinedPlayerId);
            StaticRoomData.PlayerIds.Add(joinedPlayerId);

            PlayerInRoom pl = new PlayerInRoom(joinedPlayerId);
            pl.SlotN = slotN;
            StaticRoomData.Players[slotN] = pl;

            //Observable
            OnRoomData();

            //Observable
            OnOtherPlayerJoinedRoom(joinedPlayerId, slotN);
        }

        public void OtherPlayerLeftRoom(long leftPlayerId, int slotN)
        {
            StaticRoomData.ConnectedPlayersCount--;
            StaticRoomData.OccupiedSlots.Remove(slotN);
            StaticRoomData.PlayerIds.Remove(leftPlayerId);

            StaticRoomData.Players[slotN] = null;

            foreach (var player in StaticRoomData.Players)
            {
                if (player != null)
                {
                    player.IsReady = false;
                }
            }

            //Observable
            OnRoomData();

            //Observable
            OnOtherPlayerLeftRoom(leftPlayerId, slotN);
        }

        public void OtherPlayerGotReady(long playerId, int slotN)
        {
            StaticRoomData.Players[slotN].IsReady = true;

            //Observable
            OnOtherPlayerGotReady(playerId, slotN);
        }

        public void OtherPlayerGotNotReady(long playerId, int slotN)
        {
            StaticRoomData.Players[slotN].IsReady = false;

            //Observable
            OnOtherPlayerGotNotReady(playerId, slotN);
        }

        public void YouGotCardsFromTalon(string[] cards)
        {
            StaticRoomData.MyPlayer.TakeCards(cards);

            //Observable
            OnYouGotCardsFromTalon(cards);
        }

        public void EnemyGotCardsFromTalon(long playerId, int slotN, int cardsN)
        {
            StaticRoomData.Players[slotN].TakeCards(cardsN);
            //Observable
            OnEnemyGotCardsFromTalon(playerId, slotN, cardsN);
        }

        public void StartGame()
        {
            foreach (var player in StaticRoomData.Players)
            {
                player.Won = false;
                player.Pass = false;
                player.IsReady = false;
            }

            //Observable
            OnStartGame();
        }

        public void NextTurn(long whoseTurnPlayerId, int slotN, long defendingPlayerId, int defSlotN, int turnN)
        {
            StaticRoomData.WhoseAttack = whoseTurnPlayerId;
            StaticRoomData.WhoseDefend = defendingPlayerId;
            StaticRoomData.CardsLeftInTalon -= (StaticRoomData.MaxPlayers * StaticRoomData.MaxCardsDraw);

            foreach (var player in StaticRoomData.Players)
            {
                player.Pass = false;
            }
            //Observable
            OnNextTurn(whoseTurnPlayerId, slotN, defendingPlayerId, defSlotN, turnN);
        }

        public void TalonData(int talonSize, string trumpCardCode)
        {
            StaticRoomData.CardsLeftInTalon = talonSize;

            StaticRoomData.TrumpCardCode = trumpCardCode;

            //Observable
            OnTalonData(talonSize, trumpCardCode);
        }

        public void DropCardOnTableOk(string cardCode)
        {
            //Observable
            OnDropCardOnTableOk(cardCode);
        }
        public void DropCardOnTableErrorNotYourTurn(string cardCode)
        {
            //Observable
            OnDropCardOnTableErrorNotYourTurn(cardCode);
        }
        public void DropCardOnTableErrorTableIsFull(string cardCode)
        {
            //Observable
            OnDropCardOnTableErrorTableIsFull(cardCode);
        }
        public void DropCardOnTableErrorCantDropThisCard(string cardCode)
        {
            //Observable
            OnDropCardOnTableErrorCantDropThisCard(cardCode);
        }
        public void OtherPlayerDropsCardOnTable(long playerId, int slotN, string cardCode)
        {
            //Observable
            OnOtherPlayerDropsCardOnTable(playerId, slotN, cardCode);
        }

        public void EndGame(long foolConnectionId, Dictionary<long, double> rewards)
        {
            //Observable
            OnEndGame(foolConnectionId, rewards);
        }

        public void EndGameGiveUp(long foolConnectionId, Dictionary<long, double> rewards)
        {
            //Observable
            OnEndGame(foolConnectionId, rewards);
            //Observable
            OnEndGameGiveUp(foolConnectionId, rewards);
        }

        public void EndGameFool(long foolPlayerId, Dictionary<long, double> rewards)
        {
            //Observable
            OnEndGame(foolPlayerId, rewards);
            //Observable
            OnEndGameFool(foolPlayerId);
        }

        public void OtherPlayerPassed(long passedPlayerId, int slotN)
        {
            StaticRoomData.Players[slotN].Pass = true;
            //Observable
            OnOtherPlayerPassed(passedPlayerId, slotN);
        }

        public void OtherPlayerCoversCard(long coveredPlayerId, int slotN,
            string cardOnTableCode, string cardDroppedCode)
        {
            /*foreach (var player in StaticRoomData.Players)
            {
                player.Pass = false;
            }*/

            //Observable
            OnOtherPlayerCoversCard(coveredPlayerId, slotN, cardOnTableCode, cardDroppedCode);
        }

        public void Beaten()
        {
            //Observable
            OnBeaten();
        }

        public void DefenderPicksCards(long pickedPlayerId, int slotN)
        {
            //Observable
            OnDefenderPicksCards(pickedPlayerId, slotN);
        }

        /// <summary>
        /// Sent by GameManager when i click endturnbutton
        /// </summary>
        public void MePassed()
        {
            //Observable
            OnMePassed();
        }

        /// <summary>
        /// Sent by InputManager
        /// </summary>
        public void DraggedCardUpdate(Vector2 mousePos, CardRoot draggedCardRoot, bool inTableZone)
        {
            //Observable
            OnDraggedCardUpdate(mousePos, draggedCardRoot, inTableZone);
        }

        /// <summary>
        /// Sent by InputManager
        /// </summary>
        public void CardDroppedOnTableByMe(CardRoot cardRoot)
        {
            //Observable
            OnCardDroppedOnTableByMe(cardRoot);
        }

        /// <summary>
        /// Sent by GameManager anything on table happens
        /// </summary>
        public void TableUpdated()
        {
            //Observable
            OnTableUpdated();
        }

        public void PlayerWon(long wonPlayerId, double winnerReward)
        {
            //Observable
            OnPlayerWon(wonPlayerId, winnerReward);
        }

        public void RoomList(RoomInstance[] rooms)
        {
            //Observable
            OnRoomList(rooms);
        }

    }
}
