using Assets.Fool_online.Scripts.UiScripts.Dialogues;
using System;
using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviourFoolObserver
{
    #region Singleton

    public static DialogueManager Instance;

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
    }

    #endregion

    [Header("Dialogue box objects")]
    [SerializeField] private DialogueYesNo _dialogueYesNo;
    [SerializeField] private DialogueOk _dialogueOk;
    [SerializeField] private DialogueProfile _dialogueProfile;

    [Header("Background")]
    [SerializeField] private Image _background;

    private MonoBehavoirDialogue _currentDialogue;

    /// <summary>
    /// Show YesNo with yes and no callbacks
    /// </summary>
    public void ShowYesNo(string bodyText, string yesText, string noText, Action<object> onYes, object onYesParameter, Action onNo = null)
    {
        _dialogueYesNo.ShowYesNo(bodyText, yesText, noText, onYes, onYesParameter, onNo);
        _currentDialogue = _dialogueYesNo;

        ShowBackground();
    }

    /// <summary>
    /// Show YesNo with yes callback
    /// </summary>
    public void ShowYesNo(string bodyText, string yesText, string noText, Action onYes)
    {
        _dialogueYesNo.ShowYesNo(bodyText, yesText, noText, delegate (object o) { onYes?.Invoke(); }, null, null);
        _currentDialogue = _dialogueYesNo;

        ShowBackground();
    }

    /// <summary>
    /// Show YesNo with yes callback and with parameter for yes callback
    /// </summary>
    public void ShowYesNo(string bodyText, string yesText, string noText, Action<object> onYes, object onYesParameter)
    {
        _dialogueYesNo.ShowYesNo(bodyText, yesText, noText, onYes, onYesParameter, null);
        _currentDialogue = _dialogueYesNo;

        ShowBackground();
    }


    /// <summary>
    /// Show Ok with no callbacks
    /// </summary>
    public void ShowOk(string bodyText)
    {
        _dialogueOk.ShowOk(bodyText);
        _currentDialogue = _dialogueOk;

        ShowBackground();
    }


    public void ShowProffile(FoolPlayer player)
    {
        ShowBackground();

        _dialogueProfile.Show(player);
        _currentDialogue = _dialogueProfile;

    }

    public void ShowMyProffile()
    {
        ShowBackground();

        _dialogueProfile.ShowMy();
        _currentDialogue = _dialogueProfile;
    }

    private void ShowBackground()
    {
        _background.gameObject.SetActive(true);
    }

    private void HideBackground()
    {
        _background.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called by dialogue when user clicked yes or no
    /// </summary>
    public void OnDialogueCompleted()
    {
        HideBackground();
    }

    public void OnBackgroundClick()
    {
        _currentDialogue.Hide();
        HideBackground();
    }

    public override void OnMessage(string message)
    {
        ShowOk(message);
    }
}
