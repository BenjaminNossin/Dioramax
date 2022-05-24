using UnityEngine;

public static class GameDrawDebugger  
{
    public static void DrawRay(Vector3 start, Vector3 direction, Color color, float duration = 0.25f)
    {
        #if UNITY_EDITOR
        Debug.DrawRay(start, direction, color, duration);
        #endif
    }
}
