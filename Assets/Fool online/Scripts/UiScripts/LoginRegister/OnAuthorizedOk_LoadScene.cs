using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fool_online.Scripts.UiScripts.LoginRegister
{
    /// <summary>
    /// Class that opens scene NextScene on succesful connect to game server
    /// </summary>
    public class OnAuthorizedOk_LoadScene : MonoBehaviourFoolObserver
    {
        public string NextScene = "";

        public override void OnAuthorizedOk(long connectionId)
        {
            Debug.Log("Loading next scene");
            SceneManager.LoadScene(NextScene);
        }
    }
}
