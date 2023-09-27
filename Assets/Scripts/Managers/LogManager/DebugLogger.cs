using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebugLogger : MonoBehaviour, ILogHandler
{
    public void LogException(Exception exception, UnityEngine.Object context)
    {
        Debug.unityLogger.logHandler.LogException(exception, context);
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        Debug.unityLogger.logHandler.LogFormat(logType, context, format, args);
    }

    private void Awake()
    {
        GLog.AddHandler(this);
        DontDestroyOnLoad(this);
    }

    private void OnDestroy()
    {
        GLog.RemoveHandler(this);
    }


}
