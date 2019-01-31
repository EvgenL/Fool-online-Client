using Fool_online.Scripts.FoolNetworkScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fool_online.Scripts.InRoom
{
    public class GiveUpButton : MonoBehaviour
    {
        public void OnClick()
        {
            FoolNetwork.GiveUp();
            SceneManager.LoadScene("Main menu");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //TODO give up confirmation
            }
        }
    }
}
