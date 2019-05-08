using System.Linq;
using Fool_online.Scripts.FoolNetworkScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Ui.Mainmenu
{
    /// <summary>
    /// Single room display in Open Romms List
    /// </summary>
    public class RoomDisplay : MonoBehaviour
    {
        [SerializeField] private Text _playerNames;
        [SerializeField] private Text _maxPlayers;
        [SerializeField] private Text _deckSize;

        private RoomInstance _currentRoom;

        /// <summary>
        /// Draws RoomInstance at this display
        /// </summary>
        public void DrawRoom(RoomInstance room)
        {
            _currentRoom = room;

            _maxPlayers.text = $"{room.ConnectedPlayersN}/{room.MaxPlayers}";
            _deckSize.text = room.DeckSize.ToString();


            //csv from an array of strings
            string playerNames = room.PlayerNames.Aggregate((a, b) => a + ", " + b);
            _playerNames.text = playerNames;
        }

        public void OnClick()
        {
            FoolNetwork.JoinRoom(_currentRoom.RoomId);
        }
    }
}
