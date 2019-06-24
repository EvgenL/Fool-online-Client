using System.Collections;
using System.Collections.Generic;
using System.IO;
using Fool_online.Scripts.FoolNetworkScripts;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FileSelector : MonoBehaviour, IPointerDownHandler
{
    public bool isFileSelected = false;

    public int MinimumImageH = 64;
    public int MinimumImageW = 64;
    public float MaximumImageSizeMb = 2.0f;

    [SerializeField] private Text _filenameText;
    [SerializeField] private Image _fileImage;


    [SerializeField] private Button _sendButton;
    [SerializeField] private bool _sendAuto;

    private Sprite _defaultSprite;
    private byte[] _selectedSpriteBytes;


    private void Awake()
    {
        if (_sendButton != null)
        {
            _sendButton.interactable = false;
        }
        _defaultSprite = _fileImage.sprite;
    }

    public void OnClick()
    {

        // todo for modile select from gallery
        OpenFile();
    }

    private void OpenFile()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval(
            @"
document.addEventListener('click', function() {

    var fileuploader = document.getElementById('fileuploader');
    if (!fileuploader) {
        fileuploader = document.createElement('input');
        fileuploader.setAttribute('style','display:none;');
        fileuploader.setAttribute('type', 'file');
        fileuploader.setAttribute('id', 'fileuploader');
        fileuploader.setAttribute('class', 'focused');
        document.getElementsByTagName('body')[0].appendChild(fileuploader);

        fileuploader.onchange = function(e) {
        var files = e.target.files;
            for (var i = 0, f; f = files[i]; i++) {
                window.alert(URL.createObjectURL(f));
                SendMessage('" + gameObject.name + @"', 'FileDialogResult', URL.createObjectURL(f));
            }
        };
    }
    if (fileuploader.getAttribute('class') == 'focused') {
        fileuploader.setAttribute('class', '');
        fileuploader.click();
    }
});
            ");
#endif

#if UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel("Open file", "", "");

        ChangeAvatar(path);
#endif
    }

    private void ChangeAvatar(string path)
    {
        Debug.Log("Changing avatar to " + path);
        if (path == "") return;
        Texture2D texture = new Texture2D(0, 0);
        var fileContent = File.ReadAllBytes(path);
        texture.LoadImage(fileContent);

        bool imageResOk = texture.height >= MinimumImageH
                          && texture.width >= MinimumImageW;
        bool imageSizeOk = fileContent.Length <= (MaximumImageSizeMb * 1048576); // byte to Mb.

        // if file was loaded wrong
        if (path.Length == 0 || !imageSizeOk || !imageResOk)
        {
            if (!imageResOk)
            {
                DialogueManager.Instance?.ShowOk("Этот файл не подходит.");
            }
            else if (!imageSizeOk)
            {
                DialogueManager.Instance?.ShowOk("Изображение не должно быть больше 2мб.");
            }

            ResetFile();
            return;
        }


        isFileSelected = true;
        _selectedSpriteBytes = fileContent;

        if (_sendButton != null)
            _sendButton.interactable = true;

        if (_filenameText != null)
        {
            _filenameText.text = path.Substring(path.LastIndexOf('/') + 1);
        }

        if (_fileImage != null)
        {
            // creating Unity Sprite from texture
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));

            _fileImage.sprite = sprite;
        }

        if (_sendAuto)
        {
            OnSendClick();
        }
    }

    private void ResetFile()
    {
        isFileSelected = false;
        if (_sendButton != null)
            _sendButton.interactable = false;
        if (_filenameText != null)
            _filenameText.text = "Файл не выбран...";
        _fileImage.sprite = _defaultSprite;
    }


    public void OnSendClick()
    {
        ClientSendPackets.Send_UploadAvatar(_selectedSpriteBytes);

    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Application.ExternalEval(
            @"
var fileuploader = document.getElementById('fileuploader');
if (fileuploader) {
    fileuploader.setAttribute('class', 'focused');
}
            ");
    }

    public void FileDialogResult(string fileUrl)
    {
        Debug.Log("FileDialogResult: " + fileUrl);
        //text.text = fileUrl;
        StartCoroutine(PreviewCoroutine(fileUrl));
    }

    IEnumerator PreviewCoroutine(string url)
    {
        var www = new WWW(url);
        yield return www;
        print("PreviewCoroutine: " + www.texture);
        print("texture.width: " + www.texture.width + ", texture.height:" + www.texture.height);

        // creating Unity Sprite from texture
        Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height),
            new Vector2(0.5f, 0.5f));

        _fileImage.sprite = sprite;
        _fileImage.sprite = null;
    }
}
