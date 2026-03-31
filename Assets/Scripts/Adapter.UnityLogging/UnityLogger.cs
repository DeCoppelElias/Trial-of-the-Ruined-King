using UnityEngine;

public class UnityLogger : ILogger
{
    public void LogError(string message)
    {
#if UNITY_EDITOR
        Debug.LogError(message);
#endif
    }

    public void LogInfo(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }

    public void LogWarning(string message)
    {
#if UNITY_EDITOR
        Debug.LogWarning(message);
#endif
    }
}
