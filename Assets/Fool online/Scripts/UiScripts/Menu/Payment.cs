using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Fool_online.Scripts.FoolNetworkScripts.AccountsServer;
using Fool_online.Scripts.FoolNetworkScripts;
using UnityEngine;
using UnityEngine.UI;

public class Payment : MonoBehaviour
{
    public const int PayServerPort = 5056; // todo recieve from server

    public void WithdtawMoney(float sum, string requsites)
    {
        ClientSendPackets.Send_WithdrawFunds(sum, requsites);
    }

    public void AddMoney(float sum)
    {
        string request = $"http://{FoolWebClient.GetIp()}:{PayServerPort}/payment/?user_id={FoolNetwork.LocalPlayer.UserId}&sum={sum}";
        Application.OpenURL(request);
    }
}
