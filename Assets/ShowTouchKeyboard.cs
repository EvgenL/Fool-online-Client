using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowTouchKeyboard : MonoBehaviour, IPointerClickHandler
{


    public void OnPointerClick(PointerEventData eventData)
    {
        TouchScreenKeyboard.Open("Hello", TouchScreenKeyboardType.Default);
    }
}
