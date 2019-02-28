


using System.Collections.Generic;
using System.Linq;
using Assets.Fool_online.Scripts.Manager;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using Fool_online.Scripts.InRoom.CardsScripts;
using UnityEditor;
using UnityEngine;

namespace Fool_online.Scripts.Manager
{
    /// <summary>
    /// Derived in LoomLogic
    /// Creates implementation of actions in room which called by RoomLogic
    /// Triggers animations as abstract methods
    /// </summary>
    public abstract class RoomActions : RoomStateFields // todo remove inheritance from RoomStateFields 
    {

        #region Enums and constants

        protected enum PassButtonText
        {
            Pass,
            PickUpCards,
            Beaten
        }

        #endregion

        /// <summary>
        /// Init members on constructor
        /// </summary>
        protected RoomActions()
        {
            //Room = new RoomStateFields();
        }

        #region Fields

        /// <summary>
        /// Doom data controlled by this class
        /// </summary>
        /// todo migrate state fields to aggregated class (just uncomment this)
        //protected RoomStateFields Room;

        #endregion

        //Methods to be implemented in GameManager

        #region Abstract methods animation triggers

        ///////////////////////////////////////////////
        // ANIMATIONS TRIGGERED BY RoomActions
        ///////////////////////////////////////////////

        protected abstract void StopAnimationGlowingCardsOnTable();

        protected abstract void AnimateAddCardOnTableFromMyHand(CardRoot cardRoot);

        protected abstract void AnimateCoverCardBy(CardRoot cardOnTable, CardRoot heldCard);

        protected abstract void AnimateHidePassButton();

        protected abstract void AnimateShowPassButton(PassButtonText passButton);

        protected abstract void AnimateHideTextClouds();

        protected abstract void AnimateRemoveAllCardsToDiscardPile();

        ///////////////////////////////////////////////
        // ANIMATIONS TRIGGERED BY RoomLogic
        ///////////////////////////////////////////////

        protected abstract void AnimateHideMyPassAndTextCloud();

        protected abstract void AnimatePlayerPicksCardsFromTable(int playerSlotN, List<CardRoot> cards);

        protected abstract CardRoot AnimateEnemyCoversCard(int slotN, CardRoot cardOnTable, string cardCodeHeld);

        protected abstract CardRoot AnimateEnemyAddCardToTable(int slotN, string cardCodeHeld);

        #endregion

        //Methods that have an implementation and used by RoomLogic

        #region Protected methods

