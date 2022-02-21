using UnityEngine;

// on indivividual objects
[RequireComponent(typeof(Rigidbody))]
public class SimulateEntityPhysics : MonoBehaviour, ISimulatePhysics
{
    [SerializeField] private Rigidbody rb; 
    public static System.Action<SimulateEntityPhysics, Rigidbody> OnInitializePhysicsEntity { get; set; }
    public static System.Action<Vector3> OnCameraZRotationUpdate { get; set; }

    void Start()
    {
        Initialize(); 
    }

    public void Initialize()
    {
        OnInitializePhysicsEntity(this, rb); 
    }
}
