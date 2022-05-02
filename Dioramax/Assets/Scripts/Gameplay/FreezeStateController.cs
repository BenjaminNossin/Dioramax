using UnityEngine;

// a very basic script that inverts freeze state when touched
public class FreezeStateController : MonoBehaviour
{
    [SerializeField] private bool freezeOnAwake;
    [SerializeField] private SimulateEntityPhysics simulateEntityPhysics; // rework EntityPhysicsController too
    public bool Freezed { get; private set; }

    private void Awake()
    {
        Freezed = freezeOnAwake; 
    }

    public void InvertFreezeState()
    {
        Freezed = !Freezed;
    }
}
