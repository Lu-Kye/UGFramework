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
#if UNITY_EDITOR
        	return;
#else
        	LogWriter.Instance.Write(msg);
#endif
    	}

	    public static void Log(string msg)
	    {
	        if (LogLevel > LogLevel.DEBUG)
	            return;
	        Debug.Log(msg);
	        WriteLog(msg);
	    }
	
	    public static void Log(string msg, Object context)
	    {
	        if (LogLevel > LogLevel.DEBUG)
	            return;
	        msg = "[" + context.GetType().Name + "] " + msg;
	        Log(msg);
	    }
	
	    public static void Warning(string msg)
	    {
	        if (LogLevel > LogLevel.WARNING)
	            return;
	        Debug.LogWarning(msg);
	        WriteLog(msg);
	    }
	
	    public static void Warning(string msg, object context)
	    {
	        if (LogLevel > LogLevel.WARNING)
	            return;
	        msg = "[" + context.GetType().Name + "] " + msg;
	        Warning(msg);
	    }
	
	    public static void Error(string msg)
	    {
	        if (LogLevel > LogLevel.ERROR)
	            return;
	        Debug.LogError(msg);
	        WriteLog(msg);
	    }
	
	    public static void Error(string msg, object context)
	    {
	        if (LogLevel > LogLevel.ERROR)
	            return;
	        msg = "[" + context.GetType().Name + "] " + msg;
	        Error(msg);
	    }
	}
}
