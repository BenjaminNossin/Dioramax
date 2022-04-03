using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer))]
public partial class InteractableEntity : MonoBehaviour
{
    [SerializeField] private bool changeBackAfterDelay = true;
    [SerializeField] private bool interactablesCanBeShared = true;
    protected MeshRenderer meshRenderer; // VISUAL DEBUG
    protected bool isActive;
    public bool InteractablesCanBeShared { get; set; }

    private readonly UnityEvent<int[]> OnChangeStateEvent = new(); 
    protected UnityAction<int[]> OnChangeStateCallback { get; set; }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>(); 
    }

    private void Start()
    {
        InteractablesCanBeShared = interactablesCanBeShared;
        meshRenderer.material.color = Color.red;
        OnChangeStateEvent.AddListener(OnChangeStateCallback);
    }

    public void ChangeColor()
    {
        isActive = true;
        meshRenderer.material.color = Color.blue;
        OnChangeStateEvent.Invoke(new int[5] { 1, 1, 1, 1, 1 }); 

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

    public void SwapOrChangeBack(bool swap, int[] remoteChangeArray = null)
    {
        isActive = swap && !isActive;
        meshRenderer.material.color = isActive ? Color.blue : Color.red;

        int[] swapArray = isActive ? new int[5] { 1, 1, 1, 1, 1 } : new int[5] { 0, 0, 0, 0, 0 };
        OnChangeStateEvent.Invoke(remoteChangeArray ?? swapArray);
    }
}
