using System.Collections;
using UGFramework.Log;
using UnityEngine;

namespace UGFramework.Res
{
	public class ResExample : MonoBehaviour 
	{
		void Start()
		{
			ResManager.Instance.Init();
		}

		[ContextMenu("TestLoad&Instantiate")]
		void TestLoad()
		{
			var prefab = ResManager.Instance.Load<GameObject>(ResConfig.UI_PREFABS_ROOT + "/" + "UILogin.prefab");
			GameObject.Instantiate(prefab);
		}

		[ContextMenu("TestHotUpdate(Requirement: OSX,Python)")]
		void TestHotUpdate()
		{
			UGFramework.UGEditor.CommandTool.ExecuteShell("Assets/UGRuntime/Res/ResExample/ResExampleServerStart.sh");
			this.StartCoroutine(_TestHotUpdate());
		}
		IEnumerator _TestHotUpdate()
		{
			yield return new WaitForSeconds(1);

			// Test hotupdate all files
			ResVersionFile.DebugDiffInfos = true;

			ResManager.Instance.OnHotUpdate = (info)=>{
				LogManager.Log(string.Format(
					"OnHotUpdate file({0}) count({1}/{2}) countPercent({3}) size({4:#0.00}mb/{5:#0.00}mb) error({6})",
					info.File,
					info.Index+1, info.Count,
					info.Percent,
					info.DownloadedSize/(1024*1024f), info.DownloadedMaxSize/(1024*1024f),
					info.Error
				));
			};

			ResManager.Instance.TryHotUpdate((count, size)=>{
				LogManager.Log(string.Format(
					"HotUpdate count({0}) size({1})",
					count,
					size
				));
				return true;
			});
		}
	}
}