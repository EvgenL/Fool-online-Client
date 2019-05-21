using Assets.Fool_online.Scripts.UiScripts.Dialogues;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
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

    [Header("Backgtound")]
    [SerializeField] private Image _background;

    private IDialogue _currentDialogue;

    public void ShowYesNo(string bodyText, string yesText, string noText, Action<object> onYes, object onYesParameter, Action onNo = null)
    {
        _dialogueYesNo.ShowYesNo(bodyText, yesText, noText, onYes, onYesParameter, onNo);
        _currentDialogue = _dialogueYesNo;

        ShowBackground();
    }

    public void ShowYesNo(string bodyText, string yesText, string noText, Action onYes)
    {
        _dialogueYesNo.ShowYesNo(bodyText, yesText, noText, delegate (object o) { onYes?.Invoke(); }, null, null);
        _currentDialogue = _dialogueYesNo;

        ShowBackground();
    }

    public void ShowYesNo(string bodyText, string yesText, string noText, Action<object> onYes, object onYesParameter)
    {
        _dialogueYesNo.ShowYesNo(bodyText, yesText, noText, onYes, onYesParameter, null);
        _currentDialogue = _dialogueYesNo;

        ShowBackground();
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
}
