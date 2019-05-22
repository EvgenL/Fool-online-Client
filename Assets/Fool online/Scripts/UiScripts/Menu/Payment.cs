using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Fool_online.Scripts.FoolNetworkScripts.AccountsServer;
using Fool_online.Scripts.FoolNetworkScripts;
using UnityEngine;
using UnityEngine.UI;

public class Payment : MonoBehaviour
{

    public int PayServerPort = 5056;

    public InputField InputSum, OutputSum, Requisites;

    public void Input() {
        string sum     = InputSum.text;
        string request = $"http://{FoolWebClient.GetIp()}:{PayServerPort}/payment/?user_id={FoolNetwork.LocalPlayer.UserId}&sum={sum}";
        Application.OpenURL(request);
    }

    public void Output()
    {
        float sum = (float)Convert.ToDouble(OutputSum.text);
        string requsites = Requisites.text;
        ClientSendPackets.Send_WithdrawFunds(sum, requsites);
    }
}
