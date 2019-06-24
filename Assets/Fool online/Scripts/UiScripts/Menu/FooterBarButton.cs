using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FooterBarButton : MonoBehaviour
{

    [Header("Link to animated footer marker")]
    [SerializeField] private TabMarkerAnimation _marker;

    [Header("File image of back button")]
    [SerializeField] private Sprite _backButtonSpriteFile;
    private Sprite _defaultSprite;
    private Image _image;
    private Text _text;
    private TouchscreenButton _touchscreenButton;

    public RectTransform ButtonBounds => _touchscreenButton.transform as RectTransform;

    private void Awake()
    {
        // find marker if not assigned
        if (_marker == null)
        {
            _marker = GetComponentInParent<TabMarkerAnimation>();
        }

        if (_marker == null)
        {
            Debug.LogWarning("Не удаётся найти TabMarkerAnimation в паренте");
        }



        // bufer ref to child components
        _image = GetComponent<Image>();
        _text = GetComponentInChildren<Text>();
        _touchscreenButton = GetComponentInChildren<TouchscreenButton>();

        // remember default sprite
        _defaultSprite = _image.sprite;
    }

    #region Animation 

    public void PointerDown()
    {
         _marker.Highlight(this);
    }

    public void PointerUp()
    {
         _marker.Select(this);
    }

    public void Deselect()
    {
        _marker.StopHighlight();
    }

    public void ShowBackSprite()
    {
        _image.sprite = _backButtonSpriteFile;
    }

    public void ShowNormalSprite()
    {
        _image.sprite = _defaultSprite;
    }

    #endregion

    #region Interaction

    public void ChangeToBackDisplay()
    {

    }
    public void ChangeToNormalDisplay()
    {

    }

    #endregion

}
