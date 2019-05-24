using System.Collections;
using System.Collections.Generic;
using System.IO;
using Fool_online.Scripts.FoolNetworkScripts;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class FileSelector : MonoBehaviour
{
    public bool isFileSelected = false;

    public int MinimumImageH = 64;
    public int MinimumImageW = 64;
    public float MaximumImageSizeMb = 2.0f;

    [SerializeField] private Text _filenameText;
    [SerializeField] private Image _fileImage;

    [SerializeField] private Button _sendButton;

    private Sprite _defaultSprite;
    private byte[] _selectedSpriteBytes;


    private void Awake()
    {
        _sendButton.interactable = false;
        _defaultSprite = _fileImage.sprite;
    }

    public void OnClick()
    {

#if UNITY_EDITOR
        OpenFileEditor();
#endif
    }

#if UNITY_EDITOR
    private void OpenFileEditor()
    {
        Texture2D texture = new Texture2D(0, 0);
        string path = EditorUtility.OpenFilePanel("Open file", "", "");

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
    }
#endif

    private void ResetFile()
    {
        isFileSelected = false;
        _sendButton.interactable = false;
        _filenameText.text = "Файл не выбран...";
        _fileImage.sprite = _defaultSprite;
    }


    public void OnSendClick()
    {
        ClientSendPackets.Send_UpdateAvatar(_selectedSpriteBytes);
    }
}
