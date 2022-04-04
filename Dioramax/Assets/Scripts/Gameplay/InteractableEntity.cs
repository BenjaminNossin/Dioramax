using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public abstract class InteractableEntity : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private bool changeBackAfterDelay = true;
    [SerializeField] private bool interactablesCanBeShared = true;

    protected MeshRenderer meshRenderer; // VISUAL DEBUG
    protected bool isActive;
    public bool InteractablesCanBeShared { get; set; }


    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        InteractablesCanBeShared = interactablesCanBeShared;
        meshRenderer.material.color = Color.red;
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
}
