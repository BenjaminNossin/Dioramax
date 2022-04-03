using UnityEngine;

public class RotationOnPivot : MonoBehaviour
{
    [SerializeField, Range(0f, 30)] private float snapValue = 10f; 
    public bool IsRotatable { get; set; }
    private float distanceFromRequiredAngle;
    private Transform selfTransform;

    private void Awake()
    {
        IsRotatable = false;
        selfTransform = transform; 
    }

    private void Update()
    {
        if (IsRotatable)
        {
            distanceFromRequiredAngle = DioravityCameraRotation.ZAngleWithIdentityRotation;
            selfTransform.localRotation = distanceFromRequiredAngle <= snapValue ? 
                                          Quaternion.identity : 
                                          Quaternion.Euler(0f, 0f, DioravityCameraRotation.ZRotation);
        }
    } 
}
