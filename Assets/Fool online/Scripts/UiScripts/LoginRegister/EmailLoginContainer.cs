using Assets.Fool_online.Scripts.FoolNetworkScripts.AccountsServer;
using Fool_online.Scripts.FoolNetworkScripts.AccountsServer.Packets;
using UnityEngine;
using UnityEngine.UI;

public class EmailLoginContainer : MonoBehaviour
{
    [Header("Input fields")]
    [SerializeField] private InputField Email;
    [SerializeField] private InputField Password;

    [Header("Text boxes for errors")]
    [SerializeField] private Text ErrorTextEmail;
    [SerializeField] private Text ErrorTextPassword;


    public void OnSubmit()
    {
        // disable error messages if was
        ErrorTextEmail.gameObject.SetActive(false);
        ErrorTextPassword.gameObject.SetActive(false);

        // read form
        string email = Email.text;
        string password = Password.text;


        bool errorFlag = false;

        // validate email
        if (!AccountsUtil.EmailIsValid(email))
        {
            ErrorTextEmail.text = "Пожалуйста, введите существующий адрес";
            ErrorTextEmail.gameObject.SetActive(true);
            errorFlag = true;
        }

        // validate pass
        if (password.Length < 6)
        {
            ErrorTextPassword.text = "Введите пароль";
            ErrorTextPassword.gameObject.SetActive(true);
            errorFlag = true;
        }

        if (!errorFlag)
        {
            string sha1password = AccountsUtil.GetSha1(password);
            SendAccountData.SendEmailLogin(email, sha1password);
        }
    }
}
