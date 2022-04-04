using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public abstract class InteractableEntity : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private bool changeBackAfterDelay = true;
    [SerializeField] private bool interactablesCanBeShared = true;
    [SerializeField] private bool overrideCameraPositionOnDoubleTap;
    [SerializeField] private Vector3 doubleTapCameraCraneFocusPosition; // hide this is previous bool is false


    protected MeshRenderer meshRenderer; // VISUAL DEBUG
    protected bool isActive;
    public bool InteractablesCanBeShared { get; set; }
    public bool OverrideCameraPositionOnDoubleTap { get; private set; }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.red;
        InteractablesCanBeShared = interactablesCanBeShared;
        OverrideCameraPositionOnDoubleTap = overrideCameraPositionOnDoubleTap;
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

    public Vector3 GetCameraCraneFocusPosition() => doubleTapCameraCraneFocusPosition;
}
