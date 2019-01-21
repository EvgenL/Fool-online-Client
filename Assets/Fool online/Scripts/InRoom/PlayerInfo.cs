using System.Collections.Generic;
using Fool_online.Scripts.CardsScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.InRoom
{
    public abstract class PlayerInfo : MonoBehaviour
    {

        [SerializeField] protected Text NicknameText;
        [SerializeField] protected Image Userpic;
        [SerializeField] protected Transform HandContainer;

        [SerializeField] private GameObject TextCloud;
        [SerializeField] private Text TextCloudText;

        protected int cardsN;

        public List<CardRoot> CardsInHand = new List<CardRoot>();


        public void ShowTextCloud(string message)
        {
            TextCloud.SetActive(true);
            TextCloudText.text = message;
        }
        public void HideTextCloud()
        {
            TextCloud.SetActive(false);
        }

        public abstract void PickCardsFromTable(List<CardRoot> cardsOnTable, List<CardRoot> cardsOnTableCovering);
    }
}
