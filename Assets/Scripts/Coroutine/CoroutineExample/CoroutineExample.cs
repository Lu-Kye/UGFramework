using System.Collections;
using UnityEngine;

namespace UGFramework.Coroutine
{
	public class CoroutineExample : MonoBehaviour 
	{
		void Start()
		{
			CoroutineManager.Instance.Run(this.TestCoroutine());
		}

		IEnumerator TestCoroutine()
		{
			Debug.Log("WaitForSeconds" + Time.realtimeSinceStartup);
			yield return new WaitForSeconds(2.0f);
			Debug.Log("WaitForSeconds" + Time.realtimeSinceStartup);

			Debug.Log("Return Null" + Time.realtimeSinceStartup);
			yield return null;
			Debug.Log("Return Null" + Time.realtimeSinceStartup);

			Debug.Log("Break" + Time.realtimeSinceStartup);
			yield break;
			// Debug.Log("Break" + Time.realtimeSinceStartup);
		}
	}
}