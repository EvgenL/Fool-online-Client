/// <summary>
/// Instance of room on server.
/// Displayed in OpenRoomsList by RoomDisplay
/// </summary>

using System.Linq;

namespace Fool_online.Ui.Mainmenu
{
    public struct RoomInstance
    {
        public long RoomId;
        public int MaxPlayers { get; set; }
        public int DeckSize { get; set; }
        public int ConnectedPlayersN { get; set; }
        public string[] PlayerNames { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is RoomInstance))
                return false;

            RoomInstance other = (RoomInstance)obj;

            return RoomId == other.RoomId &&
                   MaxPlayers == other.MaxPlayers &&
                   DeckSize == other.DeckSize &&
                   ConnectedPlayersN == other.ConnectedPlayersN &&
                   PlayerNames.SequenceEqual(other.PlayerNames);

        }
    }
}
