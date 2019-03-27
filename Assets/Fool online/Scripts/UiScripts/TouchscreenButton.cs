using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TouchscreenButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Button that reacts to pointer down")]

    /// <summary>
    /// Allow read input
    /// </summary>
    public bool Interactible = true;

    public UnityEvent OnPointerDownEvent;
    public UnityEvent OnPointerUpEvent;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (Interactible)
            OnPointerDownEvent.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Interactible)
            OnPointerUpEvent.Invoke();
    }
}
