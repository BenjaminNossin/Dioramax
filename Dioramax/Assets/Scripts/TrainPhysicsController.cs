using UnityEngine;


public class TrainPhysicsController : MonoBehaviour
{
    [SerializeField] private Transform mainCamTransform;
    [SerializeField] private Rigidbody rb; 
    [SerializeField, Range(0.05f, 2f)] private float gravityForceMultiplier = 1f;
    [SerializeField, Range(0.7f, 1f)] private float tolerance = 0.9f; 

    private const float GRAVITY_FORCE = 9.81f;
    private float dotProductCamAndDirection;

    private bool goingBack; // event
    private bool changeOfDirectionIsStored;
    private float remappedTolerance; 

    private void Update()
    {
        // goingBack
        // changeOfDirectionIsStored
        // EntityPathNavigation.Instance.UpdateOnDirectionChange(); 
    }

    void FixedUpdate()
    {
        UpdateEntities();
    }

    private float remappedValue; 
    // BUG 23.05 : destination node stays -1, so the entity does not move.. 
    private void UpdateEntities()
    {
        Debug.DrawRay(transform.position, mainCamTransform.up * -5, Color.red);
        dotProductCamAndDirection = Vector3.Dot(mainCamTransform.up * -1, EntityPathNavigation.NormalizedMoveDirection);
        if (IsBetweenMinAndMax(dotProductCamAndDirection, tolerance, 1f))
        {
            // 0.8 -> 0;  1 -> 1
            remappedTolerance = Remap(dotProductCamAndDirection, tolerance, 1f, 0f, 1f); 
            EntityPathNavigation.Instance.UpdateNavigationSpeed(remappedTolerance); 
            // rb.AddForce(-GRAVITY_FORCE * gravityForceMultiplier * lerpedTolerance * EntityPathNavigation.NormalizedMoveDirection, ForceMode.Acceleration);
        }
    }

    private float Remap(float value, float from1, float to1, float from2, float to2) 
        => (value - from1) / (to1 - from1) * (to2 - from2) + from2;

    private bool IsBetweenMinAndMax(float value, float min, float max) => value >= min && value <= max; 
}
