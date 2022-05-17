using UnityEngine;

// a very basic script that inverts freeze state when touched, and adds/remove rb from list of simulated entities
[RequireComponent(typeof(TweenTouch))]
public class FreezeStateController : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] meshRenderers; 

    [Space, SerializeField] private bool freezeOnStart;
    [SerializeField] private SimulateEntityPhysics simulateEntityPhysics; 
    public bool Freezed { get; private set; }

    private void Start()
    {
        Freezed = freezeOnStart;

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.SetInt("_Freezed", Freezed ? 1 : 0);
        }

        /* for (int i = 0; i < freezeMaterialInstances.Length; i++)
        {
            freezeMaterialInstances[i].SetInteger("_Freezed", Freezed ? 1 : 0); 
        } */

        if (!Freezed)
        {
            simulateEntityPhysics.AddRbToList();
        }
    }

    public void InvertFreezeState()
    {
        Freezed = !Freezed;
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.SetInt("_Freezed", Freezed ? 1 : 0);
        }

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
