using UnityEngine;

// this script violates Single Responsibility Principle by a long shot
// why reference RotationOnPivot and have an InvertPivotLockState() here ??
public class ButtonProp : MonoBehaviour
{
    [SerializeField] public OverrideCameraPositionOnDoubleTap overrideCameraPositionOnDoubleTap;
    [SerializeField] private MeshRenderer selfMeshRenderer; 

    [SerializeField] public MeshRenderer[] entitiesMeshRenderers;
    public RotationOnPivot[] rotationOnPivots;
    public bool buttonIsActive;

    private void Start()
    {
        selfMeshRenderer.material.color = Color.red;
        for (int i = 0; i < entitiesMeshRenderers.Length; i++)
        {
            entitiesMeshRenderers[i].material.color = Color.red;
        }
    }

    public void InverseButtonState()
    {
        buttonIsActive = !buttonIsActive;

        selfMeshRenderer.material.color = buttonIsActive ? Color.green : Color.red;
        SetPivotLockState();
    }

    public void SetButtonOff()
    {
        buttonIsActive = false;

        selfMeshRenderer.material.color = Color.red;
        SetPivotLockState();     
    }

    private void SetPivotLockState()
    {
        for (int i = 0; i < entitiesMeshRenderers.Length; i++)
        {
            rotationOnPivots[i].IsLocked = buttonIsActive;
            entitiesMeshRenderers[i].material.color = rotationOnPivots[i].IsLocked ? Color.green : Color.red;
            rotationOnPivots[i].CheckWinConditionOnLock();
        }
    }

    public bool CanOverrideCameraPositionOnDoubleTap() => overrideCameraPositionOnDoubleTap.DoOverride;

    public Vector3 GetCameraPositionOverride() => overrideCameraPositionOnDoubleTap.GetNewPosition();
}
