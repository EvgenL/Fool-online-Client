using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnonLoginInputField : MonoBehaviour
{
    [Header("Object from where text would be taken on submit")]
    public InputField field;

    public void OnSubmit()
    {
        AccountsServerConnection.Instance.AnonymousLogin(field.text);
    }

}
