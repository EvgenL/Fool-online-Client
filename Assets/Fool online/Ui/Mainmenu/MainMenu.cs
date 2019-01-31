using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class with logic of main menu and translations
/// between windows
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Window containers")]
    [SerializeField] private Transform _homeContainer;
    [SerializeField] private Transform _createRoomContainer;
    [SerializeField] private Transform _openRoomsListContainer;


    public void OnCreateRoomClick()
    {
        _homeContainer.gameObject.SetActive(false);
        _createRoomContainer.gameObject.SetActive(true);
        _openRoomsListContainer.gameObject.SetActive(false);
    }

    public void OnOpenRoomsListClick()
    {
        _homeContainer.gameObject.SetActive(false);
        _createRoomContainer.gameObject.SetActive(false);
        _openRoomsListContainer.gameObject.SetActive(true);
    }

    public void OnHomeClick()
    {
        _homeContainer.gameObject.SetActive(true);
        _createRoomContainer.gameObject.SetActive(false);
        _openRoomsListContainer.gameObject.SetActive(false);
    }

}
