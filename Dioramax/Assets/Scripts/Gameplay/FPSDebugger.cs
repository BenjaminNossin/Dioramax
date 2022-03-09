using UnityEngine;
using TMPro;  

public class FPSDebugger : MonoBehaviour
{
    [SerializeField] private TMP_Text fpsDebuggerText; 

    void Update()
    {
        fpsDebuggerText.text = $"{(int)(1 / Time.deltaTime)}";
    }
}

