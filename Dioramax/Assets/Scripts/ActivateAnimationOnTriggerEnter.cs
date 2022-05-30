using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ActivateAnimationOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private LayerMask detectableEntityMask;

    [Header("Cinematics")]
    [SerializeField] private Animator camAnimator;
    [SerializeField] private AnimationClip clip;
    [SerializeField] private GameObject passengersTrain;
    [SerializeField] private Animator passengersAnimator;

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

            camAnimator.enabled = true;
            camAnimator.Play(clip.name);

            passengersAnimator.enabled = true;

            passengersTrain.SetActive(true); 
        }
    }
}
