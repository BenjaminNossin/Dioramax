using UnityEngine;

public class DebugOnly : MonoBehaviour
{
    void Start()
    {
        if (!Debug.isDebugBuild)
        {
            gameObject.SetActive(false); 
        }
    }
}
