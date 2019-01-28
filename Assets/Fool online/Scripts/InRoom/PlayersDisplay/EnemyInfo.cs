using System.Collections;
using System.Collections.Generic;
using Assets.Fool_online.Scripts.InRoom;
using Fool_online.Scripts.CardsScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.InRoom
{
    public class EnemyInfo : PlayerInfo
    {

        [SerializeField] protected Image ReadyCheckmark;
        [SerializeField] private GameObject CardBackPrefab;
        [SerializeField] private Transform _cardPickPoint;
        private Transform _talonTransform;

        private int _displayCardsN;

        private void Awake()
        {
            _talonTransform = FindObjectOfType<TalonRenderer>().TalonDisplay.transform;
        }

        public void Reset()
        {
            NicknameText.text = "";
            ReadyCheckmark.enabled = false;
        }

        public override void SetReadyCheckmark(bool value)
        {
            ReadyCheckmark.enabled = value;
        }

        public void SetCardsNumberInHand(int cardsN)
        {

            this.cardsN = cardsN;
            //TODO красивое отображение карт с анимациями

            Util.DestroyAllChildren(HandContainer);

            for (int i = 0; i < cardsN; i++)
            {
                //Spawn card
                var cardGo = Instantiate(CardBackPrefab, HandContainer);
                var cardRootScript = cardGo.GetComponent<CardRoot>();
                cardRootScript.InitGraphics("BACK");
                //Add to list
                CardsInHand.Add(cardRootScript);
            }
        }

        public void TakeCardsFromTalon(int cardsN)
        {
            StartCoroutine(AnimateCardsFromTalon(cardsN));
        }

        /// <summary>
        /// Animate taking cards from talon one by one with slight delay between every card
        /// </summary>
        private IEnumerator AnimateCardsFromTalon(int cardsN)
        {
            for (int i = 0; i < cardsN; i++)
            {
                //Spawn card as child of this trahsform.
                CardRoot cardRootScript = SpawnCardInHand();
                cardRootScript.transform.SetParent(_talonTransform, false);

                //Init animation
                cardRootScript.AnimateMoveToTransform(HandContainer);

                yield return new WaitForSeconds(0.1f); //StaticParameters.TalonAnimationDelay);
            }

            //UpdateCardsInHand();
        }

        private CardRoot SpawnCardInHand()
        {
            var cardGo = Instantiate(CardBackPrefab, HandContainer);
            var cardRootScript = cardGo.GetComponent<CardRoot>();
            cardRootScript.InitGraphics("BACK");
            AddCardToHand(cardRootScript);
            cardsN++;
            return cardRootScript;
        }

        private void AddCardToHand(CardRoot cardRootScript)
        {
            CardsInHand.Add(cardRootScript);
            cardRootScript.interactibleCard.CanBeDragged = false;
            cardsN++;
        }

        /// <summary>
        /// Spawns and drops card on table
        /// </summary>
        /// <returns>Dropped card</returns>
        public CardRoot DropCardOnTable(Transform table, string cardCode)
        {
            //Spawn card as child of table trahsform.
            var cardRootScript = SpawnCard(cardCode);

            //Init animation
            cardRootScript.AnimateMoveToTransform(table);

            return cardRootScript;
        }

        public CardRoot SpawnCard(string cardCode)
        {
            //TODO scarle
            //Spawn card as child of hand trahsform.
            var cardGo = Instantiate(CardBackPrefab, HandContainer);

            if (CardsInHand.Count > 0) //bug null
            {
                cardGo.transform.position = CardsInHand[0].transform.position;
                cardGo.transform.rotation = CardsInHand[0].transform.rotation;
                cardGo.transform.localScale = CardsInHand[0].transform.localScale;

                
            }
            else
            {
                cardGo.transform.position = transform.position;
                cardGo.transform.rotation = transform.rotation;
                cardGo.transform.localScale = transform.localScale;
            }

            var cardRootScript = cardGo.GetComponent<CardRoot>();
            cardRootScript.InitGraphics(cardCode);

            //Make it non-interactible
            cardRootScript.SetOnTable(true);

            RemoveOneCardFromHand();

            return cardRootScript;
        }

        private void RemoveOneCardFromHand()
        {
            cardsN--;
            if (CardsInHand.Count > 0)
            {
                var card = CardsInHand[0];
                CardsInHand.Remove(card);
                Destroy(card.gameObject);
            }
        }

        public override void PickCardsFromTable(List<CardRoot> cardsOnTable, List<CardRoot> cardsOnTableCovering)
        {
            foreach (var card in cardsOnTable)
            {
                card.AnimateMoveToTransform(_cardPickPoint);
                SpawnCardInHand();
                card.DestroyCard(2f);
            }
            foreach (var card in cardsOnTableCovering)
            {
                card.AnimateMoveToTransform(_cardPickPoint);
                SpawnCardInHand();
                card.DestroyCard(2f);
            }
        }


    }
}    

