using UnityEngine;

namespace Fool_online.Scripts.Network
{
    public class ConnectRandomBtn : MonoBehaviour
    {
        public void OnClick()
        {
            FoolNetwork.JoinRandom();
        }
    }
}
