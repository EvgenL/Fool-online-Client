using Assets.Fool_online.Scripts.FoolNetworkScripts.AccountsServer;
using Fool_online.Scripts.FoolNetworkScripts.AccountsServer.Packets;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.UiScripts.LoginRegister
{
    public class AnonLoginContainer : MonoBehaviour
    {
        [Header("Input fields")]
        [SerializeField] private InputField Nickname;

        [Header("Text boxes for errors")]
        [SerializeField] private Text ErrorTextNickname;

        public void OnSubmit()
        {
            // disable error messages if was
            ErrorTextNickname.gameObject.SetActive(false);

            // read form
            string nickname = Nickname.text;

            bool errorFlag = false;

            // validate name
            if (!EmailRegisterContainer.NicknameIsValid(nickname))
            {
                ErrorTextNickname.text = "Имя должно содержать больше 2х символов";
                ErrorTextNickname.gameObject.SetActive(true);
                errorFlag = true;
            }

            if (!errorFlag)
            {
                SendAccountData.SendAnonLogin(nickname);
            }
        }
    }
}
