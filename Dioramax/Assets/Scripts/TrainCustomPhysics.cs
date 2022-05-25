using UnityEngine;


public class TrainCustomPhysics : MonoBehaviour
{
    [SerializeField] private Transform mainCamTransform;
    [SerializeField, Range(0.05f, 2f)] private float gravityForceMultiplier = 1f;
    [SerializeField, Range(0.4f, 1f)] private float tolerance = 0.65f; 

    private float dotGravityAndRequiredDirection;
    private float remappedTolerance;

    private bool changeIsDone, canMove;
    private bool doneOnce;

    private float dotGravityAndTrainForward; 
    public static float currentDirection, previousDirection;
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
        // going forward for the first time
        if (currentDirection == 1 && !doneOnce)
        {
            previousDirection = 1; 
        }

        GameDrawDebugger.DrawRay(transform.position, mainCamTransform.up * -5, Color.red);

        canMove = IsBetweenMinAndMax(dotGravityAndRequiredDirection, tolerance, 1f); 

        dotGravityAndRequiredDirection = Vector3.Dot(mainCamTransform.up * -1, EntityPathNavigation.NormalizedRequiredDirection); // 1 (~ALWAYS), 0 (DO ONCE)
        currentDirection = Mathf.Sign(dotGravityAndRequiredDirection);
        remappedTolerance = Remap(dotGravityAndRequiredDirection, tolerance * currentDirection, 1f * currentDirection, 0f, 1f * currentDirection);
        EntityPathNavigation.Instance.UpdateNavigationSpeed(canMove ? Mathf.Abs(remappedTolerance) : 0);

        dotGravityAndTrainForward = Vector3.Dot(transform.forward, mainCamTransform.up * -1); // indicates when moving backward
        dotGravityAndTrainForward = Mathf.Sign(dotGravityAndTrainForward);
        EntityPathNavigation.CurrentNavigationState = (NavigationState)currentDirection; 

        // Debug.Break(); 

        // changing from leaf or root
        if (currentDirection == -1 && previousDirection == 1)
        {
            GameLogger.Log("INVERTING EVENT");
            EntityPathNavigation.Instance.UpdateOnDirectionChange();
        }
    }

    private float Remap(float value, float from1, float to1, float from2, float to2) 
        => (value - from1) / (to1 - from1) * (to2 - from2) + from2; 

    private bool IsBetweenMinAndMax(float value, float min, float max) => value >= min && value <= max; 
}
