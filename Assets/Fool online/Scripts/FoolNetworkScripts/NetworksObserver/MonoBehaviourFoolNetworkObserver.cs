using System.Collections.Generic;
using Fool_online.Scripts.InRoom.CardsScripts;
using Fool_online.Ui.Mainmenu;
using UnityEngine;

namespace Fool_online.Scripts.FoolNetworkScripts.NetworksObserver
{
    /// <summary>
    /// Observer of fool online network events
    /// </summary>
    public abstract class MonoBehaviourFoolNetworkObserver : MonoBehaviour
    {
        protected MonoBehaviourFoolNetworkObserver()
        {
            FoolNetworkObservable.Attach(this);
        }

        /// <summary>
        /// Called when client tries reach the server
        /// </summary>
        public virtual void OnTryingConnectToGameServer()
        {
        }

        /// <summary>
        /// Called when client succesfully connected to server and authorized
        /// </summary>
        public virtual void OnAuthorizedOk(long connectionId)
        {
        }

        /// <summary>
        /// Called when client sent wrong token
        /// </summary>
        public virtual void OnErrorBadAuthToken()
        {
        }

        public virtual void OnUpdateUserData(long connectionId, string userId, string nickname)
        {
        }

        /// <summary>
        /// Called when client succesfully connected to server
        /// </summary>
        public virtual void OnConnectedToGameServer()
        {
        }

        /// <summary>
        /// Called when client was disconnected from server
        /// </summary>
        public virtual void OnDisconnectedFromGameServer()
        {
        }

        /// <summary>
        /// Called when client joined room
        /// </summary>
        public virtual void OnJoinRoom()
        {
        }

        /// <summary>
        /// Called when client was disconnected from room
        /// </summary>
        public virtual void OnLeaveRoom()
        {
        }

        /// <summary>
        /// Called when client was disconnected from room
        /// </summary>
        public virtual void OnFailedToJoinFullRoom()
        {
        }

        /// <summary>
        /// Called when client tried connect same room twice
        /// </summary>
        public virtual void OnYouAreAlreadyInRoom()
        {
        }

        /// <summary>
        /// Called when room data changed
        /// </summary>
        public virtual void OnRoomData()
        {
        }

        /// <summary>
        /// Called on somebody expect you joins room. Also OnRoomData called.
        /// </summary>
        public virtual void OnOtherPlayerJoinedRoom(long joinedPlayerId, int slotN, string joinedPlayerNickname)
        {
        }

        /// <summary>
        /// Called on somebody expect you leaves room. Also OnRoomData called.
        /// </summary>
        public virtual void OnOtherPlayerLeftRoom(long leftPlayerId, int slotN)
        {
        }

        /// <summary>
        /// Called on somebody expect you click 'ready'. Also OnRoomData called.
        /// </summary>
        public virtual void OnOtherPlayerGotReady(long leftPlayerId, int slotN)
        {
        }

        /// <summary>
        /// Called on somebody expect you click 'ready'. Also OnRoomData called.
        /// </summary>
        public virtual void OnOtherPlayerGotNotReady(long leftPlayerId, int slotN)
        {
        }

        /// <summary>
        /// Called on OnYouGotCards.
        /// </summary>
        public virtual void OnYouGotCards(string[] cards)
        {
        }

        /// <summary>
        /// Called on somebody takes cards from talon.
        /// </summary>
        public virtual void OnEnemyGotCardsFromTalon(long playerId, int slotN, int cardsN)
        {
        }

        /// <summary>
        /// Called on room game starts.
        /// </summary>
        public virtual void OnStartGame()
        {
        }

        /// <summary>
        /// Called on every turn.
        /// </summary>
        public virtual void OnNextTurn(long whoseTurnPlayerId, int slotN, long defendingPlayerId, int defSlotN, int turnN)
        {
        }

        /// <summary>
        /// Called on game start when talon got updated
        /// </summary>
        public virtual void OnTalonData(int talonSize, string trumpCardCode)
        {
        }
        public virtual void OnDropCardOnTableOk(string cardCode)
        {
        }
        public virtual void OnDropCardOnTableErrorNotYourTurn(string cardCode)
        {
        }
        public virtual void OnDropCardOnTableErrorTableIsFull(string cardCode)
        {
        }
        public virtual void OnDropCardOnTableErrorCantDropThisCard(string cardCode)
        {
        }
        /// <summary>
        /// Called on other player drops card on table
        /// </summary>
        public virtual void OnOtherPlayerDropsCardOnTable(long playerId, int slotN, string cardCode)
        {
        }

        public virtual void OnEndGame(long foolConnectionId, Dictionary<long, double> rewards)
        {
        }

        public virtual void OnEndGameGiveUp(long foolConnectionId, Dictionary<long, double> rewards)
        {
        }

        public virtual void OnOtherPlayerPassed(long passedPlayerId, int slotN)
        {
        }

        public virtual void OnOtherPlayerCoversCard(long coveredPlayerId, int slotN,
            string cardOnTableCode, string cardDroppedCode)
        {
        }
        public virtual void OnBeaten()
        {
        }

        public virtual void OnDefenderPicksCards(long pickedPlayerId, int slotN)
        {
        }

        public virtual void OnEndGameFool(long foolPlayerId)
        {
        }

        /// <summary>
        /// Sent by GameManager when i click endturnbutton
        /// </summary>
        public virtual void OnMePassed()
        {
        }

        /// <summary>
        /// Sent by InputManager
        /// </summary>
        public virtual void OnDraggedCardUpdate(Vector2 mousePos, CardRoot draggedCardRoot, bool inTableZone)
        {
        }

        /// <summary>
        /// Sent by InputManager
        /// </summary>
        public virtual void OnCardDroppedOnTableByMe(CardRoot cardRoot)
        {
        }

        /// <summary>
        /// Sent by GameManager anything on table happens
        /// </summary>
        public virtual void OnTableUpdated()
        {
        }


        public virtual void OnPlayerWon(long wonPlayerId, double winnerReward)
        {
        }
        public virtual void OnRoomList(RoomInstance[] rooms)
        {
        }


        private void OnDestroy()
        {
            FoolNetworkObservable.Detach(this);
        }
    }
}
