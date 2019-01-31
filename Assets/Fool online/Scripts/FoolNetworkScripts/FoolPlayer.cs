namespace Fool_online.Scripts.FoolNetworkScripts
{
    /// <summary>
    /// network player data
    /// </summary>
    public class FoolPlayer
    {
        /// <summary>
        /// Display Name Of Player
        /// </summary>
        public string Nickname => "Игрок " + ConnectionId;

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
    }
}
