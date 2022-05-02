using UnityEngine;

// a very basic script that inverts freeze state when touched, and adds/remove rb from list of simulated entities
public class FreezeStateController : MonoBehaviour
{
    [SerializeField] private bool freezeOnStart;
    [SerializeField] private SimulateEntityPhysics simulateEntityPhysics; 
    public bool Freezed { get; private set; }

    private void Start()
    {
        Freezed = freezeOnStart;

        if (!Freezed)
        {
            simulateEntityPhysics.AddRbToList();
        }
    }

    public void InvertFreezeState()
    {
        Freezed = !Freezed;
        AddOrRemoveSelfRb();
    }

    public void AddOrRemoveSelfRb()
    {
        if (Freezed)
        {
            simulateEntityPhysics.RemoveRbFromList();
        }
        else
        {
            simulateEntityPhysics.AddRbToList();
        }
    }
}
