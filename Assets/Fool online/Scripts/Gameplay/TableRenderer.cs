using System.Collections;
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
        [Header("Animation delay before removing cards on turn ended")] [SerializeField]
        private float RemoveCardsToDiscardDelay = 2f;

        /// <summary>
        /// Отбой
        /// </summary>
        [Header("Отбой")] [SerializeField] private DiscardPile Discard;

        /// <summary>
        /// Clear table if there was any cards
        /// </summary>
        private void Start()
        {
            Util.DestroyAllChildren(transform);
        }

        #region Observer methods

        /// <summary>
        /// Observer callback
        /// </summary>
        public override void OnBeaten()
        {
            AnimateRemoveCardsFromTableToDiscardPile(RemoveCardsToDiscardDelay);
        }

        /// <summary>
        /// Observer callback
        /// </summary>
        public override void OnEndGame(long foolConnectionId, Dictionary<long, double> rewards)
        {
            AnimateRemoveCardsFromTableToDiscardPile(RemoveCardsToDiscardDelay);
        }

        #endregion

        /// <summary>
        /// Animates drop card on table
        /// </summary>
        public void AnimateDropCardOnTable(CardRoot cardRoot)
        {
            cardRoot.AnimateMoveToTransform(this.transform);

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
        }

        /// <summary>
        /// Animates cover existing card on table
        /// </summary>
        public void AnimateCoverCardOnTable(CardRoot cardOnTable, CardRoot cardHeld)
        {
            cardHeld.AnimateMoveToTransform(cardOnTable.transform);

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
        }

        /// <summary>
        /// Animates removing cards from table also destroys game objects of cards
        /// </summary>
        public void AnimateRemoveCardsFromTableToDiscardPile(float delay = 0f)
        {
            var cards = GetComponentsInChildren<CardRoot>();
            foreach (var cardOnTable in cards)
            {
                Discard.AnimateRemoveCardToDiscardPile(cardOnTable, delay);
            }
        }

        /// <summary>
        /// Stops glowing animation of cards which was activated
        /// at card grab
        /// </summary>
        public void StopAnimationGlowingCardsOnTable()
        {
            var cards = GetComponentsInChildren<CardRoot>();
            foreach (var cardOnTable in cards)
            {
                cardOnTable.AnimateIdleGlow();
            }
        }


    }
}
