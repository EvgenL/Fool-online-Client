using Manager;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MyPlayerInfo : MonoBehaviour
    {
        public Text NicknameText;

        public Sprite Userpic; //TODO

        public GameObject ReadyCheckmark;

        public void Init()
        {
            NicknameText.text = PhotonNetwork.LocalPlayer.NickName;
        }

        public void OnToggleClick(bool value)
        {
            PhotonNetwork.LocalPlayer.SetBoolProperty("Ready", value);
            ServerRoomApi.Instance.OnPlayerClickReadyCheckmark(PhotonNetwork.LocalPlayer, value);
        }


        public void PlayerGetReady()
        {
            print("My player: Get ready!");
            ReadyCheckmark.SetActive(true);
            Debug.Log("ReadyCheckmark is active: " + ReadyCheckmark.activeSelf, ReadyCheckmark);
            ReadyCheckmark.GetComponentInChildren<Toggle>().isOn = false;
        }
        public void PlayerStopGetReady()
        {

            print("My player: stop geting ready!");
            ReadyCheckmark.GetComponentInChildren<Toggle>().isOn = false;
            ReadyCheckmark.SetActive(false);
        }
    }
}
