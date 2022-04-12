using UnityEngine;

public class ButtonProp : MonoBehaviour
{
    [SerializeField] public OverrideCameraPositionOnDoubleTap overrideCameraPositionOnDoubleTap;
    [SerializeField] private MeshRenderer selfMeshRenderer; 

    [SerializeField] public MeshRenderer[] entitiesMeshRenderers;
    public RotationOnPivot[] rotationOnPivots;
    public bool isActive;

    private void Start()
    {
        selfMeshRenderer.material.color = Color.red;
        for (int i = 0; i < entitiesMeshRenderers.Length; i++)
        {
            entitiesMeshRenderers[i].material.color = Color.red;
        }
    }

    // button becomes active and inverts all pivots lock state
    // button becomes inactive. Pivots state does not change unless previous == current
    public void SetButtonState(bool invertPivotsLockState = false)
    {
        isActive = !isActive;

        if (isActive)
        {
            selfMeshRenderer.material.color = Color.green;
            InvertPivotsLockState();
        }
        else
        {
            selfMeshRenderer.material.color = Color.red;

            if (invertPivotsLockState)
            {
                InvertPivotsLockState();
            }
        }
    }

    private void InvertPivotsLockState()
    {
        for (int i = 0; i < entitiesMeshRenderers.Length; i++)
        {
            rotationOnPivots[i].IsLocked = !rotationOnPivots[i].IsLocked;
            entitiesMeshRenderers[i].material.color = rotationOnPivots[i].IsLocked ? Color.green : Color.red;
        }
    }

    public bool CanOverrideCameraPositionOnDoubleTap() => overrideCameraPositionOnDoubleTap.DoOverride;
    public Vector3 GetCameraPositionOverride() => overrideCameraPositionOnDoubleTap.GetNewPosition();

}
