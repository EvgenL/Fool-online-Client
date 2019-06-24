using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DOTween.Modules;
using UnityEngine;


// TODO refractor


public class TabMarkerAnimation : MonoBehaviour
{

    [SerializeField] private float _animationLength = 0.3f;
    [SerializeField] private Ease _animationEase = Ease.OutCubic;

    public FooterBarButton[] _buttons;

    private FooterBarButton currentHighlighted;

    private float _constWidthAdd;
    private float _constHeight;
    private float _constYpos;

    private static int _lastSelectedNumber = 0;
    private FooterBarButton _lastSelectedButton; 
    private RectTransform lastSelectedRect; 

    private FooterBarButton _selectedButton;

    private void Start()
    {
        if (_lastSelectedNumber < 0) _lastSelectedNumber = 0;

        SelectFirst(_buttons[_lastSelectedNumber]);
    }

    public void Select(int pageNumber)
    {
        Select(_buttons[pageNumber]);
    }

    public void SelectFirst(FooterBarButton target)
    {
        _lastSelectedButton = target;
        lastSelectedRect = target.ButtonBounds;

        var rectTransform = transform as RectTransform;

        _constWidthAdd = rectTransform.sizeDelta.x - lastSelectedRect.sizeDelta.x;
        _constHeight = rectTransform.sizeDelta.y;
        _constYpos = rectTransform.position.y;
        _constWidthAdd = rectTransform.sizeDelta.x - lastSelectedRect.sizeDelta.x;

        // set pos
        rectTransform.position 
            = new Vector3(lastSelectedRect.position.x, // x
            _constYpos);  // y

        // set scale
        rectTransform.sizeDelta 
            = new Vector3(lastSelectedRect.sizeDelta.x + _constWidthAdd, // x
            _constHeight);  // y

    }

    public void Select(FooterBarButton target)
    {
        if (_lastSelectedButton != null) _lastSelectedButton.ShowNormalSprite();

        _lastSelectedNumber = _buttons.ToList().FindIndex(c => c == target);

        _lastSelectedButton = target;

        lastSelectedRect = target.ButtonBounds;
        // var lastSelectedRect = target.transform as RectTransform;
        
        var rectTransform = transform as RectTransform;

        // do move
        Vector2 targetPos = lastSelectedRect.position;
        targetPos.y = rectTransform.position.y;

        // init animation
        rectTransform.DOMove(targetPos, _animationLength).SetEase(_animationEase);

        // do scale
        Vector2 newSizeDelta;

        newSizeDelta.x = lastSelectedRect.sizeDelta.x + _constWidthAdd;
        newSizeDelta.y = _constHeight;

        // init animation
        rectTransform.DOSizeDelta(newSizeDelta, _animationLength).SetEase(_animationEase);
    }

    public void Highlight(FooterBarButton target)
    {
        currentHighlighted = target;
        var currentHighlightedRect = target.ButtonBounds;

        var rectTransform = transform as RectTransform;
        // var lastSelectedRect = target.transform as RectTransform;

        // do move
        // mid point between two vectors = (a + b) / 2f
        Vector2 midPoint = (currentHighlightedRect.position + lastSelectedRect.position) / 2f;
        midPoint.y = rectTransform.position.y;

        // init animation
        rectTransform.DOMove(midPoint, _animationLength).SetEase(_animationEase);
        
        // do scale
        Vector2 newSizeDelta;

        bool targetOnRight = currentHighlightedRect.position.x > rectTransform.position.x;

        float leftX;
        float rightX;
        Vector3 localPosSelected = transform.InverseTransformPoint(lastSelectedRect.position);
        Vector3 localPosHighlighted = transform.InverseTransformPoint(currentHighlightedRect.position);

        if (targetOnRight)
        {
            leftX = localPosSelected.x + lastSelectedRect.rect.xMin;
            rightX = localPosHighlighted.x + currentHighlightedRect.rect.xMax;
        }
        else
        {
            leftX = localPosSelected.x + lastSelectedRect.rect.xMax;
            rightX = localPosHighlighted.x + currentHighlightedRect.rect.xMin;
        }

        newSizeDelta.x = Mathf.Abs(leftX) + Mathf.Abs(rightX);
        //newSizeDelta.x += _constWidthAdd;
        newSizeDelta.y = _constHeight;

        // init animation
        rectTransform.DOSizeDelta(newSizeDelta, _animationLength).SetEase(_animationEase);


        lastSelectedRect = target.ButtonBounds;
    }

    public void ShowBackButton()
    {
        _lastSelectedButton.ShowBackSprite();
    }
    public void HideBackButton()
    {
        _lastSelectedButton.ShowNormalSprite();
    }

    private void AnimateMoveFromTo(RectTransform from, RectTransform to)
    {
        var rectTransform = transform as RectTransform;

        Vector2 targetPos;
        Vector2 newSizeDelta;

        // first move
        if (from == to)
        {
            
        }
        // else

    }

    public void StopHighlight()
    {
        Select(_lastSelectedButton);
    }
}
