using Fool_online.Scripts.Manager;
using UnityEngine;

namespace Fool_online.Scripts.InRoom.CardsScripts
{
    /// <summary>
    /// Class that helps transform card codes like 0.14 into card names like AS (ace of spades)
    /// </summary>
    public static class CardUtil
    {
        /// <summary>
        /// transform card codes like 0.14 into card names like AS (ace of spades)
        /// </summary>
        public static string GetNameFromCode(string cardCode)
        {
            int suit = Suit(cardCode);
            int value = Value(cardCode);

            string cardName = "";
            switch (suit)
            {
                case 0:
                    cardName += "S"; //Spades
                    break;
                case 1:
                    cardName += "H"; //hearts
                    break;
                case 2:
                    cardName += "D"; //diamonds
                    break;
                case 3:
                    cardName += "C"; //clubs
                    break;
            }
            switch (value)
            {
                case 11:
                    cardName += "J"; //Jack
                    break;
                case 12:
                    cardName += "Q"; //Queen
                    break;
                case 13:
                    cardName += "K"; //King
                    break;
                case 14:
                    cardName += "A"; //Ace
                    break;
                default: //6, 7, 8 etc
                    cardName += value;
                    break;
            }

            return cardName;
        }

        /// <summary>
        /// Returns number of a card value
        /// where for example
        /// 6 is 6
        /// 9 is 9
        /// 11 is J (Jocker = Валет)
        /// 14 is A (Ace = Туз
        /// </summary>
        public static int Value(string cardCode)
        {
            return int.Parse(cardCode.Split('.')[1]);
        }

        /// <summary>
        /// Returns number of a card suit
        /// where
        /// 0 is Spades
        /// 1 is hearts
        /// 2 is diamonds
        /// 3 is clubs
        /// </summary>
        public static int Suit(string cardCode)
        {
            return int.Parse(cardCode.Split('.')[0]);
        }

        /// <summary>
        /// Gets sprite file for card code
        /// Gets back sprite for input "BACK"
        /// </summary>
        public static Sprite GetSprite(string cardCode)
        {
            if (cardCode == "BACK")
            {
                return GetBackSprite();
            }

            string cardName = GetNameFromCode(cardCode);
            return Resources.Load<Sprite>("Cards/" + StaticRoomData.CardFrontSkin  + "/" + cardName);
        }

        /// <summary>
        /// Gets back sprite 
        /// </summary>
        public static Sprite GetBackSprite()
        {
            return Resources.Load<Sprite>("Cards/" + StaticRoomData.CardBackSkin + "/" + "BACK");
        }

    }
}
