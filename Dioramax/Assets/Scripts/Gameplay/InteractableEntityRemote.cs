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

    private bool swappingBack; 
    public override void ChangeColor()
    {
        base.ChangeColor();

        swappingBack = false;
        OnChangeStateEvent.Invoke(new int[5] { 0, 0, 0, 0, 0 }); // is locked = false for all by default
    }

    public override void SwapOrChangeBack(bool swap, int[] remoteChangeArray = null)
    {
        base.SwapOrChangeBack(swap, remoteChangeArray);

        swappingBack = true;
        OnChangeStateEvent.Invoke(remoteChangeArray ?? swapArray);
    }

    private void OnRegisterChange(int[] sharedEntitiesWithCurrent)
    {
        for (int i = 0; i < entitiesMeshRenderers.Length; i++)
        {
            if (!swappingBack)
            {
                // int newValue = SwapOneAndZero(sharedEntitiesWithCurrent[i]);
                rotationOnPivots[i].IsLocked = !rotationOnPivots[i].IsLocked;
                GameLogger.Log($"inverted value is now {rotationOnPivots[i].IsLocked} for {gameObject}");
            }
            else
            {
                rotationOnPivots[i].IsLocked = sharedEntitiesWithCurrent[i] == 1;
                GameLogger.Log($"swapped back value is now {rotationOnPivots[i].IsLocked} for {gameObject}");
            }

            entitiesMeshRenderers[i].material.color = rotationOnPivots[i].IsLocked ? Color.blue : Color.red;
        }
    }
}

