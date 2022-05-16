using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class RotationOnPivot : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private WinCondition winCondition;
    [SerializeField] private Transform transfToRotate;
    [SerializeField] private MeshRenderer[] meshRenderers;


    [Header("Values")]
    [SerializeField, Range(0f, 30)] private float snapValue = 10f;
    [SerializeField, Range(0, 360f)] private float initialZRotation;
    [SerializeField] private bool multiSnapAngles; 
    [SerializeField] private List<float> snapAngleValues; // hide if multiSnapAngles is false

    [Header("Other")]
    [SerializeField] private TweenTouch tweenTouch;
    [SerializeField] private Collider tweenCollider;
    private Collider selfCollider; 


    public bool IsLocked { get; set; }
    private float distanceFromRequiredAngle;
    private bool validPositionIsRegistered;

    private void OnEnable()
    {
        TouchDetection.OnTuyauDetected += CheckIfWasDetected;
    }

    private void OnDisable()
    {
        TouchDetection.OnTuyauDetected -= CheckIfWasDetected;
    }

    private void Start()
    {
        IsLocked = false;

        transfToRotate.localRotation = Quaternion.Euler(new Vector3(0f, 0f, initialZRotation));
        selfCollider = GetComponent<Collider>();

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.SetInt("_Freezed", IsLocked ? 1 : 0);
        }
    }

    private void Update()
    {
        if (IsLocked || validPositionIsRegistered) return;

        distanceFromRequiredAngle = DioravityCameraCraneRotation.ZAngleWithIdentityRotation - initialZRotation;
        transfToRotate.localRotation = Quaternion.Euler(0f, 0f, initialZRotation + (DioravityCameraCraneRotation.ZAngleWithIdentityRotation * -1f));

        if (multiSnapAngles)
        {
            for (int i = 0; i < snapAngleValues.Count; i++)
            {
                if (Mathf.Abs(transfToRotate.localEulerAngles.z - snapAngleValues[i]) <= snapValue)
                {
                    transfToRotate.localRotation = Quaternion.Euler(0f, 0f, snapAngleValues[i]);
                }
            }
        }
    }

    public void CheckWinConditionOnLock()
    {
        if (Mathf.Abs(distanceFromRequiredAngle) <= snapValue)
        {
            transfToRotate.localRotation = Quaternion.identity;

            if (!validPositionIsRegistered)
            {
                validPositionIsRegistered = true;
                winCondition.UpdateWinCondition(true);

                // fx and stop button tween
                LevelManager.Instance.OnTuyauxValidPosition(winCondition.entityNumber);
                tweenTouch.enabled = false;
                tweenCollider.enabled = false;
            }
        }
    }

    private void CheckIfWasDetected(Collider _detectedCollider)
    {
        if (selfCollider == _detectedCollider)
        {
            IsLocked = !IsLocked;
        }
        else
        {
            IsLocked = false; 
        }

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.SetInt("_Freezed", IsLocked ? 1 : 0);
        }

        if (IsLocked)
        {
            CheckWinConditionOnLock(); 
        }
    }
}

