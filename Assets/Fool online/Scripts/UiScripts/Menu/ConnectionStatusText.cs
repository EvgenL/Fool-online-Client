using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.UiScripts.Menu
{
    public class ConnectionStatusText : MonoBehaviour //FoolNetworkObserver
    {
        public Text TargetText;
        public GameObject ReconnectButton;

        private void Update()
        {
            if (NetworkManager.Instance.ConnectionState == FoolNetwork.ConnectionState.ConnectingGameServer)
            {
                TargetText.text = "Загрузка";
                ReconnectButton.SetActive(false);
            }
            else if (NetworkManager.Instance.ConnectionState == FoolNetwork.ConnectionState.Disconnected)
            {
                //if wo got some error
                if (!string.IsNullOrEmpty(FoolNetwork.DisconnectReasonText))
                {
                    TargetText.text = FoolNetwork.DisconnectReasonText;
                }
                else
                {
                    TargetText.text = "Сервер недоступен";
                }
                ReconnectButton.SetActive(true);
            }
        }
        /*
    //Observer callback
    public override void OnTryingConnectToGameServer()
    {
        TargetText.text = "Загрузка";
        ReconnectButton.SetActive(false);
    }

    //Observer callback
    public override void OnDisconnectedFromGameServer()
    {
        TargetText.text = "Сервер недоступен";
        ReconnectButton.SetActive(true);
    }*/
    }
}
