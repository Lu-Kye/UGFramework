using System;
using System.Collections;
using UGFramework.Log;
using UGFramework.Utility;
using UnityEngine;

namespace UGFramework.Res
{
    public partial class ResManager
    {
        // Only download txt files
        string DownloadTxt(string path)
        {
            string text = null;
            var url = ResConfig.SERVER_URL + "/" + path + "?" + TimeUtility.Timestamp;
            using (var www = _wwwSyncAgent.LoadAsset(url))
            {
                if (www == null)
                {
                    LogManager.Log(string.Format("Download asset failure url({0})", url));
                    return null;
                }
                LogManager.Log(string.Format("Download asset successfully url({0})", url));
                text = www.text;
            }
            return text;
        }

        IEnumerator DownloadServerAssetAsync(string path, Action<WWW> callback)
        {
            var url = ResConfig.SERVER_URL + "/" + path;
            yield return this.StartCoroutine(this.DownloadAssetAsync(url, callback));
        }
        IEnumerator DownloadAssetAsync(string url, Action<WWW> callback)
        {
            using (var www = new WWW(url))
            {
                yield return www;
                callback(www);
            }
        }
    }
}