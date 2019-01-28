using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fool_online.Scripts;
using Fool_online.Scripts.CardsScripts;
using UnityEngine;

namespace Assets.Fool_online.Scripts.InRoom
{
    //TODO
    internal class CardAnimationsManager : MonoBehaviour
    {
        public static CardAnimationsManager Instance;
        [SerializeField] private GameObject _cardPrefab;
        private Transform _talonTransform;

        private const float TalonAnimationDelay = 0.1f;

        private void Awake()
        {
            Instance = this;

            _talonTransform = FindObjectOfType<TalonRenderer>().TalonDisplay.transform;
        }

        public void EnemyGotCards(int cardsN, Transform enemyHand)
        {
            StartCoroutine(AnimateEnemyCardsFromTalon(cardsN, enemyHand));
        }

        public void YouGotCards(string[] cards, Transform handTransform)
        {
            StartCoroutine(AnimateCardsFromTalon(cards, handTransform));
        }

        //todo fix and migrate from MyHandRenderer
        private IEnumerator AnimateCardsFromTalon(string[] cards, Transform handTransform)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                //Spawn card as child of talon trahsform.
                var cardGo = Instantiate(_cardPrefab, _talonTransform);
                var cardRootScript = cardGo.GetComponent<CardRoot>();
                cardRootScript.InitGraphics(cards[i]);

                //Add to list
                //CardsInHand.Add(cardRootScript);

                //Init animation
                cardRootScript.AnimateMoveToMyHand(handTransform);

                yield return new WaitForSeconds(TalonAnimationDelay);
            }

            //UpdateCardsInHand();
        }


        /// <summary>
        /// Animate taking cards from talon one by one with slight delay between every card
        /// </summary>
        private IEnumerator AnimateEnemyCardsFromTalon(int cardsN, Transform enemyHand)
        {
            for (int i = 0; i < cardsN; i++)
            {
                //Spawn card as child of this trahsform.
                //CardRoot cardRootScript = SpawnCardInHand();

                //Spawn card as child of enemy hand trahsform.
                var cardGo = Instantiate(_cardPrefab, _talonTransform);
                var cardRootScript = cardGo.GetComponent<CardRoot>();
                cardRootScript.InitGraphics("BACK");

                cardRootScript.interactibleCard.CanBeDragged = false;

                //Init animation
                cardRootScript.AnimateMoveToTransform(enemyHand);

                yield return new WaitForSeconds(TalonAnimationDelay); //StaticParameters.TalonAnimationDelay);
            }

            //UpdateCardsInHand();
        }



    }
}
