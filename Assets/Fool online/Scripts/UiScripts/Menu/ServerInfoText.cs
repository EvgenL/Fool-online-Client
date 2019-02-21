using Fool_online.Scripts.FoolNetworkScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.UiScripts.Menu
{
    /// <summary>
    /// Displays server to which we are connected
    /// onto Text component
    /// </summary>
    public class ServerInfoText : MonoBehaviour
    {
        void Start()
        {
            var text = GetComponent<Text>();
            text.text = FoolNetwork.ServerInfo;
        }
    }
}
