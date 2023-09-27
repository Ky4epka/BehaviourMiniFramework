using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class FileLogger : MonoBehaviour, ILogHandler
{
    public const string LogsFileDir = "logs\\sessions\\";
    public const string LogFilePostfix = "-session.log";
    public const int FileBufferSize = 1048576;
    public const int LineMaxLen = 1024;

    protected string iOutFileName = "";
    protected StreamWriter iFileWriter = null;
    protected StringBuilder iLineBuffer = new StringBuilder(LineMaxLen);

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        LogFormat(LogType.Exception, context, "{0}", new object[1] { exception.ToString()});
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        iLineBuffer.Clear();
        iLineBuffer.Append("[");
        iLineBuffer.Append(Main.Managers.SessionManager.SessionStartTime);
        iLineBuffer.Append(" ");
        iLineBuffer.Append(GLog.LogTypeToString(logType));
        iLineBuffer.Append("] ");
        iLineBuffer.AppendFormat(format, args);

        if (context != null)
        {
            iLineBuffer.Append(" (");
            iLineBuffer.Append(context.GetInstanceID());
            iLineBuffer.Append(" ");
            iLineBuffer.Append(context.name);
            iLineBuffer.Append(" ");
            iLineBuffer.Append(context.GetType().FullName);
            iLineBuffer.Append(")");
        }

        AddLine(iLineBuffer.ToString());
    }

    protected void AddLine(string line)
    {
        iFileWriter.WriteLine(line);

        if (iFileWriter.BaseStream.Length + line.Length > FileBufferSize)
            DropToFile();
    }

    protected void DropToFile()
    {
        iFileWriter.Flush();
    }

    private void Awake()
    {
        if (!Directory.Exists(LogsFileDir))
            Directory.CreateDirectory(LogsFileDir);

        iOutFileName = LogsFileDir + Main.Managers.SessionManager.SessionStartTime.ToString("dd-MM-yy HH.mm.ss") + LogFilePostfix;
        iFileWriter = new StreamWriter(iOutFileName, true, System.Text.Encoding.UTF8, FileBufferSize);
        GLog.AddHandler(this);
    }

    private void OnDestroy()
    {
        DropToFile();
        iFileWriter.Close();
        iFileWriter.Dispose();
        GLog.RemoveHandler(this);
    }
}
