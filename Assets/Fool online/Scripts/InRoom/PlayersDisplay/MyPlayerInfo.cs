using System.Collections.Generic;
using Fool_online.Scripts.CardsScripts;
using Fool_online.Scripts.Network;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.InRoom
{
    public class MyPlayerInfo : PlayerInfo
    {

        [SerializeField] private MyHandRenderer _myHand;
        [SerializeField] private GameObject GetReadyButton;
        [SerializeField] private GameObject EndTurnButton;
        [SerializeField] private Text EndTurnButtonText;

        public new List<CardRoot> CardsInHand => _myHand.CardsInHand;

        private void Start()
        {
            //TODO my nickname
            NicknameText.text = FoolNetwork.LocalPlayer.Nickname + " (вы)";

            _myHand.ClearHand();
        }

        public void OnGetReadyButtonClick(bool value)
        {
            GameManager.Instance.OnGetReady(value);
        }

        public void OnEndTurnButtonClick()
        {
            GameManager.Instance.OnEndTurnButtonClick();
        }

        public void ShowGetReadyButton()
        {
            GetReadyButton.SetActive(true);
            print("My player info: get ready");
        }

        public void HideGetReadyButton()
        {
            GetReadyButton.SetActive(false);
            GetReadyButton.GetComponentInChildren<Toggle>().isOn = false;
        }

        public void RemoveCardFromHand(CardRoot cardRoot)
        {
            CardsInHand.Remove(cardRoot);
        }

        public void ShowPickUpCardsButton()
        {
            EndTurnButton.SetActive(true);
            EndTurnButtonText.text = "Беру";
        }
        public void ShowPassbutton()
        {
            EndTurnButton.SetActive(true);
            EndTurnButtonText.text = "Пас";
        }
        public void ShowBeatenbutton()
        {
            EndTurnButton.SetActive(true);
            EndTurnButtonText.text = "Бито";
        }
    

        /// <summary>
        /// Hides EndTurnButton and/or GetReadyButton from my player info display
        /// </summary>
        public void HideAllButtons()
        {
            EndTurnButton.SetActive(false);
            HideGetReadyButton();
        }


        public void PickUpCard(CardRoot cardRoot)
        {
            CardsInHand.Add(cardRoot);

            //Enable interactions
            cardRoot.SetOnTable(false);

            cardRoot.AnimateMoveToTransform(HandContainer);
        }

        public override void PickCardsFromTable(List<CardRoot> cardsOnTable, List<CardRoot> cardsOnTableCovering)
        {
            foreach (var card in cardsOnTable)
            {
                PickUpCard(card);
            }
            foreach (var card in cardsOnTableCovering)
            {
                PickUpCard(card);
            }
        }

    }
}
