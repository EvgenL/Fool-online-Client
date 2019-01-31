using System.Collections;
using System.Collections.Generic;
using Fool_online.Ui.Mainmenu;
using UnityEngine;
using UnityEngine.UI;

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
    /// <param name="room"></param>
    public void DrawRoom(RoomInstance room)
    {

    }

    public void OnClick()
    {

    }
}
