using UnityEngine;
using System.Collections;

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
    private EntityPathNavigation entityPathNavigation; 


    private void Start()
    {
        selfCollider = GetComponent<Collider>();
    }

    private Collider wagon; 
    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == detectableEntityMask)
        {
            wagon = other; 

            selfCollider.enabled = false;

            camAnimator.enabled = true;
            camAnimator.Play(clip.name);

            if (passengersAnimator)
            {
                passengersAnimator.enabled = true;
            }

            if (passengersTrain)
            {
                passengersTrain.SetActive(true);
            }

            StartCoroutine(nameof(SetInversion));
        }
    }

    private readonly WaitForSeconds WFS = new(3f); 
    private IEnumerator SetInversion()
    {
        yield return WFS;
        EntityPathNavigation.CurrentNavigationState = NavigationState.Backward;
        entityPathNavigation = wagon.GetComponent<EntityPathNavigation>();

        entityPathNavigation.simulateBackward = true; 
        entityPathNavigation.UpdateOnDirectionChange(); 
    }
}
