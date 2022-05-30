using UnityEngine;

public class StopLocomotiveOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private LayerMask detectableEntityMask;
    private Collider selfCollider;

    public bool stopCompletely;

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

            entityPathNavigation.enabled = false;
            selfCollider.enabled = false;

            if (!stopCompletely)
            {
                StartCoroutine(ReactivateLocomotive());
            }
        }
    }

    private readonly WaitForSeconds WFS = new(3.5f);
    System.Collections.IEnumerator ReactivateLocomotive()
    {
        yield return WFS;
        entityPathNavigation.enabled = true; 

    }
}
