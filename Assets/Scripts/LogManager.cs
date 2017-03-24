using UnityEngine;

namespace UGFramework
{
	public enum LogLevel
	{
		DEBUG,
		WARNING,
		ERROR,
		NONE,
	}

	/**
	 * --- DOC BEGIN ---
	 * --- DOC END ---
	 */ 
	public class LogManager : SingleTon<LogManager> 
	{
#if UNITY_EDITOR
		public LogLevel LogLevel = LogLevel.DEBUG;
#else
		public LogLevel LogLevel = LogLevel.ERROR;
#endif

		public void Log(string msg)
		{
			if (this.LogLevel > LogLevel.DEBUG)
				return;
			Debug.Log(msg);
		}

		public void Warning(string msg)
		{
			if (this.LogLevel > LogLevel.WARNING)
				return;
			Debug.Log(msg);
		}

		public void Error(string msg)
		{
			if (this.LogLevel > LogLevel.ERROR)
				return;
			Debug.Log(msg);
		}
	}
}
