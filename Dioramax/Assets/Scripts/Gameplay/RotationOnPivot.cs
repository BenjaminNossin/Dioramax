using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RotationOnPivot : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private WinCondition winCondition;

    [Header("Values")]
    [SerializeField, Range(0f, 30)] private float snapValue = 10f;
    [SerializeField, Range(0, 360f)] private float initialZRotation;
    [SerializeField] private bool multiSnapAngles; 
    [SerializeField] private List<float> snapAngleValues; // hide if multiSnapAngles is false

    [Header("-- DEBUG --")]
    [SerializeField] private MeshRenderer meshRenderer;

    public bool IsRotatable { get; set; }
    private float distanceFromRequiredAngle;
    private Transform selfTransform;

    // DEBUG
    private float closest;
    private float selfEulerAngles;

    private void Start()
    {
        IsRotatable = false;
        selfTransform = transform;

        selfTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, initialZRotation));

        meshRenderer.material.color = Color.red;
    }

    private float cameraToPivotRotation;
    private void Update()
    {
        winCondition.UpdateWinCondition(selfTransform.localRotation == Quaternion.identity);

        if (IsRotatable)
        {
            distanceFromRequiredAngle = DioravityCameraCraneRotation.ZAngleWithIdentityRotation - initialZRotation;

            if (!multiSnapAngles)
            {
                selfTransform.localRotation = Mathf.Abs(distanceFromRequiredAngle) <= snapValue ?
                              Quaternion.identity :
                              Quaternion.Euler(0f, 0f, DioravityCameraCraneRotation.ZRotation + initialZRotation);
            }
            else
            {
                for (int i = 0; i < snapAngleValues.Count; i++)
                {
                    selfEulerAngles = selfTransform.eulerAngles.z;
                    cameraToPivotRotation = Mathf.Repeat(DioravityCameraCraneRotation.ZRotation + initialZRotation, 360f); 

                    closest = snapAngleValues
                                    .OrderBy(n => Mathf.Abs(n - selfTransform.eulerAngles.z))
                                    .First();

                    // WIP
                    selfTransform.localRotation = Mathf.Abs(cameraToPivotRotation - closest) <= snapValue ?
                              Quaternion.Euler(new Vector3(0f, 0f, closest)) :
                              Quaternion.Euler(0f, 0f, DioravityCameraCraneRotation.ZRotation + initialZRotation); 
                }
            }
        }
    }
}

