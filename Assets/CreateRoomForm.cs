using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomForm : MonoBehaviour
{
    [SerializeField]  private int _currMaxPlayers = 3;
    [SerializeField]  private int _currDeckSize = 36;

    [Header("Max players option")]
    [SerializeField] private Toggle _maxPlayers2;
    [SerializeField] private Toggle _maxPlayers3;
    [SerializeField] private Toggle _maxPlayers4;
    [SerializeField] private Toggle _maxPlayers5;
    [SerializeField] private Toggle _maxPlayers6;

    [Header("Deck size option")]
    [SerializeField] private Toggle _buttonDeckSize24;
    [SerializeField] private Toggle _buttonDeckSize36;
    [SerializeField] private Toggle _buttonDeckSize52;


    private void Awake()
    {
        //if this player ever created a room then we load his previous settings
        _currMaxPlayers = PlayerPrefs.GetInt("LastRoomMaxPlayers", _currMaxPlayers);
        _currDeckSize = PlayerPrefs.GetInt("LastRoomDeckSize", _currDeckSize);

        _maxPlayers2.isOn = false;
        _maxPlayers3.isOn = false;
        _maxPlayers4.isOn = false;
        _maxPlayers5.isOn = false;
        _maxPlayers6.isOn = false;

        _buttonDeckSize24.isOn = false;
        _buttonDeckSize36.isOn = false;
        _buttonDeckSize52.isOn = false;

        switch (_currMaxPlayers)
        {
            case 2:
                _maxPlayers2.isOn = true;
                break;
            case 3:
                _maxPlayers3.isOn = true;
                break;
            case 4:
                _maxPlayers4.isOn = true;
                break;
            case 5:
                _maxPlayers5.isOn = true;
                break;
            case 6:
                _maxPlayers6.isOn = true;
                break;
        }

        switch (_currDeckSize)
        {
            case 24:
                _buttonDeckSize24.isOn = true;
                break;
            case 36:
                _buttonDeckSize36.isOn = true;
                break;
            case 52:
                _buttonDeckSize52.isOn = true;
                break;
        }

        CheckDeckSizeToPlayers();
    }

    /// <summary>
    /// Callback of every button on Max Player option
    /// </summary>
    /// <param name="maxPlayers">2-3-4-5-6 value</param>
    public void OnMaxPlayersChange()
    {
        int maxPlayers =
            _maxPlayers2.isOn ? 2 :
            _maxPlayers3.isOn ? 3 :
            _maxPlayers4.isOn ? 4 :
            _maxPlayers5.isOn ? 5 :
            _maxPlayers6.isOn ? 6 : -1;


        if (maxPlayers < 2 || maxPlayers > 6)
        {
            //Debug.LogError("Wrong MaxPlayers value: " + maxPlayers);
            return;
        }

        //Buffer value. Save to player prefs.
        if (_currMaxPlayers != maxPlayers)
        {
            _currMaxPlayers = maxPlayers;
            PlayerPrefs.SetInt("LastRoomMaxPlayers", _currMaxPlayers);
        }
        CheckDeckSizeToPlayers();
    }


    /// <summary>
    /// Callback of every button on Deck Size option
    /// </summary>
    /// <param name="maxPlayers">24-36-52 value</param>
    public void OnDeckSizeChange()
    {
        int deckSize =
            _buttonDeckSize24.isOn ? 24 :
            _buttonDeckSize36.isOn ? 32 :
            _buttonDeckSize52.isOn ? 52 : -1;

        if (!(deckSize == 24 || deckSize == 36 || deckSize == 52))
        {
            //Debug.LogError("Wrong DeckSize value: " + deckSize);
            return;
        }

        //Buffer value. Save to player prefs.
        if (_currDeckSize != deckSize)
        {
            _currDeckSize = deckSize;
            PlayerPrefs.SetInt("LastRoomDeckSize", deckSize);
        }

        CheckDeckSizeToPlayers();
    }

    private void CheckDeckSizeToPlayers()
    {
        //Can't use dect size of 24 for 4 players because that creates hight disadvantage for player defending first
        if (_currMaxPlayers > 4)
        {
            if (_buttonDeckSize24.isOn)
            {
                _buttonDeckSize36.isOn = true;
            }

            _buttonDeckSize24.isOn = false;
            _buttonDeckSize24.interactable = false;
        }
        else
        {
            _buttonDeckSize24.interactable = true;
        }

        //Can't use dect size of 36 for 6 players because that creates hight disadvantage for player defending first
        if (_currMaxPlayers > 5)
        {
            if (_buttonDeckSize24.isOn || _buttonDeckSize36.isOn)
            {
                _buttonDeckSize52.isOn = true;
            }

            _buttonDeckSize36.isOn = false;
            _buttonDeckSize36.interactable = false;
        }
        else
        {
            _buttonDeckSize36.interactable = true;
        }

        //Can't use dect size of 24 for 4 players because that creates hight disadvantage for player defending first
        if (_currDeckSize < 36)
        {
            if (_maxPlayers6.isOn)
            {
                _maxPlayers5.isOn = true;
            }

            _maxPlayers6.isOn = false;
            _maxPlayers6.interactable = false;
        }
        else
        {
            _maxPlayers6.interactable = true;
        }
    }

    public void OnSubmit()
    {

    }
}
