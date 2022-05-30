using UnityEngine;

public class ActivateAnimationOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private LayerMask detectableEntityMask;
    [SerializeField] private PathController pathController;
    [SerializeField] private int startingIndex; 

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
        }
    }
}
