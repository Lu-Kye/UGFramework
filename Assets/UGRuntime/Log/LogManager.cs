using UnityEngine;

public enum LogLevel
{
    DEBUG = 1,
    WARNING,
    ERROR,
    NONE,
}

namespace UGFramework.Log
{
	public static class LogManager
	{
	#if UNITY_EDITOR
		public static LogLevel LogLevel = LogLevel.DEBUG;
	#elif UNITY_DEBUG
		public static LogLevel LogLevel = LogLevel.DEBUG;
	#else
		public static LogLevel LogLevel = LogLevel.ERROR;
	#endif

		static void WriteLog(string msg)
		{
			if (Application.isPlaying == false)
				return;

			LogWriter.Instance.Write(msg);
		}

		public static void Log(string msg)
		{
			if (LogLevel > LogLevel.DEBUG)
				return;
			Debug.Log(msg);
			WriteLog(msg);
		}

		public static void Warning(string msg)
		{
			if (LogLevel > LogLevel.WARNING)
				return;
			Debug.LogWarning(msg);
			WriteLog(msg);
		}

		public static void Error(string msg)
		{
			if (LogLevel > LogLevel.ERROR)
				return;
			Debug.LogError(msg);
			WriteLog(msg);
		}
	}
}
