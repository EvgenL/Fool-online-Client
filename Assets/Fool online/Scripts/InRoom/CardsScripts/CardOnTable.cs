using UnityEngine;

namespace Fool_online.Scripts.InRoom.CardsScripts
{
    public class CardOnTable : MonoBehaviour
    {
        public bool IsOnTable = false;
        public bool CanBeCovered = false;

        public bool IsAttackingPlayer = true; //Карта, брошеная на стол - атакует игрока: true; Карта, покрывающая другую карту: false;

        public CardOnTable CoveredByCard = null;
        public CardOnTable CoveringCard = null;

        public void PutOnTable()
        {
            IsOnTable = true;
            CanBeCovered = true;
        }

        public void CoverCard()
        {
            IsOnTable = true;
            CanBeCovered = false;
        }

        public void RemoveFromTable() //Полодить в руку или в отбой и выключить анимации
        {
            IsOnTable = false;
            CanBeCovered = false;

            PlayAnimationIdle();
        }

        public void PlayAnimationIdle()
        {
        }

        public void PlayAnimationCanBeAttacked()
        {
        }

        public void PlayAnimationTargeted()
        {
        }

        public void PlayAnimationNoTargeted()
        {
        }

    }
}
