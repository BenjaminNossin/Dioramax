using UnityEngine;
using UnityEngine.Events;


public class InteractableEntityRemote : InteractableEntity
{
    [SerializeField] public MeshRenderer[] entitiesMeshRenderers;
    public RotationOnPivot[] rotationOnPivots;

    private readonly UnityEvent<int[]> OnChangeStateEvent = new();
    protected UnityAction<int[]> OnChangeStateCallback { get; set; }

    private void OnEnable()
    {
        OnChangeStateCallback += OnRegisterChange;
    }

    private void OnDisable()
    {
        OnChangeStateCallback -= OnRegisterChange;
    }

    private void Start()
    {
        OnChangeStateEvent.AddListener(OnChangeStateCallback);
    }

    public override void ChangeColor()
    {
        base.ChangeColor();
        OnChangeStateEvent.Invoke(new int[5] { 1, 1, 1, 1, 1 });
    }

    public override void SwapOrChangeBack(bool swap, int[] remoteChangeArray = null)
    {
        base.SwapOrChangeBack(swap, remoteChangeArray);
        OnChangeStateEvent.Invoke(remoteChangeArray ?? swapArray);
    }

    private void OnRegisterChange(int[] sharedEntitiesWithCurrent)
    {
        for (int i = 0; i < entitiesMeshRenderers.Length; i++)
        {
            entitiesMeshRenderers[i].material.color = sharedEntitiesWithCurrent[i] == 1 ? Color.blue : Color.red;
            rotationOnPivots[i].IsLocked = sharedEntitiesWithCurrent[i] == 1; 
        }
    }

}