        /// <summary>
        /// Do table contains card of same value with held card?
        /// </summary>
        protected bool CanAddThisCard(CardRoot heldCard)
        {
            // You always can add to an empty table
            if (TableIsEmpty) return true;

            // check If trying to drop wrong card

            // get card value from the held card
            int heldCardValue = heldCard.GetCardValue();
            // find at least one card on table with same value
            if (!GetAllCardsOnTable().Any(card => card.GetCardValue() == heldCardValue))
            {
                // say 'Cant add this card to table'
                MessageManager.Instance.ShowFullScreenText("Эту карту нельзя подкинуть");
                return false;
            }


            // if table is not full (5 cards on first turn, 6 on any other)
            // and player defending has cards
            if (TableIsFull || !DefenderHasCardsToDefend)
            {
                // say 'Too much cards on table'
                MessageManager.Instance.ShowFullScreenText("Перебор");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds card to table from my hand and animates it
        /// </summary>
        protected void MeAddCardToTable(CardRoot heldCard)
        {
            //disable interactions
            heldCard.InteractionDisable();
            //ADD CARD 
            cardsOnTable.Add(heldCard);

            //init animation
            AnimateAddCardOnTableFromMyHand(heldCard);

            //TODO save action in buffer for in case if server will say no so we can ctrl+z this action
            //Send to server
            ClientSendPackets.Send_DropCardOnTable(heldCard.CardCode);

            // if my hand is empty then count as pass
            if (MyPlayer.CardsNumber == 0 && MeLeadAttack)
            {
                AttackerPassedPriority = true;
            }

            TableUpdated();
        }

        /// <summary>
        /// If me dropped card on table while defending then check if action is possible
        /// </summary>
        protected bool CanCoverThisCardWith(CardRoot cardOnTable, CardRoot heldCard)
        {
            //if table is empty
            if (TableIsEmpty || AllCardsCovered)
            {
                // say 'you are defending'
                MessageManager.Instance.ShowFullScreenText("На вас ходят");
                return false;
            }

            //if i passed
            if (DefenderPassedPriority)
            {
                // say 'you passed'
                MessageManager.Instance.ShowFullScreenText("Вы решили брать");
                return false;
            }

            bool cardOnTableIsTrump = cardOnTable.IsTrump();
            bool droppedCardIsTrump = heldCard.IsTrump();

            if (!cardOnTableIsTrump && droppedCardIsTrump) return true;
            
            if (cardOnTableIsTrump && droppedCardIsTrump 
                && cardOnTable.Value < heldCard.Value) return true;

            if (!cardOnTableIsTrump && !droppedCardIsTrump
                && cardOnTable.Suit == heldCard.Suit
                && cardOnTable.Value < heldCard.Value) return true;

            // else 
            {
                // say 'you can not defend with this card'
                MessageManager.Instance.ShowFullScreenText("Вы не можете побиться этой картой");
                return false;
            }

        }

        /// <summary>
        /// Animates covering card on table with held card
        /// </summary>
        protected void CoverThisCardWith(CardRoot cardOnTable, CardRoot heldCard)
        {
            //TODO save action in buffer for in case if server will say no so we can ctrl+z this action
            // Beat it
            AnimateCoverCardBy(cardOnTable, heldCard);

            heldCard.InteractionDisable();

            // add to list
            cardsOnTableCovering.Add(heldCard);
            //Set cards parameters
            cardOnTable.IsCoveredByACard = true;
            cardOnTable.CoveredByCard = heldCard;


            // Send to server
            ClientSendPackets.Send_CoverCardOnTable(cardOnTable.CardCode, heldCard.CardCode);

            TableUpdated();
        }

        /// <summary>
        /// Returns physically closest card to mousePos
        /// </summary>
        protected CardRoot GetClosestCardOnTableTo(Vector2 mousePos)
        {
            CardRoot closest = null;
            float minDistance = float.MaxValue;

            foreach (var cardOnTable in cardsOnTable)
            {
                if (cardOnTable.IsCoveredByACard) continue;

                float dist = Vector2.Distance(cardOnTable.transform.position, mousePos);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = cardOnTable;
                }
            }

            return closest;
        }

        /// <summary>
        /// Find cards which can be targeted (has lower value and same suit)
        /// </summary>
        protected List<CardRoot> GetCardsCanBeTargetedBy(CardRoot heldCard)
        {
            //find cards that wasn't covered already
            var cardsCanBeTargeted = cardsOnTable.Where(card => !card.IsCoveredByACard);

            //held card is a trump?
            if (heldCard.IsTrump())
            {
                //find cards that ...
                return cardsCanBeTargeted.Where(
                        card => !card.IsTrump() //.. are not trump
                                        || (card.IsTrump() && card.Value < heldCard.Value))//.. or cheaper trump
                    .ToList(); // return as list
            }
            else //held card is not a trump
            {
                //find cards that ...
                return cardsCanBeTargeted.Where(
                        card => card.Value < heldCard.Value //.. cheaper than held card
                                && card.Suit == heldCard.Suit) //.. and has same suit
                    .ToList(); // return as list
            }
        }

        /// <summary>
        /// Both cards on table and cards covering
        /// </summary>
        protected List<CardRoot> GetAllCardsOnTable()
        {
            var allCards = new List<CardRoot>();
            allCards.AddRange(cardsOnTable);
            allCards.AddRange(cardsOnTableCovering);
            return allCards;
        }


        /// <summary>
        /// Called when someone drops card on table
        /// Manages display of 'skip turn' buttons
        /// </summary>
        protected void TableUpdated()
        {
            CheckPassButtons();

            //Hide 'i pass' texts
            AnimateHideTextClouds();

            ResetActivePlayersPassStatus();
        }


        /// <summary>
        /// Called on game end to clear all list and trigger animations
        /// </summary>
        protected void EndGame()
        {
            TurnN = 0;

            AnimateRemoveAllCardsToDiscardPile();

            //Set state accordingly to how much players are in here
            PlayerNumberChanged();

            //Hide 'i pass' texts
            AnimateHideTextClouds();
            AnimateHidePassButton();

        }

        /// <summary>
        /// Clear this turn status variables and wait for next turn
        /// </summary>
        protected void WaitForNextTurn()
        {

            ResetAllPlayersPassStatus();

            // Wait for server to send us NextTurn
            State = RoomState.Paused;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Checks if everybody's in room and enables 'ready' button
        /// </summary>
        protected void PlayerNumberChanged()
        {
            RoomIsFull = (StaticRoomData.ConnectedPlayersCount == StaticRoomData.MaxPlayers);//todo

            if (RoomIsFull)
            {
                State = RoomState.PlayersGettingReady;
            }
            else
            {
                if (State == RoomState.Playing)
                {
                    State = RoomState.WaitingForPlayersToConnect;
                }
                else
                {
                    State = RoomState.PlayersGettingReady;
                }
            }
        }

        /// <summary>
        /// As said - sets every player to no-pass
        /// defender is ignored there: if he did passed then he will be left as passed after call
        /// Also players with no cards left will be counted as passed stil
        /// Won players are always passed 
        /// </summary>
        private void ResetActivePlayersPassStatus()
        {
            foreach (var player in Players)
            {
                // ignore defender
                if (player == Defender) continue;

                // player with empty hand or already won = passes
                if (player.CardsNumber == 0 || player.Won)
                {
                    player.Pass = true;
                    continue;
                }

                player.Pass = false;
            }
        }

        /// <summary>
        /// Sets every player to no-pass
        /// </summary>
        private void ResetAllPlayersPassStatus()
        {
            // reset state variables
            DefenderPassedPriority = false;
            AttackerPassedPriority = false;

            foreach (var player in Players)
            {
                // player with empty hand or already won = passes
                if (player.Won)
                {
                    player.Pass = true;
                    continue;
                }

                player.Pass = false;
            }
        }

        /// <summary>
        /// Shows or hides your pass button according to current room state
        /// </summary>
        private void CheckPassButtons()
        {
            if (MeDefending)
            {
                // hide pass button if i've succesfully defended
                if (AllCardsCovered)
                {
                    AnimateHidePassButton();
                }
                // if i didn't pass defence show button to pass defence and take all cards
                else if (!DefenderPassedPriority)
                {
                    AnimateShowPassButton(PassButtonText.PickUpCards);
                }
            }
            //im not defending
            else
            {
                //attacker passed or i am attacker
                if (IcanAddCards)
                {
                    if (AllCardsCovered)
                    {
                        AnimateShowPassButton(PassButtonText.Beaten);
                    }
                    else if (DefenderPassedPriority)
                    {
                        AnimateShowPassButton(PassButtonText.Pass);
                    }
                    else
                    {
                        AnimateHidePassButton();
                    }
                }

                // else wait for attacker to pass priority
            }
        }

        /// <summary>
        /// Find player by connection id and return his nickname
        /// </summary>
        protected string GetPlayerNickname(long connectionId)
        {
            return Players.Single(player => player.ConnectionId == connectionId).Nickname;
        }

        #endregion
    }
}

