using UnityEngine;

public class SetObjectOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private LayerMask detectableEntityMask;

    [SerializeField] private GameObject obj;
    [SerializeField] private bool setActive;

    private Collider selfCollider;
    public bool ignoreLocomotive = true;
    private EntityPathNavigation entityPathNavigation;

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
        }
    }
}
