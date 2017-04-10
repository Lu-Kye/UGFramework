using System.Collections;
using UnityEngine;

namespace UGFramework.Coroutine
{
	public class CoroutineExample : MonoBehaviour 
	{
		void Start()
		{
			CoroutineManager.Instance.Run(this.TestCoroutine());
			CoroutineManager.Instance.Run(this.TestCoroutine());
		}

		IEnumerator TestCoroutine()
		{
			Debug.Log("TestCoroutine Wait Frames1 " + Time.frameCount);
			yield return new WaitFrames(1);
			Debug.Log("TestCoroutine Wait Frames1 " + Time.frameCount);

			Debug.Log("TestCoroutine WaitForSeconds " + Time.realtimeSinceStartup);
			yield return new WaitSeconds(2.0f);
			Debug.Log("TestCoroutine WaitForSeconds " + Time.realtimeSinceStartup);

			Debug.Log("TestCoroutine Wait Frames1 " + Time.frameCount);
			yield return new WaitFrames(1);
			Debug.Log("TestCoroutine Wait Frames1 " + Time.frameCount);

			Debug.Log("TestCoroutine Wait Frames2 " + Time.frameCount);
			yield return new WaitFrames(2);
			Debug.Log("TestCoroutine Wait Frames2 " + Time.frameCount);

			// string url = "http://images.earthcam.com/ec_metros/ourcams/fridays.jpg";
			// Debug.Log("TestCoroutine Wait WWW " + Time.realtimeSinceStartup);
			// yield return new WaitWWW(new WWW(url));
			// Debug.Log("TestCoroutine Wait WWW " + Time.realtimeSinceStartup);


			Debug.Log("TestCoroutine Wait New Coroutine Begin");
			yield return new WaitNewCoroutine(TestNewCoroutine());
			Debug.Log("TestCoroutine Wait New Coroutine End");


			Debug.Log("TestCoroutine Break " + Time.realtimeSinceStartup);
			yield break;
			// Debug.Log("Break" + Time.realtimeSinceStartup);
		}

		IEnumerator TestNewCoroutine()
		{
			Debug.Log("TestNewCoroutine WaitForSeconds " + Time.realtimeSinceStartup);
			yield return new WaitSeconds(1);
			Debug.Log("TestNewCoroutine WaitForSeconds " + Time.realtimeSinceStartup);

			Debug.Log("TestNewCoroutine Break " + Time.realtimeSinceStartup);
			yield break;
		}
	}
}