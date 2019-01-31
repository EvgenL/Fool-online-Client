using UnityEngine;

namespace Fool_online.Scripts.FoolNetworkScripts
{
    public class ConnectRandomBtn : MonoBehaviour
    {
        public void OnClick()
        {
            FoolNetwork.JoinRandom();
        }
    }
}
