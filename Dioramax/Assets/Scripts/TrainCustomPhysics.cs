using UnityEngine;


public class TrainCustomPhysics : MonoBehaviour
{
    [SerializeField] private Transform mainCamTransform;
    [SerializeField, Range(0.05f, 2f)] private float gravityForceMultiplier = 1f;
    [SerializeField, Range(0.4f, 1f)] private float tolerance = 0.65f; 

    private float dotProductCamAndDirection;
    private float remappedTolerance;

    private bool changeIsDone, canMove;
    private bool doneOnce;

    private float currentDirection, previousDirection;
    private void Start()
    {
        currentDirection = previousDirection = -1;
    }

    void Update()
    {
        UpdateEntities();
    }

    private void UpdateEntities()
    {
        GameDrawDebugger.DrawRay(transform.position, mainCamTransform.up * -5, Color.red);

        canMove = IsBetweenMinAndMax(dotProductCamAndDirection, tolerance, 1f); 

        dotProductCamAndDirection = Vector3.Dot(mainCamTransform.up * -1, EntityPathNavigation.NormalizedMoveDirection);
        currentDirection = Mathf.Sign(dotProductCamAndDirection);
        remappedTolerance = Remap(dotProductCamAndDirection, tolerance * currentDirection, 1f * currentDirection, 0f, 1f * currentDirection);

        EntityPathNavigation.Instance.UpdateNavigationSpeed(canMove ? Mathf.Abs(remappedTolerance) : 0);
        EntityPathNavigation.CurrentNavigationState = (NavigationState)currentDirection;

        // changing from leaf or root
        if (currentDirection != previousDirection)
        {
            previousDirection = currentDirection;
            GameLogger.Log("INVERTING EVENT");
            EntityPathNavigation.Instance.UpdateOnDirectionChange();
        } 

        // within certain angle forward of backward
        // When Changing midway
        /*if (canMove)
        {
            // DONE ONCE
            if (EntityPathNavigation.CurrentNavigationState != EntityPathNavigation.PreviousNavigationState)
            {
                GameLogger.Log("INVERTING EVENT");
                EntityPathNavigation.Instance.UpdateOnDirectionChange();
            }
        } */
    }

    private float Remap(float value, float from1, float to1, float from2, float to2) 
        => (value - from1) / (to1 - from1) * (to2 - from2) + from2; 

    private bool IsBetweenMinAndMax(float value, float min, float max) => value >= min && value <= max; 
}
