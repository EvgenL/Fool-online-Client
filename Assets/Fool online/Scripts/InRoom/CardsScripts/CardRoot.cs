using DG.Tweening;
using Fool_online.Scripts.InRoom;
using UnityEditor;
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
        public Transform CoveredCardContainer;

        [Header("Card animated local scales in different locations")]
        [SerializeField] private float _scaleInEnemyHand = 0.33f;
        [SerializeField] private float _scaleInMyHand = 1f;
        [SerializeField] private float _scaleOnTable = 1f;


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

        public void AnimateMoveToTransform(Transform container)
        {
            Vector3 startPos = interactibleCard.transform.position;
            Quaternion startRot = interactibleCard.transform.rotation;
            Vector3 startScale = interactibleCard.transform.localScale;

            transform.SetParent(container, false);
            transform.position = container.position;
            transform.rotation = container.rotation;

            //if card was put on table or in hand, we need to force rebuild layout on this frame
            LayoutGroup layout = container.GetComponent<LayoutGroup>();
            if (layout != null)
            {
                //LayoutRebuilder.MarkLayoutForRebuild(handTransform as RectTransform); //this doesnt do this on exact frame
                LayoutRebuilder.ForceRebuildLayoutImmediate(container as RectTransform);
            }

            interactibleCard.AnimateMoveFromToRoot(startPos, startRot, startScale);
        }

        public void AnimateMoveToMyHand(Transform handTransform)
        {
            Vector3 startPos = interactibleCard.transform.position;
            Quaternion startRot = interactibleCard.transform.rotation;
            Vector3 startScale = transform.localScale; 

            transform.SetParent(handTransform);
            transform.localScale = Vector3.one * _scaleInMyHand;
            transform.localRotation = Quaternion.identity;

            //if card was put on table or in hand, we need to force rebuild layout on this frame
            LayoutGroup layout = handTransform.GetComponent<LayoutGroup>();
            if (layout != null)
            {
                //LayoutRebuilder.MarkLayoutForRebuild(handTransform as RectTransform); //this doesnt do this on exact frame
                LayoutRebuilder.ForceRebuildLayoutImmediate(handTransform as RectTransform);
            }

            //init animation on card
            interactibleCard.AnimateMoveFromToRoot(startPos, startRot, startScale);
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
