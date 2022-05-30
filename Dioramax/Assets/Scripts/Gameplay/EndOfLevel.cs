using UnityEngine;

public class EndOfLevel : MonoBehaviour
{
    [SerializeField] private LayerMask detectableEntityMask;
    [SerializeField] private Collider starShieldCollider;
    [SerializeField] private Tween_Star_Finish finish;

    private Collider selfCollider;

    private void Start()
    {
        selfCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == detectableEntityMask)
        {
            selfCollider.enabled = false;
            starShieldCollider.enabled = false;

            finish.enabled = true;
        }
    }
}
