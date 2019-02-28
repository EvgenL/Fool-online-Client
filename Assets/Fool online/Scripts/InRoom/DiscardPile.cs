using DG.Tweening;
using Fool_online.Scripts.InRoom.CardsScripts;
using UnityEngine;

namespace Fool_online.Scripts.InRoom
{
    /// <summary>
    /// Class displayng Discard Pile (отбой)
    /// </summary>
    public class DiscardPile : MonoBehaviour
    {
        //todo show discarded card backs
        /// <summary>
        /// Animates moving card to discardpile (отбой)
        /// </summary>
        public void AnimateRemoveCardToDiscardPile(CardRoot cardRoot, float delay = 0f)
        {
            if (cardRoot == null) return;
            //bug null reference if other player leaves
            float duration = 1f;
            cardRoot.AnimateMoveToPosition(transform.position, duration, delay);
            cardRoot.DestroyCard(delay + duration);
        }
    }
}
