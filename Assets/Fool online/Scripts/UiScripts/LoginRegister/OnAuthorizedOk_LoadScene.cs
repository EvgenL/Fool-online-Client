using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fool_online.Scripts.UiScripts.LoginRegister
{
    /// <summary>
    /// Class that opens scene NextScene on succesful connect to game server
    /// </summary>
    public class OnAuthorizedOk_LoadScene : MonoBehaviourFoolObserver
    {
        [Header("Next scene name")]
        [SerializeField] private string NextScene = "";

        public override void OnAuthorizedOk(long connectionId)
        {
            // load scene
            Debug.Log("Loading next scene");
            SceneManager.LoadScene(NextScene);
        }
    }
}
