using UnityEngine;

// a very basic script that inverts freeze state when touched, and adds/remove rb from list of simulated entities
[RequireComponent(typeof(TweenTouch))]
public class FreezeStateController : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] meshRenderers; 

    [Space, SerializeField] private bool freezeOnStart;
    [SerializeField] private SimulateEntityPhysics simulateEntityPhysics;

    [Space, SerializeField] AudioSource audiosource; 
    [SerializeField] AudioClip[] freezeUnfreezeAudioclips = new AudioClip[2];

    [Header("--DEBUG--")]
    [SerializeField] private bool useDebugTrain;
    [SerializeField] private bool keepIndividualFreeze; 
    [SerializeField] private FreezeStateController otherFreeze;

    public bool Freezed { get; private set; }
    public bool IsLinked { get; set; }

    private void OnDisable()
    {
        if (otherFreeze)
        {
            Debug_StickTrains.OnDetachChildren += FreezeSelfAndReference; 
        }
    }

    private void OnEnable()
    {
        if (otherFreeze)
        {
            Debug_StickTrains.OnDetachChildren -= FreezeSelfAndReference; 
        }
    }

    private void Start()
    {
        Freezed = freezeOnStart;

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.SetInt("Freezed", Freezed ? 1 : 0);
        }

        if (!Freezed && simulateEntityPhysics)
        {
            simulateEntityPhysics.AddRbToList();           
        }
    }

    public void InvertFreezeState()
    {
        Freezed = !Freezed;
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.SetInt("Freezed", Freezed ? 1 : 0);
            audiosource.PlayOneShot(freezeUnfreezeAudioclips[Freezed ? 1 : 0]);
        }

        if (!useDebugTrain)
        {
            AddOrRemoveSelfRb();
        }
        else
        {
            if (!keepIndividualFreeze && Debug_StickTrains.IsOn)
            {
                otherFreeze.SetFreezeState(Freezed);
            }
        }
    }

    public void AddOrRemoveSelfRb()
    {
        if (!useDebugTrain && simulateEntityPhysics)
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

    public void SetFreezeState(bool state)
    {
        Freezed = state;
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.SetInt("Freezed", Freezed ? 1 : 0);
            audiosource.PlayOneShot(freezeUnfreezeAudioclips[Freezed ? 1 : 0]);
        }
    }

    private void FreezeSelfAndReference()
    {
        Freezed = true; 
        SetFreezeState(Freezed); 
        otherFreeze.SetFreezeState(Freezed); 
    }
}
