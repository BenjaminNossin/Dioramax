using UnityEngine;


public class StopTrainOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private LayerMask detectableEntityMask;
    private Collider selfCollider;

    public static System.Action OnFinishingObstacle1; 

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
            if (entityPathNavigation.isWagon)
            {
                entityPathNavigation.enabled = false;
                selfCollider.enabled = false;
                StartCoroutine(DeactivateWagon());
            }
        }
    }

    private readonly WaitForSeconds WFS = new(2f);
    System.Collections.IEnumerator DeactivateWagon()
    {
        yield return WFS;
        entityPathNavigation.gameObject.SetActive(false);

        OnFinishingObstacle1();
    }

}
