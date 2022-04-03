using System.Collections.Generic;
using UnityEngine;

// winConditonIsMet -> update Array of list with dioramaInfos as indexes

public class RotationOnPivot : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer; 
    [SerializeField] private DioramaInfos dioramaInfos;
    [SerializeField, Range(0f, 30)] private float snapValue = 10f;
    [SerializeField, Range(0, 360f)] private float initialZRotation;

    // move this to a WinConditionComponent that is local to each piece of the puzzle
    [Space, SerializeField] private DioramaPuzzleName entityPuzzleName; // PLACEHOLDER
    [SerializeField] private int entityNumber; // PLACEHOLDER

    public bool IsRotatable { get; set; }
    private float distanceFromRequiredAngle;
    private Transform selfTransform;

    public bool winConditionIsMet;
    public bool WinConditionEventIsRegistered;

    private void Awake()
    {
        IsRotatable = false;
        selfTransform = transform;

        winConditionIsMet = selfTransform.localRotation == Quaternion.identity;
        selfTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, initialZRotation));
    }

    private void Start()
    {
        meshRenderer.material.color = Color.red;
    }

    private void Update()
    {
        winConditionIsMet = selfTransform.localRotation == Quaternion.identity; // change to event, dont trigger it every frame
        if (winConditionIsMet && !WinConditionEventIsRegistered)
        {
            WinConditionEventIsRegistered = true;
            WinConditionController.Instance.ValidateWinCondition((int)entityPuzzleName, entityNumber);
        }
        else if (!winConditionIsMet && WinConditionEventIsRegistered)
        {
            WinConditionEventIsRegistered = false;
            WinConditionController.Instance.InvalidateWinCondition((int)entityPuzzleName, entityNumber);
        }

        if (IsRotatable)
        {
            distanceFromRequiredAngle = DioravityCameraRotation.ZAngleWithIdentityRotation;
            selfTransform.localRotation = distanceFromRequiredAngle <= snapValue ?
                                          Quaternion.identity :
                                          Quaternion.Euler(0f, 0f, DioravityCameraRotation.ZRotation);
        }
    }
}

