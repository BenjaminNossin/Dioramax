using UnityEngine;


public class InteractableEntityRemote : InteractableEntity
{
    [SerializeField] public MeshRenderer[] entitiesMeshRenderers;
    public RotationOnPivot[] rotationOnPivots; 

    private void OnEnable()
    {
        RegisterOnChangeStateCallback();
    }

    private void OnDisable()
    {
        UnregisterOnChangeStateCallback();
    }

    private void RegisterOnChangeStateCallback()
    {
        OnChangeStateCallback += OnRegisterChange;
    }

    private void UnregisterOnChangeStateCallback()
    {
        OnChangeStateCallback -= OnRegisterChange;
    }

    private void OnRegisterChange(int[] sharedEntitiesWithCurrent)
    {
        for (int i = 0; i < entitiesMeshRenderers.Length; i++)
        {
            entitiesMeshRenderers[i].material.color = sharedEntitiesWithCurrent[i] == 1 ? Color.blue : Color.red;
            rotationOnPivots[i].IsRotatable = sharedEntitiesWithCurrent[i] == 1; 
        }
    }

}
