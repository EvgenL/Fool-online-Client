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
         _marker.Highlight(_touchscreenButton.transform as RectTransform);
    }

    public void PointerUp()
    {
         _marker.Select(_touchscreenButton.transform as RectTransform);
    }

    public void Deselect()
    {
        _marker.StopHighlight();
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
