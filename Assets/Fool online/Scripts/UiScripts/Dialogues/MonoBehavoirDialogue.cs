using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Fool_online.Scripts.UiScripts.Dialogues
{
    public abstract class MonoBehavoirDialogue : MonoBehaviour
    {
        /// <summary>
        /// Called by clicking any button
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            DialogueManager.Instance.OnDialogueCompleted();
        }


        protected void ShowWindow()
        {
            // todo add animation there
            gameObject.SetActive(true);
        }
    } 
}
