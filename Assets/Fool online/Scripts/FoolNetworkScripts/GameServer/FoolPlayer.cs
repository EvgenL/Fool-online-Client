namespace Fool_online.Scripts.FoolNetworkScripts
{
    /// <summary>
    /// network player data
    /// </summary>
    public class FoolPlayer
    {
        /// <summary>
        /// id Of Player
        /// </summary>
        public long UserId;

        /// <summary>
        /// Display Name Of Player
        /// </summary>
        public string Nickname;

        /// <summary>
        /// ConnectionId sent by server on connect
        /// Used to differ enemy player datas from mine in room
        /// </summary>
        public long ConnectionId;

        /// <summary>
        /// Did you joined room or no
        /// </summary>
        public bool IsInRoom = false;

        /// <summary>
        /// Room in which player is
        /// </summary>
        public long RoomId;

        /// <summary>
        /// Slot/chair number in room
        /// </summary>
        public int InRoomSlotNumber;

        /// <summary>
        /// Set to true if client did send token
        /// </summary>
        public bool Authorized;

        /// <summary>
        /// Amount of currency
        /// </summary>
        public double Money;
    }
}
