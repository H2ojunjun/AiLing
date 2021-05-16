using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugHelper
{
    public static void Log(object o)
    {
#if UNITY_EDITOR
        Debug.Log(o);
#endif
    }

    public static void LogWarning(object o)
    {
#if UNITY_EDITOR
        Debug.LogWarning(o);
#endif
    }

    public static void LogError(object o)
    {
#if UNITY_EDITOR
        Debug.LogError(o);
#endif
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
#if UNITY_EDITOR
        Debug.DrawLine(start,end,color);
#endif
    }
}
