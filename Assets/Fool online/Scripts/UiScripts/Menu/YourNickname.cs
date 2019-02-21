﻿using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine.UI;

namespace Fool_online.Scripts.UiScripts.Menu
{
    /// <summary>
    /// Displays player name
    /// onto Text component
    /// </summary>
    public class YourNickname : MonoBehaviourFoolObserver
    {
        // Start is called before the first frame update
        void Start()
        {
            var text = GetComponent<Text>();
            text.text = FoolNetwork.LocalPlayer.Nickname;
        }

        public override void OnUpdateUserData(long connectionId, string userId, string nickname)
        {
            var text = GetComponent<Text>();
            text.text = nickname;
        }
    }
}
