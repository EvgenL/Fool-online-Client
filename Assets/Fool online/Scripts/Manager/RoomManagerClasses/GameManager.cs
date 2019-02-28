using System.Collections.Generic;
using System.Linq;
using Assets.Fool_online.Scripts.Manager.RoomManagerClasses;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using Fool_online.Scripts.InRoom;
using Fool_online.Scripts.InRoom.CardsScripts;
using Fool_online.Scripts.InRoom.PlayersDisplay;
using UnityEngine;

namespace Fool_online.Scripts.Manager
{
    /// <summary>
    /// Class responsive for top-level room logic and animations.
    /// </summary>
    public class GameManager : RoomLogic
    {

        /// <summary>
        /// singleton instance
        /// </summary>
        public static GameManager Instance;

        /// <summary>
        /// Calss handling top part of the screen with enemy names and cards
        /// </summary>
        [SerializeField] private PlayersInfosManager _playersInfosManager;

        /// <summary>
        /// Class responsive for drawing cards on table
        /// </summary>
        [SerializeField] private TableRenderer _tableDisplay;

        /// <summary>
        /// Class responsive for displaying my nickname, status and cards
        /// </summary>
        [SerializeField] private MyPlayerInfo _myPlayerInfoDisplay;

        /// <summary>
        /// Class displayng talon (прикуп)
        /// </summary>
        [SerializeField] private TalonRenderer _talonDisplay;

        /// <summary>
        /// Class displayng Discard Pile (отбой)
        /// </summary>
        [SerializeField] private DiscardPile _discardDisplay;

        /// <summary>
        /// Set instance on awake
        /// </summary>
        private void Start()
        {
            Instance = this;
        }


        public override void OnMePassed()
        {
            _myPlayerInfoDisplay.HidePassButton();

            if (State != RoomState.Playing) return;

            if (AllCardsCovered)
            {
                //pass
                ClientSendPackets.Send_Pass();
                StaticRoomData.MyPlayer.Pass = true;
            }
            //if i am defending 
            else if (MeDefending)
            {
                //pick up cards
                DefenderPassedPriority = true;

                ClientSendPackets.Send_Pass();
                StaticRoomData.MyPlayer.Pass = true;
            }
            //if my turn and i am attacking
            else if (IcanAddCards)
            {
                //pass
                ClientSendPackets.Send_Pass();
                StaticRoomData.MyPlayer.Pass = true;
            }

        }



        #region Animation methods


        /// <summary>
        /// Shows 'pass' button with different texts on my side of screen
        /// </summary>
        protected override void AnimateShowPassButton(PassButtonText passButton)
        {
            switch (passButton)
            {
                case PassButtonText.Beaten:
                    _myPlayerInfoDisplay.ShowBeatenbutton();
                    break;
                case PassButtonText.Pass:
                    _myPlayerInfoDisplay.ShowPassbutton();
                    break;
                case PassButtonText.PickUpCards:
                    _myPlayerInfoDisplay.ShowPickUpCardsButton();
                    break;

            }
        }

        protected override void AnimateHideTextClouds()
        {
            if (DefenderPassedPriority)
            {
                _playersInfosManager.HideTextCloudsNoDefender();
            }
            else
            {
                _playersInfosManager.HideTextClouds();
            }
        }

        protected override void AnimateHideMyPassAndTextCloud()
        {
            _myPlayerInfoDisplay.HideTextCloud();
            _myPlayerInfoDisplay.HidePassButton();
        }

        protected override void AnimatePlayerPicksCardsFromTable(int playerSlotN, List<CardRoot> cards)
        {
            _playersInfosManager.PickCardsFromTable(playerSlotN, cards);
        }

        protected override CardRoot AnimateEnemyCoversCard(int slotN, CardRoot cardOnTable, string cardCodeHeld)
        {
            var heldCard = _playersInfosManager.AnimateEnemySpawnCard(slotN, cardCodeHeld);


            AnimateCoverCardBy(cardOnTable, heldCard);

            return heldCard;
        }

        /// <summary>
        /// Animate enemy spawns a card and drops on table
        /// </summary>
        /// <param name="slotN">Enemy who did that</param>
        /// <param name="cardCodeHeld">Card code to spawn</param>
        /// <returns></returns>
        protected override CardRoot AnimateEnemyAddCardToTable(int slotN, string cardCodeHeld)
        {
            var card = _playersInfosManager.AnimateEnemySpawnCard(slotN, cardCodeHeld);

            _tableDisplay.AnimateDropCardOnTable(card);

            return card;
        }

        /// <summary>
        /// Hides 'pass'
        /// </summary>
        protected override void AnimateHidePassButton()
        {
            _myPlayerInfoDisplay.HidePassButton();
        }

        /// <summary>
        /// Covers card on table with dropped card. Adds to covered cards list
        /// </summary>
        protected override void AnimateCoverCardBy(CardRoot cardOnTable, CardRoot heldCard)
        {
            //Disable interactions
            cardOnTable.InteractionDisable();
            //Animate
            heldCard.AnimateMoveToTransform(cardOnTable.CoveredCardContainer);
            cardOnTable.CoveredCardContainer.SetAsLastSibling();
        }



        /// <summary>
        /// Removes every card from table and players hands and moves to discard pile
        /// </summary>
        protected override void AnimateRemoveAllCardsToDiscardPile()
        {
            float delay = 2f;
            _talonDisplay.HideTalon(delay);


            //my cards
            _myPlayerInfoDisplay.AnimateRemoveCardsToDiscardPile(_discardDisplay, delay);

            //enemy cards
            _playersInfosManager.AnimateRemoveCardsToDiscardPile(_discardDisplay, delay);

        }

        /// <summary>
        /// Stops glowing on cards which you could beat on table 
        /// </summary>
        protected override void StopAnimationGlowingCardsOnTable()
        {
            _tableDisplay.StopAnimationGlowingCardsOnTable();
        }

        protected override void AnimateAddCardOnTableFromMyHand(CardRoot cardRoot)
        {
            _myPlayerInfoDisplay.RemoveCardFromHand(cardRoot);
            _tableDisplay.AnimateDropCardOnTable(cardRoot);
        }

        #endregion

    }
}
