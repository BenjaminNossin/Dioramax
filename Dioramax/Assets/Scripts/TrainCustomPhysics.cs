using UnityEngine;


public class TrainCustomPhysics : MonoBehaviour
{
    [SerializeField] private Transform mainCamTransform;
    [SerializeField, Range(0.05f, 2f)] private float gravityForceMultiplier = 1f;
    [SerializeField, Range(0.4f, 1f)] private float tolerance = 0.65f; 

    private float dotGravityAndRequiredDirection;
    private float remappedTolerance;

    private bool changeDoneOnce, canMove;
    private bool initDoneOnce;

    private float dotGravityAndTrainForward;
    private float currentDirection, previousDirection;
    private void Start()
    {
        currentDirection = previousDirection = -1;
    }

    void Update()
    {
        if (EntityPathNavigation.SimulateMovement) return; 
        UpdateEntities();
    }

    private void UpdateEntities()
    {
        // going forward for the first time
        if (currentDirection == 1 && !initDoneOnce)
        {
            previousDirection = 1; 
        }

        GameDrawDebugger.DrawRay(transform.position, mainCamTransform.up * -5, Color.red);

        dotGravityAndRequiredDirection = Vector3.Dot(mainCamTransform.up * -1, EntityPathNavigation.NormalizedRequiredDirection); // 1 (~ALWAYS), 0 (DO ONCE)
        canMove = IsBetweenMinAndMax(dotGravityAndRequiredDirection, tolerance, 1f);

        currentDirection = Mathf.Sign(dotGravityAndRequiredDirection);
        remappedTolerance = Remap(dotGravityAndRequiredDirection, tolerance * currentDirection, 1f * currentDirection, 0f, 1f * currentDirection);
        EntityPathNavigation.Instance.UpdateNavigationSpeed(canMove ? Mathf.Abs(remappedTolerance) : 0);

        dotGravityAndTrainForward = Vector3.Dot(transform.forward, mainCamTransform.up * -1); // indicates when moving backward
        dotGravityAndTrainForward = Mathf.Sign(dotGravityAndTrainForward);

        if (IsBetweenMinAndMax(dotGravityAndRequiredDirection, tolerance, 1f, true) && !changeDoneOnce)
        {
            changeDoneOnce = true;
            EntityPathNavigation.CurrentNavigationState = (NavigationState)currentDirection;
            GameLogger.Log($"setting navigation state to {EntityPathNavigation.CurrentNavigationState}");
        }
        else if (!IsBetweenMinAndMax(dotGravityAndRequiredDirection, tolerance, 1f, true) && changeDoneOnce)
        {
            changeDoneOnce = false;
            EntityPathNavigation.CurrentNavigationState = NavigationState.NONE;
            GameLogger.Log($"setting navigation state to {EntityPathNavigation.CurrentNavigationState}");
        }

        // changing from leaf or root
        if (currentDirection == -1 && previousDirection == 1)
        {
            EntityPathNavigation.Instance.UpdateOnDirectionChange();
        }
    }

    private float Remap(float value, float from1, float to1, float from2, float to2) 
        => (value - from1) / (to1 - from1) * (to2 - from2) + from2; 

    private bool IsBetweenMinAndMax(float value, float min, float max, bool absValue = false) => absValue ? 
                                                                                                 Mathf.Abs(value) >= min && value <= max : 
                                                                                                           value >= min && value <= max; 
}
