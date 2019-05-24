using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Fool_online.Scripts.UiScripts.Dialogues
{

    /// <summary>
    /// Dialugue containing text box and yes/no buttons
    /// </summary>
    public class DialogueOk : MonoBehavoirDialogue
    {
        [Header("Button Ok")]
        [SerializeField] private Button _okButton;
        [SerializeField] private Text _okText;

        [Header("Body")]
        [SerializeField] private Text _bodyText;

        /// <summary>
        /// Buffered action on yes button click
        /// </summary>
        private Action<object> _onOk;
        private object _onOkParameter;

        /// <summary>
        /// Dialugue containing text box and OK button
        /// No callbacks
        /// </summary>
        /// <param name="bodyText">Text inside box</param>
        public void ShowOk(string bodyText)
        {
            this._bodyText.text = bodyText;

            _okButton.onClick.AddListener(Hide);

            ShowWindow();
        }

        private void ShowWindow()
        {
            gameObject.SetActive(true);
        }

    }

}
