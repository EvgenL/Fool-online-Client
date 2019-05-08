using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleHideBakground : MonoBehaviour
{
    public Image ImageToHide;

    public void OnValueChanged(bool value)
    {
        ImageToHide.enabled = !value;
    }
}
