using System.Collections.Generic;
using System.Linq;
using Fool_online.Scripts.Network;

namespace Fool_online.Scripts.InRoom
{
    /// <summary>
    /// We get data from server about room data. GameManager takes it from here.
    /// Basically is extent of GameManager class.
    /// Also this class managed by server events such as OtherPlayerJoinedRoom and OtherPlayerLeftRoom in class FoolNetworkObservableCallbacks
    /// </summary>
    public static class StaticRoomData
    {
        /// <summary>
        /// Player objects sorted by slot number in room
        /// </summary>
        public static int ConnectedPlayersCount;

        public static int MaxPlayers;

        public static List<long> PlayerIds;

        public static PlayerInRoom[] Players;

        public static Dictionary<int, long> OccupiedSlots;

        public static PlayerInRoom MyPlayer => Players[FoolNetwork.LocalPlayer.InRoomSlotNumber];

        public static int MySlotNumber => FoolNetwork.LocalPlayer.InRoomSlotNumber;

        /// <summary>
        /// Player id who does turn now
        /// </summary>
        public static long WhoseAttack;
        public static long WhoseDefend;

        public static int MaxCardsInTalon = 32;
        public static int CardsLeftInTalon = 32;

        public static int MaxCardsDraw = 6;

        public static string TrumpCardCode;

        public static string CardFrontSkin = "Default";
        public static string CardBackSkin = "Default";

        public static PlayerInRoom Denfender => Players.Single(player => player.ConnectionId == WhoseDefend);
        public static PlayerInRoom Attacker => Players.Single(player => player.ConnectionId == WhoseAttack);
    }
}
