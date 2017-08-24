using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UGFramework.Utility;

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

        Coroutine _hotupdatingCoroutine = null;
        /**
         * Call HotUpdate when game starting 
         */
        public void TryHotUpdate()
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
            _hotupdatingCoroutine = this.StartCoroutine(this.HotUpdating());
        }

        public void StopHotUpdate()
        {
            if (_hotupdatingCoroutine == null)
                return;
            this.StopDownload();
            this.StopCoroutine(_hotupdatingCoroutine);
            _hotupdatingCoroutine = null;
        }

        IEnumerator HotUpdating()
        {
            // Get local version file at first
            var localVersionFile = GetLocalVersionFile(); 

            // Get remote version file and compare
            // Download remote version file and save to temporary directory
            var file = ResConfig.ConvertToBundleName(ResConfig.VERSION_FILE) + ResConfig.BUNDLE_EXTENSION;
            var assets = new List<string>();
            assets.Add(file);
            var fileFolder = "resversionfile_tempfolder";
            var fileDirectory = ResConfig.MOBILE_HOTUPDATE_PATH + "/" + fileFolder;
            var failure = false;
            byte[] downloadBytes = null;
            yield return this.StartCoroutine(this.DownloadAssetsAsyncAndSave(
                ResConfig.SERVER_URL + "/",
                assets, 
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
            var txtAsset = this.Load<TextAsset>(fileFolder + "/" + ResConfig.VERSION_FILE);
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
            assets.Clear();
            for (int i = 0; i < diffInfos.Count; ++i)
            {
                var asset = diffInfos[i].File;
                assets.Add(asset);
            }
            var count = assets.Count;
            var downloadedCount = 0;
            yield return this.StartCoroutine(this.DownloadAssetsAsyncAndSave(
                ResConfig.SERVER_URL + "/",
                assets, 
                (processInfo, www) => {
                    downloadedCount++;
                    processInfo.Index = downloadedCount - 1;
                    processInfo.Count = assets.Count;
                    this.NotifyHotUpdate(processInfo);
                }
            ));

            // Finally replace local version file
            FileUtility.WriteFile(ResConfig.MOBILE_HOTUPDATE_PATH + "/" + file, downloadBytes, downloadBytes.Length, false);
        }

        [ContextMenu("TestHotUpdate")]
        void TestHotUpdate()
        {
            this.TryHotUpdate();
        }
    }
}