using System.Collections;
using UnityEngine;

namespace UGFramework.Res
{
	public class ResExample : MonoBehaviour 
	{
		void Start()
		{
			ResManager.Instance.Init();
		}

		[ContextMenu("Test")]
		void Test()
		{
			var prefab = ResManager.Instance.Load<GameObject>(ResConfig.UI_PREFABS_ROOT + "/" + "UILogin.prefab");
			GameObject.Instantiate(prefab);
		}
	}
}