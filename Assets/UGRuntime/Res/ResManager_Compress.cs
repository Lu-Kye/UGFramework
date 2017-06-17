using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UGFramework.Log;
using UGFramework.Utility;
using UnityEngine;

namespace UGFramework.Res
{
    public partial class ResManager
    {
        public bool EnableUncompress = false;

        void InitForCompress()
        {
        }

        public Action<ProcessInfo> OnUncompress = delegate {};
        void NotifyUncompress(ProcessInfo processInfo)
        {
            if (this.OnUncompress == null)
                return;
            this.OnUncompress(processInfo);

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
        public void TryUncompress()
        {
            if (this.IsUnCompressed || this.EnableUncompress == false)
            {
                this.NotifyNoUncompress();
                return;
            }

            _localVersionFile = _GetLocalVersionFile(); 
            this.StartCoroutine(this.UncompressAndSave(_localVersionFile.Infos));
        }

        IEnumerator UncompressAndSave(ResVersionInfo[] fileInfos)
        {
            for (int i = 0; i < fileInfos.Length; ++i)
            {
                var fileInfo = fileInfos[i];
                var path = fileInfo.File;
                var fullpath = ResConfig.BUILDIN_PATH + "/" + fileInfo.File;
                var url = Application.platform == RuntimePlatform.Android ? fullpath : "file://" + fullpath;
                yield return this.StartCoroutine(this.DownloadAssetAsync(url, (www) => {
                    var processInfo = new ProcessInfo();
                    processInfo.File = fileInfo.File;
                    processInfo.Index = i;
                    processInfo.Count = fileInfos.Length;

                    if (string.IsNullOrEmpty(www.error) == false)
                    {
                        processInfo.Error = "[UnCompress] Load file failure " + www.error;
                        this.NotifyUncompress(processInfo);
                        return;
                    }

                    var outputFullpath = ResConfig.MOBILE_HOTUPDATE_PATH + "/" + path; 
                    FileUtility.WriteFile(outputFullpath, www.bytes);
                    if (File.Exists(outputFullpath))
                    {
                        LogManager.Log(string.Format("[UnCompress] Save successfully path({0})", outputFullpath));
                        processInfo.Error = string.Empty;
                    }
                    else
                    {
                        LogManager.Error(string.Format("[UnCompress] Save failure path({0})", outputFullpath));
                        processInfo.Error = "Save file failure";
                    }

                    this.NotifyUncompress(processInfo);
                }));
            }
        }
    }
}