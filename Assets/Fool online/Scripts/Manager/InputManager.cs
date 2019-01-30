﻿using Fool_online.Scripts.CardsScripts;
using Fool_online.Scripts.InRoom;
using Fool_online.Scripts.Network.NetworksObserver;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instatnce;


    public RectTransform TableDropZone;

    private void Awake()
    {
        Instatnce = this;
    }

    /// <summary>
    /// Called on each frame when i drag card
    /// </summary>
    public void DraggedCardUpdate(Vector2 mousePos, CardRoot cardRoot)
    {
        //if it was dragged insude table rect
        if (RectTransformUtility.RectangleContainsScreenPoint(TableDropZone, mousePos))
        {
            FoolNetworkObservableCallbacksWrapper.Instance.DraggedCardUpdate(mousePos, cardRoot, true);
        }
        else
        {
            FoolNetworkObservableCallbacksWrapper.Instance.DraggedCardUpdate(mousePos, cardRoot, false);
        }
    }

    public void DraggedCardDrop(Vector2 mousePos, CardRoot cardRoot)
    {
        //if it was dropped insude table rect
        if (RectTransformUtility.RectangleContainsScreenPoint(TableDropZone, mousePos))
        {
            FoolNetworkObservableCallbacksWrapper.Instance.CardDroppedOnTableByMe(cardRoot);
        }
    }
}