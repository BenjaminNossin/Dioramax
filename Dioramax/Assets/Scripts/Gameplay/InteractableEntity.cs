using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(OverrideCameraPositionOnDoubleTap))]
public abstract class InteractableEntity : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private bool changeBackAfterDelay = true;
    [SerializeField] private bool interactablesCanBeShared = true;

    public bool InteractablesCanBeShared { get; set; }

    protected MeshRenderer meshRenderer; // VISUAL DEBUG
    protected bool isActive;

    private OverrideCameraPositionOnDoubleTap overrideCameraPositionOnDoubleTap;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        overrideCameraPositionOnDoubleTap = GetComponent<OverrideCameraPositionOnDoubleTap>();
        meshRenderer.material.color = Color.red;
        InteractablesCanBeShared = interactablesCanBeShared;
    }

    public virtual void ChangeColor()
    {
        isActive = true;
        meshRenderer.material.color = Color.blue;

        if (changeBackAfterDelay)
        {
            StartCoroutine((nameof(ChangeColorRoutine)));
        }
    }

    private readonly WaitForSeconds colorChangeWFS = new WaitForSeconds(0.5f);
    IEnumerator ChangeColorRoutine()
    {
        yield return colorChangeWFS;
        SwapOrChangeBack(false);
    }

    protected int[] swapArray;
    public virtual void SwapOrChangeBack(bool swap, int[] remoteChangeArray = null)
    {
        isActive = swap && !isActive;
        meshRenderer.material.color = isActive ? Color.blue : Color.red;

        swapArray = isActive ? new int[5] { 1, 1, 1, 1, 1 } : new int[5] { 0, 0, 0, 0, 0 };
    }

    public bool CanOverrideCameraPositionOnDoubleTap() => overrideCameraPositionOnDoubleTap.DoOverride;
    public Vector3 GetCameraPositionOverride() => overrideCameraPositionOnDoubleTap.GetNewPosition();

}
