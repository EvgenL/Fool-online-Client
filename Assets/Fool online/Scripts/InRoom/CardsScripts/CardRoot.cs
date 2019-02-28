using DG.Tweening;
using Fool_online.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.InRoom.CardsScripts
{
    public class CardRoot : MonoBehaviour
    {
        //private bool IsOnTable = false; //In hand or on table
        public bool IsCoveredByACard = false;
        public CardRoot CoveredByCard;

        public Image CardVisual;
        public int Suit; //Масть
        public int Value; //Сила
        public string CardCode;

        [SerializeField] private InteractibleCard interactibleCard;


        /// <summary>
        /// Returns number of a card suit
        /// where
        /// 0 is Spades
        /// 1 is hearts
        /// 2 is diamonds
        /// 3 is clubs
        /// </summary>
        public int GetCardSuit()
        {
            return CardUtil.Suit(CardCode);
        }

        /// <summary>
        /// Returns number of a card value
        /// where for example
        /// 6 is 6
        /// 9 is 9
        /// 11 is J (Jocker = Валет)
        /// 14 is A (Ace = Туз
        /// </summary>
        public int GetCardValue()
        {
            return CardUtil.Value(CardCode);
        }

        /// <summary>
        /// If this card is covered by some other card then 
        /// other card would be display at that position
        /// </summary>
        public Transform CoveredCardContainer;

        //[Header("Card animated local scales in different locations")]
        //[SerializeField] private float _scaleInEnemyHand = 0.33f;
        //[SerializeField] private float _scaleInMyHand = 1f;
        //[SerializeField] private float _scaleOnTable = 1f;


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

        /// <summary>
        /// Aniamates card smoothly move from its current position to targeted transform
        /// Changes parent to tagreted transform
        /// </summary>
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
                //LayoutRebuilder.MarkLayoutForRebuild(handTransform as RectTransform); //this doesnt make this happen on exact frame
                LayoutRebuilder.ForceRebuildLayoutImmediate(container as RectTransform);
            }

            interactibleCard.AnimateMoveToRootFrom(startPos, startRot, startScale);
        }

        /// <summary>
        /// Tweens card to move to some position
        /// </summary>
        /// <param name="targetPos">Position to where move</param>
        /// <param name="duration">Duration of animation</param>
        /// <param name="delay">delay before start of animation</param>
        public void AnimateMoveToPosition(Vector3 targetPos, float duration, float delay)
        {

            interactibleCard.transform
                .DOMove(targetPos, duration)
                .SetEase(Ease.InOutCubic)
                .SetDelay(delay)
                .Play()
                ;

            InteractionDisable();

        }

        public void DestroyCard(float delay)
        {
            Destroy(gameObject, delay);
        }
        
        /// <summary>
        /// Returns if this card is trump in this game
        /// </summary>
        public bool IsTrump()
        {
            int trumpSuit = CardUtil.Suit(StaticRoomData.TrumpCardCode);
            return Suit == trumpSuit;
        }

        public void InteractionDisable()
        {
            interactibleCard.CanBeDragged = false;

            IsCoveredByACard = false;
            CoveredByCard = null;
        }

        public void InteractionEnable()
        {
            interactibleCard.CanBeDragged = true;

            IsCoveredByACard = false;
            CoveredByCard = null;
        }

        public void AnimateIdleGlow()
        {
            interactibleCard.AnimateIdle();
        }

        public void AnimateTargetedGlow()
        {
            interactibleCard.AnimateCanBeTargeted();
        }

        public void AnimateCanBeTargetedGlow()
        {
            interactibleCard.AnimateCanBeTargeted();
        }

        public Vector3 GetDisplayPosition()
        {
            return interactibleCard.transform.position;
        }
    }
}
