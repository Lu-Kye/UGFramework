using UnityEngine;

namespace UGFramework
{
	/**
	 * --- DOC BEGIN ---
	 * --- DOC END ---
	 */ 
	public class LogManager : SingleTon<LogManager> 
	{
		public void Debug(string format, params object[] args)
		{
			UnityEngine.Debug.Log(string.Format(format, args));
		}
	}
}
