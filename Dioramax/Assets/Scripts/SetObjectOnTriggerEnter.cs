using UnityEngine;

public class SetObjectOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private LayerMask detectableEntityMask;

    [SerializeField] private GameObject obj;
    [SerializeField] private bool setActive;

    private Collider selfCollider;
    public bool ignoreLocomotive = true;
    private EntityPathNavigation entityPathNavigation;

    public bool deactivateSelfAfterDelay;
    [Range(0f, 5f)] public float delay = 3f; 

    private void Start()
    {
        selfCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == detectableEntityMask)
        {
            entityPathNavigation = other.GetComponent<EntityPathNavigation>();

            if (!entityPathNavigation.isWagon && ignoreLocomotive) return; 

            obj.SetActive(setActive);
            selfCollider.enabled = false;

            if (deactivateSelfAfterDelay)
            {
                Destroy(gameObject, delay);
            }
        }
    }
}
