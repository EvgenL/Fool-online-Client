using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Fool_online.Scripts.FoolNetworkScripts.AccountsServer;
using Fool_online.Scripts.FoolNetworkScripts;
using UnityEngine;
using UnityEngine.UI;

public class Payment : MonoBehaviour {
    public InputField InputSum, OutputSum;

    public void Input() {
        string sum     = InputSum.text;
        string request = $"http://{AccountsTransport.AccountsServerIp}/payment/?user_id={FoolNetwork.LocalPlayer.UserId}&sum={sum}";
        Application.OpenURL(request);
    }

    public void Output() {
        float sum = (float) Convert.ToDouble(OutputSum.text);
        ClientSendPackets.Send_WithdrawFunds(sum);
    }
}
