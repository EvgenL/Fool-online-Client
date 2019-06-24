using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fool_online.Scripts.Manager;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Fool_online.Scripts.Chacher
{
    public static class WebChacher
    {
        private const int SecondsToExpire = 300;


        // todo delete unused

        public static void DownloadOrChache(string url, Action<Sprite> onDownloaded)
        {
            string filePath = Application.persistentDataPath;
            filePath += "cache/" + url.GetHashCode();

            bool existsInChache = System.IO.File.Exists(filePath);

            string localPath = "file:///" + filePath;
            if (existsInChache)
            {
                // check if not too old
                System.DateTime written = File.GetLastWriteTimeUtc(filePath);
                System.DateTime now = System.DateTime.UtcNow;
                double lifetime = now.Subtract(written).TotalSeconds;
                if (lifetime > SecondsToExpire)
                {
                    File.Delete(filePath);
                    DialogueManager.Instance.StartCoroutine(Download(url, filePath, onDownloaded));
                }
                else
                {
                    DialogueManager.Instance.StartCoroutine(Download(localPath, filePath, onDownloaded));
                }
            }
            else
            {
                DialogueManager.Instance.StartCoroutine(Download(url, filePath, onDownloaded));
            }
        }

        private static IEnumerator Download(string url, string filePath, Action<Sprite> onDownloaded)
        {
            if (string.IsNullOrEmpty(url)) yield break;

            UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
            req.SendWebRequest();

            while (!req.isDone)
            {
                yield return null;
            }

            // if error while downloading
            if (req.downloadedBytes == 0) yield break;


            Texture2D texture = new Texture2D(1, 1);
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error);
            }
            else
            {
                texture = ((DownloadHandlerTexture)req.downloadHandler).texture;
            }

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            if (onDownloaded != null)
            {
                onDownloaded(sprite);
            }
        }
    }
}
