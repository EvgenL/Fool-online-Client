using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TestImageDownloader : MonoBehaviour
{
    [Header("Download picture and put in this UI image")]
    [SerializeField] private Image _targetImage;

    [SerializeField] private bool _downloalOnAwake = false;
    [SerializeField] private string _awakeUrl;

    void Start()
    {
        if (_downloalOnAwake)
        {
            DownloadPic(_awakeUrl);
        }   
    }


    public void DownloadPic(string url)
    {
        //StartCoroutine(DownloadCoroutineWWW(url));
        StartCoroutine(DownloadCoroutineUWRQ(url));
    }


    private IEnumerator DownloadCoroutineWWW(string url)
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

    private IEnumerator DownloadCoroutineUWRQ(string url)
    {
        UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
        yield return req.SendWebRequest();

        // if error while downloading
        if (req.isNetworkError || req.isHttpError)
        {
            Debug.LogWarning("Error downloading avatar: " + req.error);
            yield break;
        }

        // download texture
        Texture2D texture = ((DownloadHandlerTexture)req.downloadHandler).texture;

        // apply ro sprite
        Sprite image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        _targetImage.sprite = image;
    }
}
