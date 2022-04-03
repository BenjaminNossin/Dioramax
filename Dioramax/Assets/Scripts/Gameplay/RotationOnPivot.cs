using UnityEngine;

public class RotationOnPivot : MonoBehaviour
{
    public bool IsRotatable { get; set; }

    private void Awake()
    {
        IsRotatable = false; 
    }

    private void Update()
    {
        if (IsRotatable)
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, DioravityCameraRotation.ZRotation);           
        }
    } 
}
