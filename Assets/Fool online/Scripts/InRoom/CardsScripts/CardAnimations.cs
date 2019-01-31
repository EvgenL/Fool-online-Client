using System.Collections.Generic;
using UnityEngine;

namespace Fool_online.Scripts.InRoom.CardsScripts
{
    public static class CardAnimations
    {
        public static void PutMyCardOnTable(CardRoot cardRoot, RectTransform table)
        {
        }

        //For animationg hover effect for cards on table
        public static void AnimateCardsOnTableDefending(CardRoot draggedCardRoot, List<CardRoot> cardsOnTable, Transform handContainer)
        {
            CardRoot closestCardRoot = GetClosestCardOnTable(draggedCardRoot, cardsOnTable, handContainer);

            foreach (var c in cardsOnTable)
            {
                if (c == closestCardRoot)
                {
                   // c.cardOnTable.PlayAnimationTargeted();
                }
                else
                {
                   // c.cardOnTable.PlayAnimationNoTargeted();
                   // c.cardOnTable.PlayAnimationCanBeAttacked();
                }

            }
        }

        //For animationg hover effect for cards on table. Returns null if hand is closer (on screen) than any card on table
        public static CardRoot GetClosestCardOnTable(CardRoot draggedCardRoot, List<CardRoot> cardsOnTable, Transform handContainer)
        {
            Vector3 draggedCardPos = draggedCardRoot.interactibleCard.transform.position;

            Vector3 handX0pos = handContainer.position;
            handX0pos.x = draggedCardPos.x;
            float distanceToHand = Vector3.Distance(draggedCardPos, handX0pos);


            CardRoot closestCardRoot = null;
            float minDistance = float.MaxValue;

            foreach (var cardOnTable in cardsOnTable)
            {
                float dist = Vector3.Distance(cardOnTable.transform.position, draggedCardPos);

                if (dist < distanceToHand && dist < minDistance)
                {
                    minDistance = dist;
                    closestCardRoot = cardOnTable;
                }
            }
            if (closestCardRoot == null) Debug.Log("Closest card = null");
            return closestCardRoot;
        }

        public static void StopAnimationsOnTable(List<CardRoot> cardsOnTable)
        {
            foreach (var c in cardsOnTable)
            {
               //c.cardOnTable.PlayAnimationIdle();
            }
        }
    }
}
