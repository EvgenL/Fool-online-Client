using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using Fool_online.Scripts.InRoom.CardsScripts;
using Fool_online.Scripts.Manager;
using UnityEngine;

namespace Assets.Fool_online.Scripts.Manager.RoomManagerClasses
{
    /// <summary>
    /// Derived in GameManager
    /// Class that manages room logic 
    /// Processes observer callbacks
    /// Calls actions from RoomActions
    /// </summary>
    public abstract class RoomLogic : RoomActions
    {
        private void Awake()
        {
            //Show 'ready' button if me joined last
            PlayerNumberChanged();
        }


        #region Observer callbacks

        ///////////////////////////////////////////////
        // WEB CALLBACKS
        ///////////////////////////////////////////////
        
        /// <summary>
        /// Observer callback
        /// When all cards are beaten and all players are passed except defender
        /// </summary>
        public override void OnBeaten()
        {
            WaitForNextTurn();
        }

        /// <summary>
        /// Observer callback
        /// When not all cards are beaten and all players are passed including defender
        /// </summary>
        public override void OnDefenderPicksCards(long pickedPlayerId, int slotN)
        {
            var cards = GetAllCardsOnTable();
            AnimatePlayerPicksCardsFromTable(slotN, cards);

            WaitForNextTurn();
        }

        /// <summary>
        /// Callback on somebody enters room
        /// </summary>
        public override void OnOtherPlayerJoinedRoom(long joinedPlayerId, int slotN, string joinedPlayerNickname)
        {
            PlayerNumberChanged();
        }

        /// <summary>
        /// Callback on somebody leaves room
        /// if game is not
        /// </summary>
        public override void OnOtherPlayerLeftRoom(long leftPlayerId, int slotN)
        {
            if (State == RoomState.Playing)
            {
                //TODO pause game and wait for him to recconect
            }
            else
            {
                PlayerNumberChanged();
            }
        }

        /// <summary>
        /// Observer callback when enemy adds card to table
        /// </summary>
        public override void OnOtherPlayerDropsCardOnTable(long playerId, int slotN, string cardCode)
        {
            // spawn and animate card
            var cardRoot = AnimateEnemyAddCardToTable(slotN, cardCode);

            // Add to list
            cardsOnTable.Add(cardRoot);

            TableUpdated();
        }

        /// <summary>
        /// Other player covered some card on table
        /// </summary>
        public override void OnOtherPlayerCoversCard(long coveredPlayerId, int slotN,
            string cardOnTableCode, string cardDroppedCode)
        {
            //find card on table
            CardRoot cardOnTable = cardsOnTable.Find(c =>
                !c.IsCoveredByACard && c.CardCode == cardOnTableCode);

            if (cardOnTable == null)
            {
                Debug.LogWarning("Trying to cover non existing card: " + cardOnTableCode + " by " + cardDroppedCode, this);
                return;
            }

            var droppedCardRoot = AnimateEnemyCoversCard(slotN, cardOnTable, cardDroppedCode);

            //set state
            cardOnTable.IsCoveredByACard = true;
            cardsOnTableCovering.Add(droppedCardRoot);

            TableUpdated();
        }

        /// <summary>
        /// Observer callback on somebody clicked 'pass'
        /// </summary>
        public override void OnOtherPlayerPassed(long passedPlayerId, int slotN)
        {
            //if defender just passed then i can add card or just pass
            if (Defender.ConnectionId == passedPlayerId)
            {
                DefenderPassedPriority = true;

                if (IcanAddCards)
                {
                    AnimateShowPassButton(PassButtonText.Pass);
                }
            }
            //if attacker just passed then i can add card or just pass. If attack was defeated (all cards covered) then i can say 'beaten'
            else if (Attacker.ConnectionId == passedPlayerId)
            {
                AttackerPassedPriority = true;

                if (!MeDefending)
                {
                    if (AllCardsCovered)
                    {
                        AnimateShowPassButton(PassButtonText.Beaten);
                    }
                    else
                    {
                        AnimateShowPassButton(PassButtonText.Pass);
                    }
                }
            }

            // if defender decided to take cards 
            // or defence was succecful
            // anyway - turn ended
            // then wait for next turn
            if (AllPassed || (AllButDefenderPassed && AllCardsCovered))
            {
                WaitForNextTurn();
            }
        }
        
        /// <summary>
        /// Somebody clicked 'ready' button
        /// if all players are ready then wait for game to start
        /// </summary>
        public override void OnOtherPlayerGotReady(long leftPlayerId, int slotN)
        {
            if (EverybodyReady) WaitForGameStartSignal();
        }

        /// <summary>
        /// Called every next turn
        /// </summary>
        public override void OnNextTurn(long whoseTurnPlayerId, int slotN, long defendingPlayerId, int defSlotN, int turnN)
        {
            //hide infos
            AnimateHidePassButton();
            AnimateHideTextClouds();

            //cards on table
            cardsOnTable.Clear();
            cardsOnTableCovering.Clear();

            //attacker defender
            Attacker = Players[slotN];
            Defender = Players[defSlotN];

            this.TurnN = turnN;
            State = RoomState.Playing;
        }

