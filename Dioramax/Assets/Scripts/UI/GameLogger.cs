using UnityEngine;

public static class GameLogger
{
    public static void Log(string message)
    {
        #if UNITY_EDITOR
        Debug.Log(message);
        #endif
    }
}
