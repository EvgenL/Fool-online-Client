using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarDownloader : MonoBehaviour
{
    [Header("Download picture and put in this UI image")]
    [SerializeField] private Image _targetImage;

    [SerializeField] private bool _downloalOnAwake = false;
    [SerializeField] private string _awakeUrl;

    void Awake()
    {
        if (_downloalOnAwake)
        {
            DownloadPic(_awakeUrl);
        }   
    }


    public void DownloadPic(string url)
    {
        StartCoroutine(DownloadCoroutine(url));
    }


    private IEnumerator DownloadCoroutine(string url)
    {
        Texture2D texture = new Texture2D(1, 1);
        WWW www = new WWW(url);
        yield return www;

        // if error while downloading
        if (www.bytesDownloaded == 0) yield break;

        www.LoadImageIntoTexture(texture);
        Sprite image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        _targetImage.sprite = image;
    }
}
