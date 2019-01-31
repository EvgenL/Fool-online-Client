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

    public void OnCreateRoomClick()
    {
        _homeContainer.gameObject.SetActive(false);
        _createRoomContainer.gameObject.SetActive(true);
    }

    public void OnExitCreateRoomClick()
    {
        _homeContainer.gameObject.SetActive(true);
        _createRoomContainer.gameObject.SetActive(false);
    }

}
