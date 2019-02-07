using System;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using Fool_online.Scripts.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fool_online.Scripts
{
    /// <summary>
    /// Class that opens scene NextScene on succesful connect to game server
    /// </summary>
    public class OnConnectedScene : MonoBehaviourFoolNetworkObserver
    {
        public string NextScene = "";

        public override void OnConnectedToGameServer()
        {
            SceneManager.LoadScene(NextScene);
            this.enabled = false;
        }

        private void Update()
        {
            if (FoolNetwork.connectionState == FoolNetwork.ConnectionState.Connected)
            {
                SceneManager.LoadScene(NextScene);
                this.enabled = false;
            }
        }

    }
}
