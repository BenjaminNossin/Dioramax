using UnityEngine;
using System.Collections.Generic;

public class RotationOnPivot : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private WinCondition winCondition;

    [Header("Values")]
    [SerializeField, Range(0f, 30)] private float snapValue = 10f;
    [SerializeField, Range(0, 360f)] private float initialZRotation;
    [SerializeField] private bool multiSnapAngles; 
    [SerializeField] private List<float> snapAngleValues; // hide if multiSnapAngles is false

    public bool IsLocked { get; set; }
    private float distanceFromRequiredAngle;
    private Transform selfTransform;

    private void Start()
    {
        IsLocked = false;
        selfTransform = transform;

        selfTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, initialZRotation));
    }

    private void Update()
    {
        winCondition.UpdateWinCondition(selfTransform.localRotation == Quaternion.identity);

        if (IsLocked) return; 

        distanceFromRequiredAngle = DioravityCameraCraneRotation.ZAngleWithIdentityRotation - initialZRotation;

        // rotate by using the inverse of camera rotation
        selfTransform.localRotation = Mathf.Abs(distanceFromRequiredAngle) <= snapValue ?
                                      Quaternion.identity :
                                      Quaternion.Euler(0f, 0f, initialZRotation + (DioravityCameraCraneRotation.ZAngleWithIdentityRotation * -1f));

        if (multiSnapAngles)
        {
            for (int i = 0; i < snapAngleValues.Count; i++)
            {
                if (Mathf.Abs(selfTransform.localEulerAngles.z - snapAngleValues[i]) <= snapValue)
                {
                    selfTransform.localRotation = Quaternion.Euler(0f, 0f, snapAngleValues[i]);
                }
            }
        }
    }
}

