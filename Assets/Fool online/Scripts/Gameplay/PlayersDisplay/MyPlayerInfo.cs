using System.Collections.Generic;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.InRoom.CardsScripts;
using Fool_online.Scripts.Manager;
using Fool_online.Scripts.UiScripts.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.InRoom.PlayersDisplay
{
    /// <summary>
    /// Class responsive for displaying my nickname, status and cards
    /// </summary>
    public class MyPlayerInfo : PlayerInfo
    {
        [SerializeField] private MyHandRenderer _myHand;
        [SerializeField] private GameObject EndTurnButton;
        [SerializeField] private Text EndTurnButtonText;

        public new List<CardRoot> CardsInHand => _myHand.CardsInHand;

        private void Start()
        {
            //TODO my nickname
            NicknameText.text = FoolNetwork.LocalPlayer.Nickname + " (вы)";

            _myHand.ClearHand();
        }

        public void OnEndTurnButtonClick()
        {
            InputManager.Instance.OnMePass();
        }

        public void RemoveCardFromHand(CardRoot cardRoot)
        {
            CardsInHand.Remove(cardRoot);
        }

        public void ShowPickUpCardsButton()
        {
            ShowEndTurnButtonText("Беру");
        }
        public void ShowPassbutton()
        {
            ShowEndTurnButtonText("Пас");
        }
        public void ShowBeatenbutton()
        {
            ShowEndTurnButtonText("Бито");
        }

        private void ShowEndTurnButtonText(string buttonText)
        {
            if (StaticRoomData.Iwon) return;
            EndTurnButton.SetActive(true);
            EndTurnButtonText.text = buttonText;
        }


        /// <summary>
        /// Hides EndTurnButton and/or GetReadyButton from my player info display
        /// </summary>
        public void HidePassButton()
        {
            EndTurnButton.SetActive(false);
        }


        public void PickUpCard(CardRoot cardRoot)
        {
            CardsInHand.Add(cardRoot);

            //Enable interactions
            cardRoot.InteractionEnable();

            cardRoot.AnimateMoveToTransform(HandContainer);
        }

        public override void PickCardsFromTable(List<CardRoot> cards)
        {
            foreach (var card in cards)
            {
                PickUpCard(card);
            }
        }

        public override void AnimateRemoveCardsToDiscardPile(DiscardPile discard, float delay)
        {
            foreach (var cardInHand in _myHand.CardsInHand)
            {
                discard.AnimateRemoveCardToDiscardPile(cardInHand, delay);
            }

            _myHand.CardsInHand.Clear();
        }

    }
}
