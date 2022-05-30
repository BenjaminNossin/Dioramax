using UnityEngine;

public class SetObjectOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private LayerMask detectableEntityMask;

    [SerializeField] private GameObject obj;
    [SerializeField] private bool setActive;

    private Collider selfCollider;

    private void Start()
    {
        selfCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == detectableEntityMask)
        {
            obj.SetActive(setActive);
            selfCollider.enabled = false;
        }
    }
}
