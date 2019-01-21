using DG.Tweening;
using Fool_online.Scripts.InRoom;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.CardsScripts
{
    public class CardRoot : MonoBehaviour
    {
        private bool IsOnTable = false; //In hand or on table
        public bool IsCoveredByACard = false;
        public CardRoot CoveredByCard;

        public Image CardVisual;
        public int Suit; //Масть
        public int Value; //Сила
        public string CardCode;

        public InteractibleCard interactibleCard;


        /// <summary>
        /// If this card is covered by some other card then 
        /// other card would be display at that position
        /// </summary>
        public Vector3 CoveredPosition;
        public Quaternion CoveredRotation;


        public void InitGraphics(string cardCode)
        {
            CardCode = cardCode;

            if (cardCode != "BACK")
            {
                Suit = CardUtil.Suit(cardCode);
                Value = CardUtil.Value(cardCode);
            }

            var sprite = CardUtil.GetSprite(cardCode);

            CardVisual.sprite = sprite;
        }

        public void AnimateFromToRoot(Vector3 startPosition)
        {
            interactibleCard.AnimateFromToRoot(startPosition);
        }
        public void AnimateFromToRoot(Vector3 startPosition, Quaternion startRotation)
        {
            interactibleCard.AnimateFromToRoot(startPosition, startRotation);
        }

        public void AnimateMoveToTransform(Transform container)
        {
            Vector3 startPos = interactibleCard.transform.position;
            Quaternion startRot = interactibleCard.transform.rotation;
            Vector3 startScale = interactibleCard.transform.localScale;

            transform.SetParent(container);

            transform.position = container.position;
            transform.rotation = container.rotation;
            transform.localScale = container.localScale;


            interactibleCard.AnimateFromToRoot(startPos, startRot, startScale);
        }

        public void DestroyCard(float delay)
        {
            Destroy(gameObject, delay);
        }

        /// <summary>
        /// Disables interations with card on table
        /// </summary>
        public void SetOnTable(bool value)
        {
            IsOnTable = value;
            interactibleCard.CanBeDragged = !value;

            if (value == false)
            {
                IsCoveredByACard = false;
                CoveredByCard = null;
            }
        }
        
        /// <summary>
        /// Returns if this card is trump in this game
        /// </summary>
        public bool IsTrump()
        {
            int trumpSuit = CardUtil.Suit(StaticRoomData.TrumpCardCode);
            return Suit == trumpSuit;
        }
    }
}
