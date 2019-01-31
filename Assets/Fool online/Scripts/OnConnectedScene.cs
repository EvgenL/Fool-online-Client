using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fool_online.Scripts
{
    public class OnConnectedScene : MonoBehaviour   
    {
        public string NextScene = "";

        private void Start()
        {
            NetworkManager.Instance.Connect();
        }

        private void Update()
        {
            if (FoolNetwork.connectionState == FoolNetwork.ConnectionState.Connected)
            {
                SceneManager.LoadScene(NextScene);
                this.enabled = false;
            }
        }

        /*public override void OnConnectedToGameServer()
    {
        SceneManager.LoadScene(NextScene);
    }*/
    }
}
