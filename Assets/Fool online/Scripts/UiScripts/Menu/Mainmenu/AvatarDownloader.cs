using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Fool_online.Scripts.Chacher;
using Fool_online.Scripts.FoolNetworkScripts;
using Fool_online.Scripts.FoolNetworkScripts.NetworksObserver;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AvatarDownloader : MonoBehaviourFoolObserver
{
    public long AvatarHolderConnectionId;

    public bool DownloadMyAvatarOnConnect = false;

    [Header("Download avatar and put in this UI image")]
    [SerializeField] private Image _targetImage;

    private Sprite _defaultSprite;

    private void Awake()
    {
        _defaultSprite = _targetImage.sprite;
    }

    public void ResetImage()
    {
        _targetImage.sprite = _defaultSprite;
    }

    /// <summary>
    /// observer callback used to download my avatar if wasn't
    /// </summary>
    public override void OnUpdateUserData(long connectionId, long userId, string nickname, double money, string avatarFile)
    {
        if (DownloadMyAvatarOnConnect)
        {
            AvatarHolderConnectionId = FoolNetwork.LocalPlayer.ConnectionId;

            // todo not download my avatar but find it on disc
            WebChacher.DownloadOrChcahe(FoolNetwork.LocalPlayer.AvatarFile, SetAvatar);
        }
    }

    public new void OnEnable()
    {
        base.OnEnable();

        if (DownloadMyAvatarOnConnect && !string.IsNullOrEmpty(FoolNetwork.LocalPlayer.AvatarFile))
        {
            WebChacher.DownloadOrChcahe(FoolNetwork.LocalPlayer.AvatarFile, SetAvatar);
            //StartCoroutine(DownloadCoroutine(FoolNetwork.LocalPlayer.AvatarFile));
        }
    }


    public override void OnUpdateUserAvatar(long avatarHolder, string avatarPath)
    {
        if (avatarHolder == AvatarHolderConnectionId)
        {
            WebChacher.DownloadOrChcahe(avatarPath, SetAvatar);
            //StartCoroutine(DownloadCoroutine(avatarPath));
        }
    }

    private void SetAvatar(Sprite sprite)
    {
        if (_targetImage != null)
            _targetImage.sprite = sprite;
    }
}
