using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;

public class LoginIfRememberMe : MonoBehaviourFoolObserver
{
    [SerializeField] private LoginManager _loginManager;

    void Start()
    {
        if (PlayerPrefs.GetString("RememberMe") == "true")
        {
            string email = PlayerPrefs.GetString("Email");
            string pass = PlayerPrefs.GetString("Password");
            string method = PlayerPrefs.GetString("LastLoginMethod");

            if (method == "Email")
            {
                _loginManager.LoginEmail(email, pass);
            }
        }
    }

    /// <summary>
    /// Authorizetion failed
    /// </summary>
    public override void OnErrorBadAuthToken()
    {
        PlayerPrefs.SetString("RememberMe", "false");
    }
}
