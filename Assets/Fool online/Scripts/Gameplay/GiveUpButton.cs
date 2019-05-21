using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fool_online.Scripts.InRoom
{
    public class GiveUpButton : MonoBehaviour
    {
        public void OnClick()
        {
            // game is playing?
            if (StaticRoomData.IsPlaying)
            {
                // if i did not win
                if (!StaticRoomData.MyPlayer.Won)
                {
                    // ask to give up
                    DialogueManager.Instance.ShowYesNo("Сдаться?", "Да", "Нет", OnActionConfirmed);
                    return;
                }
            }

            // if game isn't playing - just leave
            OnActionConfirmed();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnClick();
            }
        }

        private void OnActionConfirmed()
        {
            FoolNetwork.LeaveRoom();
            SceneManager.LoadScene("Main menu");
        }
    }
}
