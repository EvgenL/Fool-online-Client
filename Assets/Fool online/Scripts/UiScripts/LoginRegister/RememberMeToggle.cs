using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RememberMeToggle : MonoBehaviour
{
    private Toggle _toggle;

    private void Start()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = PlayerPrefs.GetString("RememberMe") == "true";
    }

    public void OnValueChanged(bool value)
    {
        LoginManager.Instance.RememberMe = value;
    }
}
