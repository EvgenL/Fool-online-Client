using System.Collections;
using System.Collections.Generic;
using Assets.Fool_online.Scripts.FoolNetworkScripts.AccountsServer;
using Fool_online.Scripts.FoolNetworkScripts.AccountsServer.Packets;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;

public class LoginManager : MonoBehaviourFoolObserver
{
    private bool RememberMe;
    private string Email;
    private string Password;
    private string LastLoginMethod;


    [SerializeField] private string _accountsServerIp = "51.77.235.237";
    [SerializeField] private int _accountsServerPort = 5054;

    [Header("If check - connect to 127.0.0.1")]
    [SerializeField] private bool _testModeLocalhost = true;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize AccountsTransport with server endpoint
        string ip = _testModeLocalhost ? "127.0.0.1" : _accountsServerIp;
        AccountsTransport.SetIpEndpoint(ip, _accountsServerPort);

        //StartCoroutine(//todo CheckVersion(LoginServerIp, LoginServerPort));
    }

    public void OnRememberMeToggleValueChanged(bool value)
    {
        RememberMe = value;
    }

    /// <summary>
    /// Send login data to server
    /// Data needs to be pre-checked
    /// </summary>
    public void LoginEmail(string email, string password)
    {
        this.Email = email;
        this.Password = password;
        this.LastLoginMethod = "Email";


        string sha1password = AccountsUtil.GetSha1(password);
        AccountPackets.SendEmailLogin(email, sha1password);
    }

    /// <summary>
    /// Send registerq data to server
    /// Data needs to be pre-checked
    /// </summary>
    public void RegisterEmail(string nickname, string email, string password)
    {
        this.Email = email;
        this.Password = password;
        this.LastLoginMethod = "Email";


        string sha1password = AccountsUtil.GetSha1(password);
        AccountPackets.SendEmailRegistration(nickname, email, sha1password);
    }

    /// <summary>
    /// Observer callback
    /// If user wants ro remember his login data
    /// save it to player prefs
    /// </summary>
    public override void OnAuthorizedOk(long connectionId)
    {
        // save account data if needed

        // save RememberToggle value
        if (RememberMe)
        {
            PlayerPrefs.SetString("RememberMe", "true");

            // save account data
            PlayerPrefs.SetString("Email", Email);
            PlayerPrefs.SetString("Password", Password);
            PlayerPrefs.SetString("LastLoginMethod", LastLoginMethod);
        }
        else
        {
            PlayerPrefs.SetString("RememberMe", "false");
        }
    }

}
