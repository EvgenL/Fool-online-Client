using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts.FoolNetworkScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Logs player out by calling ExitAccount
/// </summary>
public class LogOut : MonoBehaviour
{
    public bool OpenSceneAfterExit = true;

    public string LogoutSceneName = "Login register";

    public void ExitAccount()
    {
        PlayerPrefs.SetString("RememberMe", "false");

        FoolNetwork.Disconnect("User log out");

        if (OpenSceneAfterExit)
        {
            SceneManager.LoadScene(LogoutSceneName);
        }
    }
}
