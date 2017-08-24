using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UGFramework.Log;
using UGFramework.Utility;

namespace UGFramework.Res
{
    public partial class ResManager
    {
        public bool EnableUncompress = false;
    
        void InitForCompress()
        {
        }
    
        [HideInInspector]
        public Action<ProcessInfo> OnDecompress = delegate {};
        void NotifyUncompress(ProcessInfo processInfo)
        {
            if (this.OnDecompress == null)
                return;
            this.OnDecompress(processInfo);
    
            // Done
            if (this.EnableUncompress == false) return;
            if (Application.isMobilePlatform == false && this.Simulate == false) return;
            if (processInfo.Index == processInfo.Count - 1)
            {
                // Save new versionFile
                FileUtility.WriteFile(ResConfig.MOBILE_HOTUPDATE_PATH + "/" + ResConfig.VERSION_FILE, _localVersionFile.Serialize());
            }
        }
        void NotifyNoUncompress()
        {
            var processInfo = new ProcessInfo();
            processInfo.File = "";
            processInfo.Error = "";
            processInfo.Index = -1;
            processInfo.Count = 0;
            this.NotifyUncompress(processInfo);
        }
    
        bool IsUnCompressed 
        {
            get 
            {
                if (Application.isMobilePlatform == false && this.Simulate == false)
                    return true;
    
                var versionFile = ResConfig.MOBILE_HOTUPDATE_PATH + "/" + ResConfig.VERSION_FILE;
                return File.Exists(versionFile);
            }
        }
    
        ResVersionFile _localVersionFile;
        Coroutine _decompressCoroutine = null;
        public void TryDecompress()
        {
            if (this.IsUnCompressed || this.EnableUncompress == false)
            {
                this.NotifyNoUncompress();
                return;
            }
    
            _localVersionFile = GetLocalVersionFile(); 
            _decompressCoroutine = this.StartCoroutine(this.DecompressAndSave(_localVersionFile.Infos));
        }
    
        public void StopDecompress()
        {
            if (_decompressCoroutine == null)
                return;
            this.StopDownload();
            this.StopCoroutine(_decompressCoroutine);
            _decompressCoroutine = null;
        }
    
        IEnumerator DecompressAndSave(ResVersionInfo[] fileInfos)
        {
            var urlPrefix = Application.platform == RuntimePlatform.Android 
                ? ResConfig.BUILDIN_PATH + "/" 
                : "file://" + ResConfig.BUILDIN_PATH + "/";
            var assets = new List<string>();
            for (int i = 0; i < fileInfos.Length; ++i)
            {
                assets.Add(fileInfos[i].File);
            }
    
            var count = assets.Count;
            var downloadedCount = 0;
            yield return this.StartCoroutine(this.DownloadAssetsAsyncAndSave(
                urlPrefix,
                assets, 
                (processInfo, www) => {
                    downloadedCount++;
                    processInfo.Index = downloadedCount - 1;
                    processInfo.Count = assets.Count;
                    this.NotifyUncompress(processInfo);
                },
                null,
                9 
            ));
        }
    
        [ContextMenu("TestCompress")]
        void TestCompress()
        {
            var time = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    
            var filePath = Application.dataPath + "/StreamingAssets/android/res/art/ui/texture/runtime/temp";
            var sourceFilePath = filePath + ".assetbundle";
            var compressFilePath = filePath + "_compressed.assetbundle";
            var decompressFilePath = filePath + "_decompressed.assetbundle";
    
            LogManager.Error("MD5 before: " + MD5Utility.GetFileMD5(sourceFilePath));
    
            // var filePath = Application.dataPath + "/Res/Lua/Main";
            // var sourceFilePath = filePath + ".lua";
            // var compressFilePath = filePath + "_compressed.lua";
            // var decompressFilePath = filePath + "_decompressed.lua";
            FileUtility.CompressFile(sourceFilePath, compressFilePath);
    
            FileUtility.DeCompressFile(compressFilePath, decompressFilePath);
            // var bytes = File.ReadAllBytes(compressFilePath);
            // FileUtility.DeCompressBuffer(bytes, decompressFilePath);
    
            LogManager.Error("MD5 after: " + MD5Utility.GetFileMD5(decompressFilePath));
    
            var elapsedTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - time;
            LogManager.Error(string.Format("ElapsedTime: {0}(ms)", elapsedTime));
        }
    }
}