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
			Debug.Log("WaitForSeconds " + Time.realtimeSinceStartup);
			yield return new WaitSeconds(2.0f);
			Debug.Log("WaitForSeconds " + Time.realtimeSinceStartup);

			Debug.Log("Wait Frames0 " + Time.frameCount);
			yield return new WaitFrames(0);
			Debug.Log("Wait Frames0 " + Time.frameCount);

			Debug.Log("Wait Frames1 " + Time.frameCount);
			yield return new WaitFrames(1);
			Debug.Log("Wait Frames1 " + Time.frameCount);

			Debug.Log("Wait Frames2 " + Time.frameCount);
			yield return new WaitFrames(2);
			Debug.Log("Wait Frames2 " + Time.frameCount);

			string url = "http://images.earthcam.com/ec_metros/ourcams/fridays.jpg";
			Debug.Log("Wait WWW " + Time.realtimeSinceStartup);
			yield return new WaitWWW(new WWW(url));
			Debug.Log("Wait WWW " + Time.realtimeSinceStartup);

			Debug.Log("Break " + Time.realtimeSinceStartup);
			yield break;
			// Debug.Log("Break" + Time.realtimeSinceStartup);
		}
	}
}