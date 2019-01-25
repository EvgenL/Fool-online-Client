using System.Collections.Generic;

namespace Fool_online.Scripts.InRoom
{
    public class PlayerInRoom
    {
        public PlayerInRoom(long connectionId)
        {
            this.ConnectionId = connectionId;
        }

        public long ConnectionId;

        public string Nickname => "Игрок " + ConnectionId;

        public int CardsNumber;

        public bool WaitForRecconect = false;

        public bool IsReady = false;

        public bool Pass = false;

        public int SlotN;

        #region local player only

        public List<string> Cards = new List<string>();

        /// <summary>
        /// Local player only
        /// Take visible cards
        /// </summary>
        public void TakeCards(string[] cards)
        {
            this.Cards.AddRange(cards);
            CardsNumber += cards.Length;
        }

        #endregion

        /// <summary>
        /// Non-local player only
        /// Take invisible cards
        /// </summary>
        public void TakeCards(int amount)
        {
            CardsNumber += amount;
        }
    }
}
