using UnityEngine;

public class AnimEvent_SetObjectActive : MonoBehaviour
{
    [SerializeField] private GameObject objToDeactivate;
    [SerializeField] private bool value; 

    public void DeactivateObject()
    {
        objToDeactivate.SetActive(value); 
    }
}
