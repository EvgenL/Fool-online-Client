using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fool_online.Scripts
{
    /// <summary>
    /// Class that opens scene NextScene on succesful connect to game server
    /// </summary>
    public class OnAuthorizedLoadScene : MonoBehaviourFoolObserver
    {
        public string NextScene = "";

        public override void OnAuthorizedOk(long cid)
        {
            Debug.Log("Loading next scene");
            SceneManager.LoadScene(NextScene);
            this.enabled = false;
        }

    }
}
