using UnityEngine;

public class RotationOnPivot : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private WinCondition winCondition;

    [Header("Values")]
    [SerializeField, Range(0f, 30)] private float snapValue = 10f;
    [SerializeField, Range(0, 360f)] private float initialZRotation;

    [Header("-- DEBUG --")]
    [SerializeField] private MeshRenderer meshRenderer;

    public bool IsRotatable { get; set; }
    private float distanceFromRequiredAngle;
    private Transform selfTransform; 

    private void Start()
    {
        IsRotatable = false;
        selfTransform = transform;

        selfTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, initialZRotation));

        meshRenderer.material.color = Color.red;
    }

    private void Update()
    {
        winCondition.UpdateWinCondition(selfTransform.localRotation == Quaternion.identity);

        if (IsRotatable)
        {
            distanceFromRequiredAngle = DioravityCameraCraneRotation.ZAngleWithIdentityRotation;
            selfTransform.localRotation = distanceFromRequiredAngle <= snapValue ?
                                          Quaternion.identity :
                                          Quaternion.Euler(0f, 0f, DioravityCameraCraneRotation.ZRotation);
        }
    }
}

