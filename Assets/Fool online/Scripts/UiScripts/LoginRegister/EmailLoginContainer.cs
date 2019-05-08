using Assets.Fool_online.Scripts.FoolNetworkScripts.AccountsServer;
using Fool_online.Scripts.FoolNetworkScripts.AccountsServer.Packets;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
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

    [Header("Login Manager to send login data to server")]
    [SerializeField] private LoginManager _loginManager;

    private void Awake()
    {
        // Get email if was saved
        Email.text = PlayerPrefs.GetString("Email", "");
    }

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


        // if everything went okay
        if (!errorFlag)
        {
            _loginManager.LoginEmail(Email.text, Password.text);
        }
    }

    
}
