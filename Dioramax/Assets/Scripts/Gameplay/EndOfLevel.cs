using UnityEngine;

public class EndOfLevel : MonoBehaviour
{
    [SerializeField] private LayerMask detectableEntityMask;
    [SerializeField] private Collider starShieldCollider;
    [SerializeField] private Collider starCollider;

    [SerializeField] private Tween_Star_Finish finish;
    [SerializeField] private GameObject sparkleVFX;

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
            starCollider.enabled = true;

            finish.enabled = true;
            sparkleVFX.SetActive(true);
        }
    }
}
