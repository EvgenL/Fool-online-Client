using Fool_online.Scripts.Manager;
using UnityEngine;

namespace Fool_online.Scripts
{
    public class ReconnectButton : MonoBehaviour
    {
        public void OnClick()
        {
            NetworkManager.Instance.Connect();
        }
    }
}
