﻿using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using Fool_online.Scripts.InRoom.CardsScripts;
using Fool_online.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.InRoom
{
    /// <summary>
    /// Class responsive for drawing cards on table
    /// </summary>
    public class TableRenderer : MonoBehaviourFoolObserver
    {

        /// <summary>
        /// Animation delay before removing cards on turn ended
        /// </summary>
        [Header("Animation delay before removing cards on turn ended")]
        [SerializeField] private float RemoveCardsToDiscardDelay = 2f;

        /// <summary>
        /// Отбой
        /// </summary>
        [Header("Отбой")]
        [SerializeField] private DiscardPile Discard;

        /// <summary>
        /// Clear table if there was any cards
        /// </summary>
        private void Start()
        {
            Util.DestroyAllChildren(transform);
        }

        //Observer callback
        public override void OnBeaten()
        {
            var cardsOnTable = RoomLogic.Instance.cardsOnTable;
            var cardsOnTableCovering = RoomLogic.Instance.cardsOnTableCovering;
            var cards = new List<CardRoot>();
            cards.AddRange(cardsOnTable);
            cards.AddRange(cardsOnTableCovering);
            StartCoroutine(DelayRemoveCardsFromTableToDiscardPile(cards));
        }

        /// <summary>
        /// Animates drop card on table
        /// </summary>
        /// <param name="cardRoot"></param>
        public void AnimateDropCardOnTable(CardRoot cardRoot)
        {
            cardRoot.AnimateMoveToTransform(this.transform);

            transform.SetParent(this.transform, false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
        }

        //todo
        public void ClearTableAnimation()
        {

        }

        /// <summary>
        /// Delays method RemoveCardsFromTableToDiscardPile for RemoveCardsToDiscardDelay secounds.
        /// I use coroutine unstead of Invoke to pass parameter
        /// </summary>
        private IEnumerator DelayRemoveCardsFromTableToDiscardPile(List<CardRoot> cards)
        {
            yield return new WaitForSeconds(RemoveCardsToDiscardDelay);
            RemoveCardsFromTableToDiscardPile(cards);
        }

        /// <summary>
        /// Animates removing cards from table also destroys game objects of cards
        /// </summary>
        public void RemoveCardsFromTableToDiscardPile(List<CardRoot> cards)
        {

            foreach (var cardOnTable in cards)
            {
                Discard.AnimateRemoveCardToDiscardPile(cardOnTable);
            }
        }


    }
}
