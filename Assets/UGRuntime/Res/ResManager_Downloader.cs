using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UGFramework.Utility;
using UGFramework.Log;

namespace UGFramework.Res
{
    public partial class ResManager
    {
        public float DownloadTimeout = 5f;
        public int DownloadRetryTimes = 3;
    
        LinkedList<Coroutine> _downloadCoroutines = new LinkedList<Coroutine>();
        LinkedList<Coroutine> _wwwCoroutines = new LinkedList<Coroutine>();
        Dictionary<Coroutine, WWW> _wwws = new Dictionary<Coroutine, WWW>();
    
        public void StopDownload()
        {
            var coroutines = _wwwCoroutines.ToArray();
            for (int i = 0; i < coroutines.Length; ++i)
            {
                var coroutine = coroutines[i];
                this.StopCoroutine(coroutine);
    
                // Dispose www
                if (_wwws.ContainsKey(coroutine))
                {
                    var www = _wwws[coroutine];
                    www.Dispose();
                    _wwws.Remove(coroutine);
                }
            }
    
            coroutines = _downloadCoroutines.ToArray();
            for (int i = 0; i < coroutines.Length; ++i)
            {
                var coroutine = coroutines[i];
                this.StopCoroutine(coroutine);
            }
    
            // Clear 
            _assetsCount = 0;
            _downloadedAssetsCount = 0;
            _wwwCoroutines.Clear();
            _wwws.Clear();
            _downloadCoroutines.Clear();
        }
    
        public IEnumerator DownloadAssetAsync(string url, string urlParams, Action<WWW> callback)
        {
            var coroutine = this.StartCoroutine(_DownloadAssetAsync(url, urlParams, (www) => {
                if (callback != null) callback(www); 
            }));
            _downloadCoroutines.AddLast(coroutine);
            yield return coroutine;
            _downloadCoroutines.Remove(coroutine);
        }
        IEnumerator _DownloadAssetAsync(string url, string urlParams, Action<WWW> callback)
        {
            var tryTimes = 0;
            while (tryTimes <= this.DownloadRetryTimes)
            {
                var time = Time.realtimeSinceStartup;
                var wwwUrl = url + (string.IsNullOrEmpty(urlParams) ? "" : "?" + urlParams);
                // LogManager.Error(string.Format("_DownloadAssetAsync wwwUrl({0})", wwwUrl));

                var www = new WWW(wwwUrl);
                var result = new List<bool>() { false };
                var progress = 0f;
                var coroutine = this.StartCoroutine(_DownloadAssetAsyncWWW(www, result)); 
                _wwwCoroutines.AddLast(coroutine);
                _wwws.Add(coroutine, www);
    
                // Check timeout
                while (result[0] == false)
                {
                    // Progress changed and reset start time
                    var preProgress = progress;
                    progress = www.progress;
                    if (preProgress != progress)
                        time = Time.realtimeSinceStartup;
    
                    if ((Time.realtimeSinceStartup - time) >= this.DownloadTimeout)
                    {
                        // Timeout
                        this.StopCoroutine(coroutine);
                        _wwwCoroutines.Remove(coroutine);
                        _wwws.Remove(coroutine);
                        www.Dispose();
                        tryTimes++;
                        break;
                    }
                    else
                    {
                        yield return 1;
                    }
                }
    
                // Success
                if (result[0] == true)
                {
                    this.StopCoroutine(coroutine);
                    _wwwCoroutines.Remove(coroutine);
                    _wwws.Remove(coroutine);
                    callback(www);
                    www.Dispose();
                    yield break;
                }
            }
    
            // Failure
            callback(null);
            yield break;
        }
        IEnumerator _DownloadAssetAsyncWWW(WWW www, List<bool> result)
        {
            yield return www;
            result[0] = true;
        }
    
        int _assetsCount = 0;
        int _downloadedAssetsCount = 0;
        IEnumerator DownloadAssetsAsyncAndSave(
            string urlPrefix, 
            List<string> urlParams,
            List<string> assets, 
            Action<ProcessInfo, WWW> callback, 
            string outputDirectory = null, 
            int returnStep = 1)
        {
            _assetsCount = assets.Count;
            _downloadedAssetsCount = 0;
            for (int i = 0; i < assets.Count; ++i)
            {
                var path = assets[i];
                var file = Path.GetFileName(path);
                var url = urlPrefix + path;
                var coroutine = this.StartCoroutine(this.DownloadAssetAsync(url, urlParams[i], (www) => {
                    _downloadedAssetsCount++;
    
                    // Define error message
                    var error = "";
                    if (www == null) error = "Timeout";
                    else if (string.IsNullOrEmpty(www.error) == false) error = www.error;
    
                    // Create processInfo
                    var processInfo = new ProcessInfo();
                    processInfo.File = file;
                    processInfo.Index = i;
                    processInfo.Count = assets.Count;
                    if (string.IsNullOrEmpty(error) == false)
                    {
                        processInfo.Error = string.Format("Download failure! asset({0}) error({1}) url({2})", path, error, url);
                        callback(processInfo, www);
                        return;
                    }
    
                    // Compress or just save asset
                    var exception = "";
                    var fullpath = (string.IsNullOrEmpty(outputDirectory) ? ResConfig.MOBILE_HOTUPDATE_PATH : outputDirectory) + "/" + path;
                    try
                    {
                        var decompressable = ResConfig.IsCompressable(fullpath);
                        var decompressFailure = false;
                        if (decompressable)
                        {
                            try
                            {
                                var length = www.bytesDownloaded;
                                // var length = www.bytes.Length;
                                FileUtility.DeCompressBuffer(www.bytes, length, fullpath);
                            }
                            catch (Exception e)
                            {
                                exception = e.Message;
                                decompressFailure = true;
                            }
                        }
    
                        // Dont decompress or decompress failure, then
                        // just copy
                        if (decompressable == false ||
                           (decompressable == true && decompressFailure == true))
                        {
                            var length = www.bytesDownloaded;
                            // var length = www.bytes.Length;
                            FileUtility.WriteFile(fullpath, www.bytes, length, false);
                        }
                    }
                    catch (Exception e)
                    {
                        exception = e.Message;
                    }
                    finally
                    {
                        if (File.Exists(fullpath))
                        {
                            LogManager.Log(string.Format(
                                "Save successfully! asset({0}) fullpath({1})", 
                                path,
                                fullpath
                            ));
                            processInfo.Error = string.Empty;
                        }
                        else
                        {
                            LogManager.Error(string.Format(
                                "Save failure! asset({0}) fullpath({1}) exception({2})", 
                                path,
                                fullpath,
                                exception
                            ));
                            processInfo.Error = string.Format("Save failure! asset({0}) exception({1})", path, exception);
                        }
                    }
    
                    callback(processInfo, www);
                }));
    
                if (i % returnStep == 0)
                    yield return coroutine;
            }
    
            // Check finished or not
            while (_downloadedAssetsCount < _assetsCount)
            {
                yield return 1;
            }
            yield break;
        }
    }
}