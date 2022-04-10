using UnityEngine;

// on indivividual objects
[RequireComponent(typeof(Rigidbody))]
public class SimulateEntityPhysics : MonoBehaviour, ISimulatePhysics
{
    [SerializeField] private Rigidbody rb; 
    public static System.Action<SimulateEntityPhysics, Rigidbody> OnInitializePhysicsEntity { get; set; }

    void Start()
    {
        Initialize(); 
    }

    public void Initialize()
    {
        try
        {
            OnInitializePhysicsEntity(this, rb);
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message + "\n You may be lacking an EntityPhysicsController component on top of your diorama's hierarchy"); 
        }
    }
}
