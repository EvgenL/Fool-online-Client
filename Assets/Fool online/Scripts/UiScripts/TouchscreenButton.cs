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
    public UnityEvent OnDeselectedEvent;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (Interactible)
            OnPointerDownEvent.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Interactible)
        {
            RectTransform rectTransform = transform as RectTransform;

            var pointerPos = rectTransform.InverseTransformPoint(eventData.position);

            if (rectTransform.rect.Contains(pointerPos))
            {
                OnPointerUpEvent.Invoke();
            }
            else
            {
                OnDeselectedEvent.Invoke();
            }
        }
    }
}
