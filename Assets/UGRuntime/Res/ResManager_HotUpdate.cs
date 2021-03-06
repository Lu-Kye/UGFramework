using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UGFramework.Utility;
using UGFramework.Log;

namespace UGFramework.Res
{
    public partial class ResManager
    {
        public bool EnableHotUpdate = true;

        void InitForHotUpdate()
        {
        }

        [HideInInspector]
        public Action<ProcessInfo> OnHotUpdate = delegate {};
        void NotifyHotUpdate(ProcessInfo processInfo)
        {
            if (this.OnHotUpdate == null)
                return;
            this.OnHotUpdate(processInfo);
        }
        void NotifyNoHotUpdate()
        {
            var processInfo = new ProcessInfo();
            processInfo.File = "";
            processInfo.Error = "";
            processInfo.Index = -1;
            processInfo.Count = 0;
            this.NotifyHotUpdate(processInfo);
        }

        public delegate bool ConfirmCallbackDef(int fileCount, ulong fileSizeSum);
        ConfirmCallbackDef _confirmCallback;
        Coroutine _hotupdatingCoroutine = null;
        Coroutine _hotupdatingDownloadCoroutine = null;
        /**
         * Call HotUpdate when game starting 
         */
        public void TryHotUpdate(ConfirmCallbackDef confirmCallback)
        {
            if (this.EnableHotUpdate == false)
            {
                this.NotifyNoHotUpdate();
                return;
            }

            if (Application.isMobilePlatform == false && this.Simulate == false)
            {
                this.NotifyNoHotUpdate();
                return;
            }

    #if UNITY_DEBUG
            if (Directory.Exists(ResConfig.MOBILE_HOTUPDATE_PATH))
            {
                LogManager.Log(string.Format("HotUpdate: Delete MOBILE_HOTUPDATE_PATH({0})", ResConfig.MOBILE_HOTUPDATE_PATH));
                Directory.Delete(ResConfig.MOBILE_HOTUPDATE_PATH, true);
            }
    #endif

            // Start hotupdating
            this.StopHotUpdate();
            _confirmCallback = confirmCallback;
            _hotupdatingCoroutine = this.StartCoroutine(this.HotUpdating());
        }

        public void StopHotUpdate()
        {
            if (_hotupdatingCoroutine == null)
                return;
            this.StopDownload();
            this.StopCoroutine(_hotupdatingCoroutine);
            _hotupdatingCoroutine = null;

            if (_hotupdatingDownloadCoroutine != null)
            {
                this.StopCoroutine(_hotupdatingDownloadCoroutine);
                _hotupdatingDownloadCoroutine = null;
            }
        }

        IEnumerator HotUpdating()
        {
            // Get local version file at first
            var localVersionFile = GetLocalVersionFile(); 

            // Get remote version file and compare
            // Download remote version file and save to temporary directory
            var file = ResConfig.ConvertToBundleName(ResConfig.VERSION_FILE) + ResConfig.BUNDLE_EXTENSION;
            var fileFolder = "resversionfile_tempfolder";
            var fileDirectory = ResConfig.MOBILE_HOTUPDATE_PATH + "/" + fileFolder;
            var failure = false;
            byte[] downloadBytes = null;
            yield return this.StartCoroutine(this.DownloadAssetsAsyncAndSave(
                ResConfig.PLATFORM_SERVER_URL + "/",
                new List<string>() { "param=" + TimeUtility.Timestamp },
                new List<string>() { file }, 
                (processInfo, www) => {
                    if (string.IsNullOrEmpty(processInfo.Error) == false)
                    {
                        failure = true;
                        processInfo.Index = -1;
                        processInfo.Count = 0;
                        this.NotifyHotUpdate(processInfo);
                    }
                    else
                    {
                        var length = www.bytesDownloaded;
                        // var length = www.bytes.Length;
                        downloadBytes = new byte[length];
                        Array.Copy(www.bytes, 0, downloadBytes, 0, length);
                    }
                },
                fileDirectory
            ));
            if (failure)
            {
                yield break;    
            }
            var txtAsset = this.Load<TextAsset>(fileFolder + "/" + ResConfig.VERSION_FILE, ResConfig.VERSION_FILE);
            if (txtAsset == null)
            {
                var processInfo = new ProcessInfo();
                processInfo.File = ResConfig.VERSION_FILE;
                processInfo.Index = -1;
                processInfo.Count = 0;
                processInfo.Error = "Download version file error";
                this.NotifyHotUpdate(processInfo);
                yield break;
            }
            var txt = txtAsset.text;
            var remoteVersionFile = ResVersionFile.UnSerialize(txt);
            // Unload asset bundle
            this.UnloadAsset(fileFolder + "/" + ResConfig.VERSION_FILE);

            // Hotupdating(download and save)
            var diffInfos = ResVersionFile.GetDiffInfos(localVersionFile, remoteVersionFile);
            if (diffInfos.Count <= 0)
            {
                this.NotifyNoHotUpdate();
                yield break;
            }

            var assets = new List<string>();
            var assetSizes = new Dictionary<string, ulong>();
            var assetUrlParams = new List<string>();
            var assetsSumSize = (ulong)0;
            for (int i = 0; i < diffInfos.Count; ++i)
            {
                var asset = diffInfos[i].File;

                // Exclude resources_version
                if (asset == (ResConfig.ConvertToBundleName(ResConfig.VERSION_FILE)+ResConfig.BUNDLE_EXTENSION))
                    continue;

                assets.Add(asset);
                assetUrlParams.Add("md5=" + diffInfos[i].MD5);
                assetSizes[asset] = diffInfos[i].Size;
                assetsSumSize += diffInfos[i].Size;
            }

            // Whether continue download and update assets?
            while (_confirmCallback != null && _confirmCallback(assets.Count, assetsSumSize) == false)
            {
                yield return 1;
            }
            _confirmCallback = null;

            // Start downloading and saving assetBundles to local storage system.
            var failed = false;
            var downloadedCount = 0;
            var downloadedSize = (ulong)0;
            var downloadMaxSize = assetsSumSize;
            _hotupdatingDownloadCoroutine = this.StartCoroutine(this.DownloadAssetsAsyncAndSave(
                ResConfig.PLATFORM_SERVER_URL + "/",
                assetUrlParams,
                assets, 
                (processInfo, www) => {
                    if (string.IsNullOrEmpty(processInfo.Error) == false) 
                    {
                        failed = true;
                        this.StopHotUpdate();
                    }
                    downloadedCount++;
                    processInfo.Index = downloadedCount - 1;
                    processInfo.Count = assets.Count;

                    var currentDownloadedSize = (ulong)www.bytesDownloaded;
                    if (assetSizes.ContainsKey(processInfo.File)) currentDownloadedSize = assetSizes[processInfo.File];

                    processInfo.DownloadedSize = downloadedSize = downloadedSize + currentDownloadedSize;
                    processInfo.DownloadedMaxSize = downloadMaxSize;
                    this.NotifyHotUpdate(processInfo);
                }
            ));
            yield return _hotupdatingDownloadCoroutine;

            // Finally & Successfully, then replace local version file
            if (failed == false && downloadedSize >= downloadMaxSize)
                FileUtility.WriteFile(ResConfig.MOBILE_HOTUPDATE_PATH + "/" + file, downloadBytes, downloadBytes.Length, false);
        }
    }
}