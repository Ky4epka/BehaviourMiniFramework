using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GLog :  ILogHandler
{
    public List<ILogHandler> LogHandlers = new List<ILogHandler>();
    protected Logger iHandler = null;
    protected static GLog iInstance = null;

    public static string LogTypeToString(LogType logtype)
    {
        switch (logtype)
        {
            case LogType.Warning:
                return "WARNING";
            case LogType.Log:
                return "LOG";
            case LogType.Assert:
                return "ASSERT";
            case LogType.Error:
                return "ERROR";
            case LogType.Exception:
                return "EXCEPTION";
            default:
                return "";
        }
    }
    
    public static GLog Instance
    {
        get
        {
            if (iInstance == null)
                iInstance = new GLog();

            return iInstance;
        }
    }

    public GLog()
    {
        iHandler = new Logger(this);
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        for (int i=0; i<LogHandlers.Count; i++)
        {
            LogHandlers[i].LogException(exception, context);
        }
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        for (int i = 0; i < LogHandlers.Count; i++)
        {
            LogHandlers[i].LogFormat(logType, context, format, args);
        }
    }

    public static void AddHandler(ILogHandler handler)
    {
        Instance.LogHandlers.Add(handler);
    }

    public static void RemoveHandler(ILogHandler handler)
    {
        Instance.LogHandlers.Remove(handler);
    }

    public static bool IsLogTypeAllowed(LogType logType)
    {
        return Instance.iHandler.IsLogTypeAllowed(logType);
    }

    public static void Log(LogType logType, string tag, object message, UnityEngine.Object context)
    {
        Instance.iHandler.Log(logType, tag, message, context);
    }

    public static void Log(object message)
    {
        Instance.iHandler.Log(message);
    }

    public static void Log(string tag, object message)
    {
        Instance.iHandler.Log(tag, message);
    }
    public static void Log(string tag, object message, UnityEngine.Object context)
    {
        Instance.iHandler.Log(tag, message, context);
    }
    public static void Log(LogType logType, object message, UnityEngine.Object context)
    {
        Instance.iHandler.Log(logType, message, context);
    }
    public static void Log(LogType logType, object message)
    {
        Instance.iHandler.Log(logType, message);
    }
    public static void Log(LogType logType, string tag, object message)
    {
        Instance.iHandler.Log(logType, tag, message);
    }
    public static void LogError(string tag, object message)
    {
        Instance.iHandler.LogError(tag, message);
    }
    public static void LogError(string tag, object message, UnityEngine.Object context)
    {
        Instance.iHandler.LogError(tag, message, context);
    }
    public static void LogException(Exception exception)
    {
        Instance.iHandler.LogException(exception);
    }
    public static void LogFormat(LogType logType, string format, params object[] args)
    {
        Instance.iHandler.LogFormat(logType, format, args);
    }
    public static void LogWarning(string tag, object message, UnityEngine.Object context)
    {
        Instance.iHandler.LogWarning(tag, message, context);
    }
    public static void LogWarning(string tag, object message)
    {
        Instance.iHandler.LogWarning(tag, message);
    }

}
