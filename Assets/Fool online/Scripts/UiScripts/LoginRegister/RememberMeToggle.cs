using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RememberMeToggle : MonoBehaviour
{
    public Toggle Toggle;

    private void Awake()
    {
        Toggle.isOn = PlayerPrefs.GetString("RememberMe") == "true";
    }
}
