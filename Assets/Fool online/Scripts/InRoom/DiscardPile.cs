using DG.Tweening;
using Fool_online.Scripts.CardsScripts;
using UnityEngine;

namespace Fool_online.Scripts
{
    public class DiscardPile : MonoBehaviour
    {

        /// <summary>
        /// Animates moving card to discardpile (отбой)
        /// </summary>
        /// <param name="cardRoot"></param>
        public void AnimateRemoveCardToDiscardPile(CardRoot cardRoot)
        {
            if (cardRoot == null) return;
            //bug null reference if other player leaves
            cardRoot.interactibleCard.transform.DOMove(transform.position, 1f);
            cardRoot.SetOnTable(true);
            cardRoot.interactibleCard.enabled = false;
            cardRoot.DestroyCard(2f);
        }
    }
}
