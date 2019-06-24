using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyDisplay : MonoBehaviourFoolObserver
{
    [SerializeField] private Text _currencyText;

    void Start()
    {
        // if player data was recieved
        if (!string.IsNullOrEmpty(FoolNetwork.LocalPlayer.Nickname))
        {
            _currencyText.text = FoolNetwork.LocalPlayer.Money.ToString("N2");
        }
        else
        {
            _currencyText.text = "--";
        }
    }

    public override void OnUpdateUserData(long connectionId, long userId, string nickname, double money, string avatarFile)
    {
        Debug.Log("Money was updated", this);
        _currencyText.text = money.ToString("N2");
    }
}
