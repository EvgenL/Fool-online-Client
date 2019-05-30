using System.Collections;
using System.Collections.Generic;
using Assets.Fool_online.Scripts.UiScripts.Dialogues;
using Fool_online.Scripts.FoolNetworkScripts;
using UnityEngine;
using UnityEngine.UI;

public class DialogueProfile : MonoBehavoirDialogue
{
    [SerializeField] private Text _textNickname;

    private void Awake()
    {
        _textNickname.text = FoolNetwork.LocalPlayer.Nickname;
    }
    
    public void Show(FoolPlayer player)
    {
        base.ShowWindow();
    }

    public void ShowMy()
    {
        base.ShowWindow();
    }
}
