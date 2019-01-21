using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fool_online.Scripts
{
    public class ButtonRegisterOk : MonoBehaviour
    {
        public string PlayerID;

        public void OnClick()
        {
            PlayerPrefs.SetString("PlayerID", PlayerID);
            SceneManager.LoadScene("Connecting to server");
        }


    }
}
