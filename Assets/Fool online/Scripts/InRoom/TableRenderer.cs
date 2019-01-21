using DG.Tweening;
using Fool_online.Scripts.CardsScripts;
using Fool_online.Scripts.InRoom;
using Fool_online.Scripts.Network.NetworksObserver;
using UnityEngine;

namespace Fool_online.Scripts
{
    public class TableRenderer : MonoBehaviourFoolNetworkObserver
    {
        public DiscardPile Discard;

        public override void OnBeaten()
        {
            RemoveCardsFromTableToDiscardPile();
        }

        /// <summary>
        /// Animates removing cards from table also destroys game objects of cards
        /// </summary>
        public void RemoveCardsFromTableToDiscardPile()
        {
            var cardsOnTable = GameManager.Instance.cardsOnTable;
            var cardsOnTableCovering = GameManager.Instance.cardsOnTableCovering;

            foreach (var cardOnTable in cardsOnTable)
            {
                Discard.AnimateRemoveCardToDiscardPile(cardOnTable);
            }

            foreach (var cardOnTable in cardsOnTableCovering)
            {
                Discard.AnimateRemoveCardToDiscardPile(cardOnTable);
            }
        }
    }
}
