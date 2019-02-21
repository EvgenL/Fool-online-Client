using Fool_online.Scripts.FoolNetworkScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts.UiScripts.LoginRegister
{
    public class AnonLoginInputField : MonoBehaviour
    {
        [Header("Object from where text would be taken on submit")]
        public InputField field;

        private void Start()
        {
        
        }

        public void OnSubmit()
        {
            AccountsServerConnection.Instance.AnonymousLogin(field.text);
        }

    }
}
