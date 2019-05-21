using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts.FoolNetworkScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Logs player out by calling ExitAccount
/// </summary>
public class LogOutButton : MonoBehaviour
{
    public bool OpenSceneAfterExit = true;

    public string LogoutSceneName = "Login register";

    public void ExitAccount()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowYesNo("Выйти из аккаунта?", "Да", "Нет", ExitConfirmedAction, LogoutSceneName);
        }
        else
        {
            ExitConfirmedAction(LogoutSceneName);
        }
    }

    private void ExitConfirmedAction(object nextSceneName)
    {
        PlayerPrefs.SetString("RememberMe", "false");

        FoolNetwork.Disconnect("User log out");

        if (OpenSceneAfterExit)
        {
            SceneManager.LoadScene((string)nextSceneName);
        }
    }
}