        /// <summary>
        /// Error on i try add card to table
        /// Wrong card
        /// </summary>
        public override void OnDropCardOnTableErrorCantDropThisCard(string cardCode)
        {
            MessageManager.Instance.ShowFullScreenText("Нельзя подкинуть эту карту");
            //TODO go back (cardCode)
        }

        /// <summary>
        /// Error on i try add card to table
        /// Not my turn
        /// </summary>
        public override void OnDropCardOnTableErrorNotYourTurn(string cardCode)
        {
            MessageManager.Instance.ShowFullScreenText("Не ваш ход");
            //TODO go back (cardCode)
        }

        /// <summary>
        /// Error on i try add card to table
        /// Table is full
        /// </summary>
        public override void OnDropCardOnTableErrorTableIsFull(string cardCode)
        {
            MessageManager.Instance.ShowFullScreenText("Перебор");
            //TODO go back (cardCode)
        }

        /// <summary>
        /// Server sends this when game has ended natural way and somebody lost
        /// </summary>
        public override void OnEndGameFool(long foolPlayerId)
        {
            string foolNickname = GetPlayerNickname(foolPlayerId);
            MessageManager.Instance.ShowFullScreenText(foolNickname + " - дурак");


            EndGame();
        }

        /// <summary>
        /// Server sends this on game was interrupted by somebody giving up
        /// </summary>
        public override void OnEndGameGiveUp(long foolConnectionId, Dictionary<long, double> rewards)
        {
            string foolNickname = GetPlayerNickname(foolConnectionId);
            MessageManager.Instance.ShowFullScreenText(foolNickname + " сдался.");

            EndGame();
        }





        ///////////////////////////////////////////////
        // OTHER CALLBACKS
        // (mainly called by InputManager)
        ///////////////////////////////////////////////

        /// <summary>
        /// Called by InputManager when i click 'ready'
        /// </summary>
        public override void OnMeGotReady(bool value)
        {
            MyPlayer.IsReady = value;

            if (value)
            {
                ClientSendPackets.Send_GetReady();
            }
            else
            {
                ClientSendPackets.Send_GetNotReady();
            }
        }

        /// <summary>
        /// Called by InputManager whenever i drop card on table
        /// </summary>
        public override void OnCardDroppedOnTableByMe(CardRoot heldCard)
        {
            StopAnimationGlowingCardsOnTable();

            if (State != RoomState.Playing) return;

            if (AllPassed ||
                (AllButDefenderPassed && AllCardsCovered)) return;

            //if i am defending. 
            if (MeDefending)
            {
                var closestCardOnTable = GetClosestCardOnTableTo(Input.mousePosition);
                // check if action is correct
                if (CanCoverThisCardWith(closestCardOnTable, heldCard))
                {
                    CoverThisCardWith(closestCardOnTable, heldCard);
                }
            }
            else if (IcanAddCards && CanAddThisCard(heldCard)) //if i am attacking and can add this card
            {
                MeAddCardToTable(heldCard);
            }
            // else if i am not attacking nor defenfing and cant add cards
            // then wait till attacker passes priority
        }

        /// <summary>
        /// Called on each frame by InputManger when i drag card
        /// Animates cards that can be covered when you are defending
        /// </summary>
        public override void OnDraggedCardUpdate(Vector2 mousePos, CardRoot heldCardRoot, bool inTableZone)
        {
            //Am i defending or attacking?
            if (!MeDefending) return;

            //if dragged above a table
            if (inTableZone)
            {
                //Choose cards that can be covered with grabbed card
                var cardsCanBeTargeted = GetCardsCanBeTargetedBy(heldCardRoot);
                if (cardsCanBeTargeted.Count == 0) return;

                //Chose closest beatable card
                var closestCard = GetClosestCardOnTableTo(mousePos);

                //Animate them
                foreach (var cardCanBeTargeted in cardsCanBeTargeted)
                {
                    if (cardCanBeTargeted == closestCard)
                    {
                        cardCanBeTargeted.AnimateTargetedGlow();
                    }
                    else
                    {
                        cardCanBeTargeted.AnimateCanBeTargetedGlow();
                    }
                }
            }
            else
            {
                //if not in table: idle animation
                StopAnimationGlowingCardsOnTable();
            }

        }

        /// <summary>
        /// On i clicked get ready button
        /// </summary>
        public void OnGetReadyClick(bool value)
        {
            if (State != RoomState.PlayersGettingReady) return;

            if (value)
            {
                ClientSendPackets.Send_GetReady();
            }
            else
            {
                ClientSendPackets.Send_GetNotReady();
            }

            StaticRoomData.MyPlayer.IsReady = value;

            if (EverybodyReady)
            {
                WaitForGameStartSignal();
            }
        }

        #endregion



        #region Protected methods

        /// <summary>
        /// Called on every player clicks 'ready'
        /// </summary>
        protected void WaitForGameStartSignal()
        {
            //Wait for server to start game
            State = RoomState.Paused;
        }

        #endregion



    }
}
