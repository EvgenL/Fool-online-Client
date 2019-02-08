using System.Collections.Generic;
using System.Linq;
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

        #region Singleton

        //it was thread-safe but removed because not needed
        //private static object padlock = new object();

        public static FoolNetworkObservableCallbacksWrapper Instance
        {
            get
            {
                //lock (padlock)
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

        public void AuthorizedOk(long connectionId)
        {
            FoolNetwork.LocalPlayer.Authorized = true;
            //Observable
            OnAuthorizedOk(connectionId);
        }

        public void ErrorBadAuthToken()
        {
            //Observable
            OnErrorBadAuthToken();
        }

        public void UpdateUserData(long connectionId, string userId, string nickname)
        {
            FoolNetwork.LocalPlayer.UserId = userId;
            FoolNetwork.LocalPlayer.Nickname = nickname;
            //Observable
            OnUpdateUserData(connectionId, userId, nickname);
        }

        
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
            FoolNetwork.DisconnectReasonText = reason;

            //Observable
            OnDisconnectedFromGameServer();
        }

        public void RoomDataUpdated()
        {
            //Observable
            OnRoomData();
        }

        public void OtherPlayerJoinedRoom(long joinedPlayerId, int slotN, string joinedPlayerNickname)
        {
            StaticRoomData.ConnectedPlayersCount++;

            // if this slot was occupied once then replace slot values
            if (StaticRoomData.OccupiedSlots.TryGetValue(slotN, out long n))
            {
                StaticRoomData.OccupiedSlots[slotN] = joinedPlayerId;
                StaticRoomData.PlayerIds.Remove(n);
                StaticRoomData.PlayerIds.Add(joinedPlayerId);
            }
            else // else create new slot values
            {
                StaticRoomData.OccupiedSlots.Add(slotN, joinedPlayerId);
                StaticRoomData.PlayerIds.Add(joinedPlayerId);
            }

            PlayerInRoom newPlayer = new PlayerInRoom(joinedPlayerId);
            newPlayer.SlotN = slotN;
            newPlayer.Nickname = joinedPlayerNickname;
            StaticRoomData.Players[slotN] = newPlayer;

            //Observable
            OnRoomData();

            //Observable
            OnOtherPlayerJoinedRoom(joinedPlayerId, slotN, joinedPlayerNickname);
        }

        public void OtherPlayerLeftRoom(long leftPlayerId, int slotN)
        {
            StaticRoomData.ConnectedPlayersCount--;

            //Observable
            OnOtherPlayerLeftRoom(leftPlayerId, slotN);

            StaticRoomData.Players[slotN].Left = true;

            foreach (var player in StaticRoomData.Players)
            {
                if (player != null)
                {
                    player.IsReady = false;
                }
            }
            //Observable
            OnRoomData();
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
            StaticRoomData.Players[slotN].AddCardsN(cardsN);
            //Observable
            OnEnemyGotCardsFromTalon(playerId, slotN, cardsN);
        }
        
        public void StartGame()
        {
            StaticRoomData.IsPlaying = true;

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
            StaticRoomData.MyPlayer.AddCardsN(-1);
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
            StaticRoomData.Players[slotN].AddCardsN(-1);
            //Observable
            OnOtherPlayerDropsCardOnTable(playerId, slotN, cardCode);
        }

        public void EndGame(long foolConnectionId, Dictionary<long, double> rewards)
        {
            StaticRoomData.IsPlaying = false;

            //Observable
            OnEndGame(foolConnectionId, rewards);

            /*foreach (var player in StaticRoomData.Players)
            {
                if (player != null && player.Left)
                {

                    StaticRoomData.ConnectedPlayersCount--;
                    StaticRoomData.OccupiedSlots.Remove(player.SlotN);
                    StaticRoomData.PlayerIds.Remove(player.ConnectionId);

                    StaticRoomData.Players[player.SlotN] = null;
                }
            }*/
        }

        public void EndGameGiveUp(long foolPlayerId, Dictionary<long, double> rewards)
        {
            EndGame(foolPlayerId, rewards);
            //Observable
            OnEndGameGiveUp(foolPlayerId, rewards);
        }

        public void EndGameFool(long foolPlayerId, Dictionary<long, double> rewards)
        {
            EndGame(foolPlayerId, rewards);
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
            StaticRoomData.Players[slotN].AddCardsN(-1);
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
            StaticRoomData.Players[slotN].AddCardsN(GameManager.Instance.CardsOnTableNumber);
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
            StaticRoomData.MyPlayer.AddCardsN(-1);
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
            StaticRoomData.Players.Single(player => player.ConnectionId == wonPlayerId).Won = true;
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
