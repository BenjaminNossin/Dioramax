using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public partial class InteractableEntity : MonoBehaviour
{
    [SerializeField] protected MeshRenderer renderer;
    [SerializeField] private bool changeBackAfterDelay = true;
    [SerializeField] private bool interactablesCanBeShared = true; 
    protected bool isActive;
    public bool InteractablesCanBeShared { get; set; }

    private readonly UnityEvent<int[]> OnChangeStateEvent = new(); 
    protected UnityAction<int[]> OnChangeStateCallback;

    private void Start()
    {
        InteractablesCanBeShared = interactablesCanBeShared;
        renderer.material.color = Color.red;
        OnChangeStateEvent.AddListener(OnChangeStateCallback);
    }

    public void ChangeColor()
    {
        isActive = true;
        renderer.material.color = Color.blue;
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
        renderer.material.color = isActive ? Color.blue : Color.red;

        int[] swapArray = isActive ? new int[5] { 1, 1, 1, 1, 1 } : new int[5] { 0, 0, 0, 0, 0 };
        OnChangeStateEvent.Invoke(remoteChangeArray ?? swapArray);
    }
}
