using System.Collections;
using System.Collections.Generic;
using Assets.Fool_online.Scripts.FoolNetworkScripts.AccountsServer;
using Fool_online.Scripts.FoolNetworkScripts.AccountsServer.Packets;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviourFoolObserver
{

    #region Singleton

    public static LoginManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
            return;
        }

        // initialize ip endpoints
        Init();
    }

    #endregion

    public bool RememberMe;
    private string Email;
    private string Password;
    private string LastLoginMethod;
    

    [SerializeField] private string _accountsServerIp = "51.77.235.237";
    [SerializeField] private int _accountsServerPort = 5054;

    [Header("If check - connect to 127.0.0.1")]
    [SerializeField] private bool _testModeLocalhost = true;

    [Header("Try login if RememberMe was set")]
    [SerializeField] private bool _loginIfRememberMe = true;

    [Header("Scene name for logging in")]
    [SerializeField] private string _sceneLogin = "Login register";


    /// <summary>
    /// Set ip endpoint
    /// </summary>
    private void Init()
    {
        // Initialize AccountsTransport with server endpoint
        string ip = _testModeLocalhost ? "127.0.0.1" : _accountsServerIp;
        AccountsTransport.SetIpEndpoint(ip, _accountsServerPort);

        //StartCoroutine(//todo CheckVersion(LoginServerIp, LoginServerPort));

        // login if remember me
        RememberMe = PlayerPrefs.GetString("RememberMe") == "true";
        if (_loginIfRememberMe && RememberMe)
        {
            print("Remember me is set. Logging in with last saved player data.");

            string email = PlayerPrefs.GetString("Email");
            string pass = PlayerPrefs.GetString("Password");
            string method = PlayerPrefs.GetString("LastLoginMethod");

            if (method == "Email")
            {
                LoginEmail(email, pass);
            }
        }
        else // open login scene
        {
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(_sceneLogin))
            {
                SceneManager.LoadScene(_sceneLogin);
            }
        }
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

    /// <summary>
    /// Authorization failed
    /// </summary>
    public override void OnErrorBadAuthToken()
    {
        PlayerPrefs.SetString("RememberMe", "false");
    }
}
