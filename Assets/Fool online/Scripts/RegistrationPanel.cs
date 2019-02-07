using UnityEngine;
using UnityEngine.UI;

namespace Fool_online.Scripts
{
    public class RegistrationPanel : MonoBehaviour
    {
        public InputField InputUsername;
        public InputField InputEmail;
        public InputField InputPassword;
        public InputField InputPasswordConfirm;

        public Text TextErrorNickname;
        public Text TextErrorEmail;
        public Text TextErrorPass;

        public void OnSubmitClick()
        {
            if (CheckValues())
            {
                Debug.Log("Sending new account data to server");
                //ClientSendPackets.Send_NewAccount(InputUsername.text, InputPassword.text, InputEmail.text);
            }
        }

        /// <summary>
        /// Checks input fields
        /// </summary>
        /// <returns>true if fields are correct</returns>
        private bool CheckValues()
        {
            bool hadErrors = false;
            TextErrorNickname.text = "";

            //is email correct
            if (Util.TestEmail(InputEmail.text) && InputEmail.text.Length < 39)
            {
                TextErrorEmail.text = "";
            }
            else
            {
                TextErrorEmail.text = "Пожалуйста, введите корректный email адрес.";
                hadErrors = true;
            }

            //are password fields the same
            if (InputPassword.text == InputPasswordConfirm.text)
            {
                TextErrorPass.text = "";
            }
            else
            {
                TextErrorPass.text = "Пароли не совпадают.";
                hadErrors = true;
            }

            //TODO is nickname good

            //TODO is mail, like, really good

            //TODO paswords max 40 sym

            return !hadErrors;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
