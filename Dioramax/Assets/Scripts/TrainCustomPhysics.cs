using UnityEngine;


public class TrainCustomPhysics : MonoBehaviour
{
    [SerializeField] private Transform mainCamTransform;
    [SerializeField, Range(0.05f, 2f)] private float gravityForceMultiplier = 1f;
    [SerializeField, Range(0.4f, 1f)] private float tolerance = 0.65f; 

    private float dotProductCamAndDirection;
    private float remappedTolerance;

    private bool changeIsDone, canMove; 

    void Update()
    {
        UpdateEntities();
    }

    private float direction; 
    private void UpdateEntities()
    {
        GameDrawDebugger.DrawRay(transform.position, mainCamTransform.up * -5, Color.red);

        canMove = IsBetweenMinAndMax(dotProductCamAndDirection, tolerance, 1f); 

        dotProductCamAndDirection = Vector3.Dot(mainCamTransform.up * -1, EntityPathNavigation.NormalizedMoveDirection);
        direction = Mathf.Sign(dotProductCamAndDirection);
        remappedTolerance = Remap(dotProductCamAndDirection, tolerance * direction, 1f * direction, 0f, 1f * direction);

        EntityPathNavigation.Instance.UpdateNavigationSpeed(canMove ? Mathf.Abs(remappedTolerance) : 0);

        // within certain angle forward of backward
        if (canMove)
        {
            // DONE ONCE
            if (!changeIsDone)
            {
                GameLogger.Log("CHANGING EVENT");
                changeIsDone = true;
                EntityPathNavigation.CurrentNavigationState = (NavigationState)direction; 
                Debug.Log($"current navigation state: {EntityPathNavigation.CurrentNavigationState}"); 
            }

            // DONE ONCE
            if (EntityPathNavigation.CurrentNavigationState != EntityPathNavigation.PreviousNavigationState)
            {
                GameLogger.Log("INVERTING EVENT");
                EntityPathNavigation.Instance.UpdateOnDirectionChange();
            }
        } 
        // out of certain angle
        else
        {
            // DONE ONCE
            if (changeIsDone)
            {
                GameLogger.Log("OUT OF ALLOWED RANGE EVENT");
                changeIsDone = false;
            }
        }
    }

    private float Remap(float value, float from1, float to1, float from2, float to2) 
        => (value - from1) / (to1 - from1) * (to2 - from2) + from2; 

    private bool IsBetweenMinAndMax(float value, float min, float max) => Mathf.Abs(value) >= min && value <= max; 
}
