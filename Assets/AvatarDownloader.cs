using System.Collections;
using System.Collections.Generic;
using System.IO;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;
using UnityEngine.UI;

public class AvatarDownloader : MonoBehaviourFoolObserver
{
    public long AvaratHolderConnectionId;
    public bool UseMyconnectionId = false;

    [Header("Download avatar and put in this UI image")]
    [SerializeField] private Image _targetImage;


    public override void OnUpdateUserAvatar(long avatarHolder, string avatarPath)
    {

        if (avatarHolder == AvaratHolderConnectionId
            || UseMyconnectionId && avatarHolder == FoolNetwork.LocalPlayer.ConnectionId)
        {
            StartCoroutine(DownloadCoroutine(avatarPath));
        }
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
