using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LoadNewPathOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private LayerMask detectableEntityMask;
    [SerializeField] private PathController pathController;
    [SerializeField] private int startingIndex;

    [SerializeField] private Animator camAnimator;
    [SerializeField] private AnimationClip clip; 

    private Collider selfCollider;

    private void Start()
    {
        selfCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == detectableEntityMask)
        {
            // other.GetComponent<Animator>().enabled = true;
            other.GetComponent<EntityPathNavigation>().LoadNewPath(pathController, startingIndex);
            selfCollider.enabled = false;

            camAnimator.enabled = true; 
            camAnimator.Play(clip.name);
        }
    }
}
