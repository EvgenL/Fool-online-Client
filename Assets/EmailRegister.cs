using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Fool_online.Scripts.FoolNetworkScripts.AccountsServer;
using Fool_online.Scripts.FoolNetworkScripts;
using UnityEngine;
using UnityEngine.UI;

public class EmailRegister : MonoBehaviour
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
        if (!EmailIsValid(email))
        {
            ErrorTextEmail.text = "Пожалуйста, введите существующий адрес";
            ErrorTextEmail.gameObject.SetActive(true);
            errorFlag = true;
        }

        // validate pass
        if (password.Length < 6)
        {
            ErrorTextPassword.text = "Придумайте пароль длинее 6-ти символов";
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
            AccountsClientSendData.SendRegister(nickname, email, password);
        }
    }

    /// <summary>
    /// Checks nickname length
    /// </summary>
    private bool NicknameIsValid(string nickname)
    {
        return nickname.Length >= 3 && nickname.Length <= 20;
    }

    /// <summary>
    /// Checks email is correct
    /// </summary>
    private bool EmailIsValid(string email)
    {
        // Email regexp that 99.9% works
        // emailregex.com
        return (Regex.IsMatch(email,
                "(?:[a-z0-9!#$%&\'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&\'*+/=?^_`{|}~-]+)*|\"(?:" +
                "[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b" +
                "\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])" +
                "?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|" +
                "[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])")
            );
    }
}
