using UnityEngine;

public class CameraCinematic : MonoBehaviour
{
    public static CameraCinematic Instance { get; private set; }

    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip phase2Cinematic;

    private void Awake()
    {
        animator.applyRootMotion = false; 
        if (Instance)
        {
            Destroy(Instance); 
        }
        Instance = this; 
    }

    public void PlayCinematic()
    {
        Debug.Log("playing cinematic");
        SetAnimatorState(1); 
        animator.Play(phase2Cinematic.name); 
    }

    // done at the end of every cinematic
    public void SetAnimatorState(int enabled) // can't serialize bool.. 
    {
        animator.enabled = enabled == 1; // PLACEHOLDER until I find how to get control of movement when animation is done; 
    }
}
