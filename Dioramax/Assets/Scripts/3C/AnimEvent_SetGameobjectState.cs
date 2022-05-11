using UnityEngine;

public class AnimEvent_SetGameobjectState : MonoBehaviour
{
    [SerializeField] private GameObject objToDeactivate; 

    public void DeactivateObject()
    {
        GameLogger.Log("deactivating object");
        objToDeactivate.SetActive(false); 
    }
}
