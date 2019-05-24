using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Fool_online.Scripts.UiScripts.Dialogues
{
    /// <summary>
    /// Dialugue containing text box and yes/no buttons
    /// </summary>
    public class DialogueYesNo : MonoBehavoirDialogue
    {
        [Header("Button Yes")]
        [SerializeField] private Button _yesButton;
        [SerializeField] private Text _yesText;
    
        [Header("Button No")]
        [SerializeField] private Button _noButton;
        [SerializeField] private Text _noText;

        [Header("Body")]
        [SerializeField] private Text _bodyText;

        /// <summary>
        /// Buffered action on yes button click
        /// </summary>
        private Action<object> _onYes;
        private object _onYesParameter;

        /// <summary>
        /// Dialugue contiineng text box and yes/no buttons
        /// </summary>
        /// <param name="bodyText">Text inside box</param>
        /// <param name="yesText">Text on YES button</param>
        /// <param name="noText">Text on NO button</param>
        /// <param name="onYes">Action called on YES button click</param>
        /// <param name="onYesParameter">Parameter of action called on YES button click</param>
        /// <param name="onNo">Action called on NO button click</param>
        public void ShowYesNo(string bodyText, string yesText, string noText, Action<object> onYes, object onYesParameter, Action onNo = null)
        {
            this._onYes = onYes;
            this._onYesParameter = onYesParameter;

            this._bodyText.text = bodyText;

            _yesButton.onClick.RemoveAllListeners();

            _yesButton.onClick.AddListener(delegate {
                onYes(onYesParameter);
                Hide();
            });

            ShowWindow();
        }
        
        /// <summary>
        /// Called by clicking any button
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            DialogueManager.Instance.OnDialogueCompleted();
        }
    }
}
