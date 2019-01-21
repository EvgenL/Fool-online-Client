using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts.CardsScripts;
using Fool_online.Scripts.Network.NetworksObserver;
using UnityEngine;

namespace Fool_online.Scripts.InRoom
{
    public class MyHandRenderer : MonoBehaviourFoolNetworkObserver
    {
        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private Transform _talonTransform;

        public List<CardRoot> CardsInHand = new List<CardRoot>();

        public override void OnYouGotCards(string[] cards)
        {
            StartCoroutine(AnimateCardsFromTalon(cards));
        }

        private IEnumerator AnimateCardsFromTalon(string[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                //Spawn card as child of this trahsform.
                var cardGo = Instantiate(_cardPrefab, transform);
                var cardRootScript = cardGo.GetComponent<CardRoot>();
                cardRootScript.InitGraphics(cards[i]);

                //Add to list
                CardsInHand.Add(cardRootScript);

                //Init animation
                AnimateFromTalonToHand(cardRootScript);

                yield return new WaitForSeconds(StaticParameters.TalonAnimationDelay);
            }

            //UpdateCardsInHand();
        }

        private void AnimateFromTalonToHand(CardRoot cardRoot)
        {
            cardRoot.AnimateFromToRoot(_talonTransform.position);


            //UpdateCardsInHand();
        }

        public void ClearHand()
        {
            Util.DestroyAllChildren(transform);
        }
        /*
        public void UpdateCardsInHand()
        {
            float spaceBetween = 200f;

            float centerIndex = _cardsInHand.Count / 2f;
            for (int i = 0; i < _cardsInHand.Count; i++)
            {
                Transform cardRoot = _cardsInHand[i].transform;

                Vector3 targetPosition = new Vector3(
                    transform.position.x + (i - centerIndex) * spaceBetween,
                    transform.position.y);

                cardRoot.DOMove(targetPosition, 0.2f);
            }
        }*/
    }
}
