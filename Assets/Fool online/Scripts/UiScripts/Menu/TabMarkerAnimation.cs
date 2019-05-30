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

    public RectTransform[] _containers;

    private RectTransform currntHighlighted;

    private float _constWidthAdd;
    private float _constHeight;
    private float _constYpos;

    private static int _lastSelected = 0;
    private RectTransform _lastSelectedRect;

    private void Start()
    {
        if (_lastSelected < 0) _lastSelected = 0;

        SetFirstTarget(_containers[_lastSelected]);
    }

    public void Select(int pageNumber)
    {
        Select(_containers[pageNumber]);
    }

    public void SetFirstTarget(RectTransform target)
    {
        _lastSelectedRect = target;

        var rectTransform = transform as RectTransform;

        _constWidthAdd = rectTransform.sizeDelta.x - _lastSelectedRect.sizeDelta.x;
        _constHeight = rectTransform.sizeDelta.y;
        _constYpos = rectTransform.position.y;
        _constWidthAdd = rectTransform.sizeDelta.x - target.sizeDelta.x;

        // set pos
        rectTransform.position 
            = new Vector3(target.position.x, // x
            _constYpos);  // y

        // set scale
        rectTransform.sizeDelta 
            = new Vector3(target.sizeDelta.x + _constWidthAdd, // x
            _constHeight);  // y

        return;
    }

    public void Select(RectTransform target)
    {
        _lastSelected = _containers.ToList().FindIndex(c => c == target);

        _lastSelectedRect = target;

        var rectTransform = transform as RectTransform;

        // do move
        Vector2 targetPos = _lastSelectedRect.position;
        targetPos.y = rectTransform.position.y;

        // init animation
        rectTransform.DOMove(targetPos, _animationLength).SetEase(_animationEase);

        // do scale
        Vector2 newSizeDelta;

        newSizeDelta.x = _lastSelectedRect.sizeDelta.x + _constWidthAdd;
        newSizeDelta.y = _constHeight;

        // init animation
        rectTransform.DOSizeDelta(newSizeDelta, _animationLength).SetEase(_animationEase);
    }

    public void Highlight(RectTransform target)
    {
        currntHighlighted = target;

        var rectTransform = transform as RectTransform;

        // do move
        // mid point between two vectors = (a + b) / 2f
        Vector2 midPoint = (currntHighlighted.position + _lastSelectedRect.position) / 2f;
        midPoint.y = rectTransform.position.y;

        // init animation
        rectTransform.DOMove(midPoint, _animationLength).SetEase(_animationEase);

        // do scale
        Vector2 newSizeDelta;

        bool targetOnRight = currntHighlighted.position.x > rectTransform.position.x;

        float leftX;
        float rightX;
        Vector3 localPosSelected = transform.InverseTransformPoint(_lastSelectedRect.position);
        Vector3 localPosHighlighted = transform.InverseTransformPoint(currntHighlighted.position);

        if (targetOnRight)
        {
            leftX = localPosSelected.x + _lastSelectedRect.rect.xMin;
            rightX = localPosHighlighted.x + currntHighlighted.rect.xMax;
        }
        else
        {
            leftX = localPosSelected.x + _lastSelectedRect.rect.xMax;
            rightX = localPosHighlighted.x + currntHighlighted.rect.xMin;
        }

        newSizeDelta.x = Mathf.Abs(leftX) + Mathf.Abs(rightX);
        newSizeDelta.x += _constWidthAdd;
        newSizeDelta.y = _constHeight;

        // init animation
        rectTransform.DOSizeDelta(newSizeDelta, _animationLength).SetEase(_animationEase);
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
        Select(_lastSelectedRect);
    }
}
