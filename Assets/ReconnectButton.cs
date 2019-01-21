using Fool_online.Scripts;
using Fool_online.Scripts.Network;
using UnityEngine;

public class ReconnectButton : MonoBehaviour
{
    public void OnClick()
    {
        NetworkManager.Instance.Connect();
    }
}
