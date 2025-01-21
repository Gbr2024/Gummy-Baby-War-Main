using UnityEngine;
using System;
using System.IO;

// Place this script in a Scripts/Server folder
public static class ServerLogger
{
    private static readonly string logPath = "server_log.txt";
    private static readonly object lockObject = new object();

    public static void Log(string message)
    {
        if (!ServerManager.Instance.IsServer) return;

        string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logMessage = $"[{timeStamp}] {message}\n";

        lock (lockObject)
        {
            try
            {
                File.AppendAllText(logPath, logMessage);
                Debug.Log($"[SERVER] {message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write to log file: {e.Message}");
            }
        }
    }

    public static void LogWarning(string message)
    {
        Log($"WARNING: {message}");
        Debug.LogWarning($"[SERVER] {message}");
    }

    public static void LogError(string message)
    {
        Log($"ERROR: {message}");
        Debug.LogError($"[SERVER] {message}");
    }
}