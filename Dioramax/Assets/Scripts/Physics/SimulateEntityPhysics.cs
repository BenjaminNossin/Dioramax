using UnityEngine;

// on indivividual objects
[RequireComponent(typeof(Rigidbody))]
public class SimulateEntityPhysics : MonoBehaviour
{
    [SerializeField] private Rigidbody rb; 

    public void AddRbToList()
    {
        EntityPhysicsController.Instance.AddRbToList(rb); 
    }

    public void RemoveRbFromList()
    {
        GameLogger.Log("removing"); 
        EntityPhysicsController.Instance.RemoveRbFromList(rb); 
    }
}
