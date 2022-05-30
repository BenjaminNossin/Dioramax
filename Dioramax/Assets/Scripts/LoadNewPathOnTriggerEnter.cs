using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LoadNewPathOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private LayerMask detectableEntityMask;
    [SerializeField] private PathController pathController;
    [SerializeField] private int startingIndex;

    [Header("Cinematics")]
    [SerializeField] private Animator camAnimator;
    [SerializeField] private AnimationClip clip; 

    private Collider selfCollider;
    public bool simulateBackward; 

    private void Start()
    {
        selfCollider = GetComponent<Collider>();
    }

    private EntityPathNavigation entityPathNavigation;
    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == detectableEntityMask)
        {
            entityPathNavigation = other.GetComponent<EntityPathNavigation>();
            entityPathNavigation.simulateBackward = simulateBackward; 

            selfCollider.enabled = false;

            camAnimator.enabled = true;
            camAnimator.Play(clip.name);

            entityPathNavigation.LoadNewPath(pathController, startingIndex);

        }
    }
}
