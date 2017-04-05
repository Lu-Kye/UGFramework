using UnityEngine;

namespace UGFramework
{
	public class SingleTonRoot : MonoBehaviour
	{
		static SingleTonRoot _instance;
		public static SingleTonRoot Instance
		{
			get
			{
				if (_instance == null)
				{
					var go = new GameObject("SingleTon");
					_instance = go.AddComponent<SingleTonRoot>();
					GameObject.DontDestroyOnLoad(go);
				}

				return _instance;
			}
		}
	}

	/**
	 * --- DOC BEGIN ---
	 * All singleTon class have to extend MonoBehaviour
	 * so that we can see all singleTon in Hierarchy	
	 * --- DOC END ---
	 */
	public class SingleTon<T> : MonoBehaviour 
		where T : SingleTon<T>
	{
		static T instance = null;
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					GameObject go = new GameObject(typeof(T).Name); 
					go.transform.SetParent(SingleTonRoot.Instance.transform);
					instance = go.AddComponent<T>();	
					instance.Ctor();
				}

				return instance;
			}
		}

		protected virtual void Ctor()
		{
		}

		public virtual void Dispose()
		{
		}
	}
}