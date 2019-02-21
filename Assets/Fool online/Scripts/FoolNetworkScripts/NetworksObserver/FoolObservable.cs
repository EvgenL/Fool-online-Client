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
    ///     Implemented by fool networking classes to call server events on observer objects
    ///     Uses subscriber pattern with observers subscribing on their constructor and unsubscribing
    ///     via unity callback OnDestroy()
    /// </summary>
    public class FoolObservable
    {
        //Handling of observers list

        #region Observers list

        /// Subscribed observers
        private static HashSet<MonoBehaviourFoolObserver> _observers =
            new HashSet<MonoBehaviourFoolObserver>();

        /// <summary>
        /// Called from observer constructor. Attaches it to _observers list.
        /// </summary>
        public static void Attach(MonoBehaviourFoolObserver observer)
        {
            _observers.Add(observer);
        }


        /// <summary>
        /// Called from observer constructor. Detaches it from _observers list.
        /// </summary>
        public static void Detach(MonoBehaviourFoolObserver observer)
        {
            if (_observers.Contains(observer))
            {
                _observers.Remove(observer);
            }
        }

        #endregion


        //Methods that being sent to every observer

        #region Events

        public static void OnTryingConnectToGameServer()
        {
            foreach (var obs in _observers)
            {
                obs.OnTryingConnectToGameServer();
            }
        }

        public static void OnConnectedToGameServer()
        {
            foreach (var obs in _observers)
            {
                obs.OnConnectedToGameServer();
            }
        }

        public static void OnAuthorizedOk(long connectionId)
        {
            FoolNetwork.LocalPlayer.Authorized = true;

            foreach (var obs in _observers)
            {
                obs.OnAuthorizedOk(connectionId);
            }
        }

        public static void OnErrorBadAuthToken()
        {
            foreach (var obs in _observers)
            {
                obs.OnErrorBadAuthToken();
            }
        }

        public static void OnUpdateUserData(long connectionId, string userId, string nickname)
        {
            FoolNetwork.LocalPlayer.UserId = userId;
            FoolNetwork.LocalPlayer.Nickname = nickname;


            foreach (var obs in _observers)
            {
                obs.OnUpdateUserData(connectionId, userId, nickname);
            }
        }

        public static void OnDisconnectedFromGameServer(string reason = null)
        {
            //Set player status to inRoom
            FoolNetwork.DisconnectReasonText = reason;

            foreach (var obs in _observers)
            {
                obs.OnDisconnectedFromGameServer();
            }
        }

        public static void OnJoinRoom(long roomId)
        {
            //Set player status to inRoom
            FoolNetwork.LocalPlayer.IsInRoom = true;
            FoolNetwork.LocalPlayer.RoomId = roomId;

            foreach (var obs in _observers)
            {
                obs.OnJoinRoom();
            }
        }

        public static void OnLeaveRoom()
        {
            //Set player status to not-in-Room
            FoolNetwork.LocalPlayer.IsInRoom = false;
            FoolNetwork.LocalPlayer.RoomId = 0;

            foreach (var obs in _observers)
            {
                obs.OnLeaveRoom();
            }
        }

        public static void OnFailedToJoinFullRoom()
        {
            //Set player status to not-in-Room
            FoolNetwork.LocalPlayer.IsInRoom = false;
            FoolNetwork.LocalPlayer.RoomId = 0;

            foreach (var obs in _observers)
            {
                obs.OnFailedToJoinFullRoom();
            }
        }

        public static void OnYouAreAlreadyInRoom()
        {
            foreach (var obs in _observers)
            {
                obs.OnYouAreAlreadyInRoom();
            }
        }

        public static void OnRoomDataUpdated()
        {
            foreach (var obs in _observers)
            {
                obs.OnRoomData();
            }
        }

        public static void OnOtherPlayerJoinedRoom(long joinedPlayerId, int slotN, string joinedPlayerNickname)
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


            //Observable - room data changed
            OnRoomDataUpdated();

            foreach (var obs in _observers)
            {
                obs.OnOtherPlayerJoinedRoom(joinedPlayerId, slotN, joinedPlayerNickname);
            }
        }

        public static void OnOtherPlayerLeftRoom(long leftPlayerId, int slotN)
        {
            StaticRoomData.ConnectedPlayersCount--;

            StaticRoomData.Players[slotN].Left = true;

            //Set all players to non-ready
            foreach (var player in StaticRoomData.Players)
            {
                if (player != null)
                {
                    player.IsReady = false;
                }
            }

            foreach (var obs in _observers)
            {
                obs.OnOtherPlayerLeftRoom(leftPlayerId, slotN);
            }
        }

        public static void OnOtherPlayerGotReady(long playerId, int slotN)
        {
            StaticRoomData.Players[slotN].IsReady = true;

            foreach (var obs in _observers)
            {
                obs.OnOtherPlayerGotReady(playerId, slotN);
            }
        }

        public static void OnOtherPlayerGotNotReady(long playerId, int slotN)
        {
            StaticRoomData.Players[slotN].IsReady = false;

            foreach (var obs in _observers)
            {
                obs.OnOtherPlayerGotNotReady(playerId, slotN);
            }
        }

        public static void OnYouGotCardsFromTalon(string[] cards)
        {
            StaticRoomData.MyPlayer.TakeCards(cards);

            foreach (var obs in _observers)
            {
                obs.OnYouGotCards(cards);
            }
        }

        public static void OnEnemyGotCardsFromTalon(long playerId, int slotN, int cardsN)
        {
            StaticRoomData.Players[slotN].AddCardsN(cardsN);

            foreach (var obs in _observers)
            {
                obs.OnEnemyGotCardsFromTalon(playerId, slotN, cardsN);
            }
        }

        public static void OnStartGame()
        {
            StaticRoomData.IsPlaying = true;

            foreach (var player in StaticRoomData.Players)
            {
                player.Won = false;
                player.Pass = false;
                player.IsReady = false;
            }


            foreach (var obs in _observers)
            {
                obs.OnStartGame();
            }
        }

        public static void OnNextTurn(long whoseTurnPlayerId, int slotN, long defendingPlayerId, int defSlotN, int turnN)
        {
            StaticRoomData.WhoseAttack = whoseTurnPlayerId;
            StaticRoomData.WhoseDefend = defendingPlayerId;
            StaticRoomData.CardsLeftInTalon -= (StaticRoomData.MaxPlayers * StaticRoomData.MaxCardsDraw);

            foreach (var player in StaticRoomData.Players)
            {
                player.Pass = false;
            }

            foreach (var obs in _observers)
            {
                obs.OnNextTurn(whoseTurnPlayerId, slotN, defendingPlayerId, defSlotN, turnN);
            }
        }

        public static void OnTalonData(int talonSize, string trumpCardCode)
        {
            StaticRoomData.CardsLeftInTalon = talonSize;

            StaticRoomData.TrumpCardCode = trumpCardCode;

            foreach (var obs in _observers)
            {
                obs.OnTalonData(talonSize, trumpCardCode);
            }
        }

        public static void OnDropCardOnTableOk(string cardCode)
        {
            foreach (var obs in _observers)
            {
                obs.OnDropCardOnTableOk(cardCode);
            }
        }

        public static void OnDropCardOnTableErrorNotYourTurn(string cardCode)
        {
            foreach (var obs in _observers)
            {
                obs.OnDropCardOnTableErrorNotYourTurn(cardCode);
            }
        }

        public static void OnDropCardOnTableErrorTableIsFull(string cardCode)
        {
            foreach (var obs in _observers)
            {
                obs.OnDropCardOnTableErrorTableIsFull(cardCode);
            }
        }

        public static void OnDropCardOnTableErrorCantDropThisCard(string cardCode)
        {
            foreach (var obs in _observers)
            {
                obs.OnDropCardOnTableErrorCantDropThisCard(cardCode);
            }
        }

        public static void OnOtherPlayerDropsCardOnTable(long playerId, int slotN, string cardCode)
        {
            StaticRoomData.Players[slotN].AddCardsN(-1);

            foreach (var obs in _observers)
            {
                obs.OnOtherPlayerDropsCardOnTable(playerId, slotN, cardCode);
            }
        }

        public static void OnEndGame(long foolConnectionId, Dictionary<long, double> rewards)
        {
            StaticRoomData.IsPlaying = false;


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

            foreach (var obs in _observers)
            {
                obs.OnEndGame(foolConnectionId, rewards);
            }
        }

        public static void OnEndGameGiveUp(long foolConnectionId, Dictionary<long, double> rewards)
        {
            OnEndGame(foolConnectionId, rewards);

            foreach (var obs in _observers)
            {
                obs.OnEndGameGiveUp(foolConnectionId, rewards);
            }
        }

        public static void OnOtherPlayerPassed(long passedPlayerId, int slotN)
        {
            StaticRoomData.Players[slotN].Pass = true;

            foreach (var obs in _observers)
            {
                obs.OnOtherPlayerPassed(passedPlayerId, slotN);
            }
        }

        public static void OnOtherPlayerCoversCard(long coveredPlayerId, int slotN,
            string cardOnTableCode, string cardDroppedCode)
        {
            StaticRoomData.Players[slotN].AddCardsN(-1);

            foreach (var obs in _observers)
            {
                obs.OnOtherPlayerCoversCard(coveredPlayerId, slotN, cardOnTableCode, cardDroppedCode);
            }
        }

        public static void OnBeaten()
        {
            foreach (var obs in _observers)
            {
                obs.OnBeaten();
            }
        }

        public static void OnDefenderPicksCards(long pickedPlayerId, int slotN)
        {
            StaticRoomData.Players[slotN].AddCardsN(RoomLogic.Instance.CardsOnTableNumber);

            foreach (var obs in _observers)
            {
                obs.OnDefenderPicksCards(pickedPlayerId, slotN);
            }
        }

        public static void OnEndGameFool(long foolPlayerId, Dictionary<long, double> rewards)
        {
            OnEndGame(foolPlayerId, rewards);

            foreach (var obs in _observers)
            {
                obs.OnEndGameFool(foolPlayerId);
            }
        }

        public static void OnMePassed()
        {
            foreach (var obs in _observers)
            {
                obs.OnMePassed();
            }
        }

        public static void OnDraggedCardUpdate(Vector2 mousePos, CardRoot draggedCardRoot, bool inTableZone)
        {
            foreach (var obs in _observers)
            {
                obs.OnDraggedCardUpdate(mousePos, draggedCardRoot, inTableZone);
            }
        }

        public static void OnCardDroppedOnTableByMe(CardRoot cardRoot)
        {
            StaticRoomData.MyPlayer.AddCardsN(-1);

            foreach (var obs in _observers)
            {
                obs.OnCardDroppedOnTableByMe(cardRoot);
            }
        }

        public static void OnTableUpdated()
        {
            foreach (var obs in _observers)
            {
                obs.OnTableUpdated();
            }
        }

        public static void OnPlayerWon(long wonPlayerId, double winnerReward)
        {
            //find player who won and set his status to PlayerInRoom.Won = true;
            StaticRoomData.Players.Single(player => player.ConnectionId == wonPlayerId).Won = true;

            foreach (var obs in _observers)
            {
                obs.OnPlayerWon(wonPlayerId, winnerReward);
            }
        }

        public static void OnRoomList(RoomInstance[] rooms)
        {
            foreach (var obs in _observers)
            {
                obs.OnRoomList(rooms);
            }
        }

        #endregion
    }
}