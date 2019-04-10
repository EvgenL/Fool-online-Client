using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Assets.Fool_online.Scripts.FoolNetworkScripts.AccountsServer;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.AccountsServer.Packets;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;
using UnityEngine.UI;

public class EmailRegisterContainer : MonoBehaviour
{
    [Header("Input fields")]
    [SerializeField] private InputField Nickname;
    [SerializeField] private InputField Email;
    [SerializeField] private InputField Password;
    [SerializeField] private InputField PasswordRepeat;

    [Header("Text boxes for errors")]
    [SerializeField] private Text ErrorTextNickname;
    [SerializeField] private Text ErrorTextEmail;
    [SerializeField] private Text ErrorTextPassword;


    public void OnSubmit()
    {
        // disable error messages if was
        ErrorTextNickname.gameObject.SetActive(false);
        ErrorTextEmail.gameObject.SetActive(false);
        ErrorTextPassword.gameObject.SetActive(false);

        // read form
        string nickname = Nickname.text;
        string email = Email.text;
        string password = Password.text;
        string passwordRepeat = PasswordRepeat.text;


        bool errorFlag = false;

        // validate name
        if (!NicknameIsValid(nickname))
        {
            ErrorTextNickname.text = "Имя должно содержать больше 2х символов";
            ErrorTextNickname.gameObject.SetActive(true);
            errorFlag = true;
        }

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
            ErrorTextPassword.text = "Придумайте пароль длиннее 6-ти символов";
            ErrorTextPassword.gameObject.SetActive(true);
            errorFlag = true;
        }
        else if (passwordRepeat.Length == 0)
        {
            ErrorTextPassword.text = "Повторите пароль ниже";
            ErrorTextPassword.gameObject.SetActive(true);
            errorFlag = true;
        }
        else if (password != passwordRepeat)
        {
            ErrorTextPassword.text = "Пароли не совпадают";
            ErrorTextPassword.gameObject.SetActive(true);
            errorFlag = true;
        }

        if (!errorFlag)
        {
            string sha1password = AccountsUtil.GetSha1(password);
            AccountPackets.SendEmailRegistration(nickname, email, sha1password);
        }
    }

    /// <summary>
    /// Checks nickname length
    /// </summary>
    public static bool NicknameIsValid(string nickname)
    {
        return nickname.Length >= 3 && nickname.Length <= 20;
    }
}
