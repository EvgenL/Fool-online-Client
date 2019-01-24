using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fool_online.Scripts.InRoom;
using Fool_online.Scripts.Network;

namespace Assets.Fool_online.Scripts.FoolNetworkScripts
{
    public class RoomInfo
    {
        #region Game rules fields

        /// <summary>
        /// Initial number of cards in talon
        /// </summary>
        private int talonCardsNumber = 32;

        /// <summary>
        /// in fool game usually ace can not be a trump card
        /// </summary>
        private bool aceCanBeTrump = false;

        /// <summary>
        /// In case i will want to change it
        /// </summary>
        private int maxCardsOnTable = 6;

        /// <summary>
        /// In case i will want to change it
        /// </summary>
        private int maxCardsOnTableFirstTurn = 5;

        /// <summary>
        /// Money bet in this room
        /// </summary>
        private int bet;


        #endregion

        /// <summary>
        /// Player objects sorted by slot number in room
        /// </summary>
        public static int MaxPlayers;

        public static PlayerInRoom[] Players;

        public static PlayerInRoom MyPlayer => Players[FoolNetwork.LocalPlayer.InRoomSlotNumber];

        public static int MySlotNumber => FoolNetwork.LocalPlayer.InRoomSlotNumber;

        /// <summary>
        /// Player id who does turn now
        /// </summary>
        public static long WhoseTurn;
        public static long WhoseDefend;

        public static int MaxCardsInTalon = 32;
        public static int CardsLeftInTalon = 32;

        public static int MaxCardsDraw = 6;

        public static string TrumpCardCode;

        public static string CardFrontSkin = "Default";
        public static string CardBackSkin = "Default";

        public static bool DefenderPassed()
        {
            return Players.Any(player => player.Pass && player.ConnectionId == WhoseDefend);
        }

        public static PlayerInRoom Denfender => Players.Single(player => player.ConnectionId == WhoseDefend);
    }
}
