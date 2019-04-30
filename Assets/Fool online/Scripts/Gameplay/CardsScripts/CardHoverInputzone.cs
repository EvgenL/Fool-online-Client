using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts.InRoom.CardsScripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoverInputzone : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Card reference")]
    [SerializeField] private InteractibleCard _interactibleCard;

    private Vector2 baseSizeDelta;

    private void Awake()
    {
        // remember base size
        baseSizeDelta = (_interactibleCard.transform as RectTransform).sizeDelta;

        // set current size to card's visual size
        var rectTransform = transform as RectTransform;
        rectTransform.sizeDelta = baseSizeDelta;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_interactibleCard.CanBeDragged
            || _interactibleCard.IsDragged) return;

        var sizedelta = baseSizeDelta;
        sizedelta.y += _interactibleCard.CardMoveUpOnHover;

        var rectTransform = transform as RectTransform;
        rectTransform.sizeDelta = sizedelta;
        rectTransform.position += Vector3.up * _interactibleCard.CardMoveUpOnHover / 2f;

        _interactibleCard.Zoom();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_interactibleCard.CanBeDragged
            || _interactibleCard.IsDragged) return;

        var rectTransform = transform as RectTransform;
        rectTransform.sizeDelta = baseSizeDelta;
        rectTransform.position = transform.parent.position;

        _interactibleCard.UnZoom();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        _interactibleCard.BeginDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _interactibleCard.UpdateDrag();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _interactibleCard.EndDrag();
    }


   /* public void OnPointerDown()
    {
    //TODO touchscreen zoom card controll
        //Zoom();
    }
    public void OnPointerUp()
    {
        //UnZoom();
    }*/
}
