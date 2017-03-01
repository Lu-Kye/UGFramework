using UnityEngine;

namespace UGFramework
{
	/**
	 * --- DOC BEGIN ---
	 * All singleTon class have to extend MonoBehaviour
	 * so that we can see all singleTon in Hierarchy	
	 * --- DOC END ---
	 */
	public class SingleTon<T> : MonoBehaviour 
		where T : SingleTon<T>
	{
		static GameObject singleTonRoot = null;

		static T instance = null;
		public static T Instance
		{
			get
			{
				if (singleTonRoot == null)
				{
					singleTonRoot = new GameObject("SingleTon");
					GameObject.DontDestroyOnLoad(singleTonRoot);
				}

				if (instance == null)
				{
					GameObject go = new GameObject(typeof(T).Name); 
					go.transform.SetParent(singleTonRoot.transform);
					instance = go.AddComponent<T>();	
				}

				return instance;
			}
		}
	}
}