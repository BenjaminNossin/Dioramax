using UnityEngine;
using System;
using System.Collections; 

// makes an entity move along a path
public enum NavigationState { NONE, Forward = 1, Backward = -1 }
public class EntityPathNavigation : MonoBehaviour
{
    [SerializeField] private PathController pathController;
    [SerializeField] private Transform entityToMoveTransform;
    [SerializeField] private Transform[] initialNodesDebugArray;
    [SerializeField, Range(0f, 2f)] private float navigationSpeedMultiplier = 1f;
    private float _navigationSpeedMultiplier; 

    private PathNode[] pathNodes;
    private Vector3 lastNodePosition; // reached the end of track

    private const float SNAP_VALUE = 0.05f;
    private Vector3[] pointsAlongPath;

    private int lastVisitedPointOnSegmentIndex;

    public static NavigationState PreviousNavigationState { get; set; }
    public static NavigationState CurrentNavigationState { get; set; }
    private bool overrideLastVisitedSegment = true;

    [Header("--DEBUG--")]
    [SerializeField] private GameObject debugObject;
    [SerializeField] private GameObject previousDestination;
    [SerializeField] private GameObject nextDestination;
    [SerializeField] private int overriddenStartingNodeIndex;
    [SerializeField] private bool showDebugPoints;
    [SerializeField] private bool showSubDestination;

    private GameObject debugObjReference;
    private GameObject[] debugObject_PathPoints;
    public bool simulateMovement;
    public bool simulateBackward;
    public static bool SimulateMovement; 

    private void Awake()
    {
        CurrentNavigationState = PreviousNavigationState;
        SimulateMovement = simulateMovement; 
    }

    void Start()
    {
        if (showDebugPoints)
        {
            debugObject_PathPoints = new GameObject[PathController.Resolution];
            for (int i = 0; i < debugObject_PathPoints.Length; i++)
            {
                debugObjReference = Instantiate(debugObject, Vector3.zero, Quaternion.identity);
                debugObject_PathPoints[i] = debugObjReference;
            }
        }

        pathNodes = new PathNode[pathController.GetPathNodes().Length]; 
        pathController.GetPathNodes().CopyTo(pathNodes, 0);
        // lastNodePosition = pathNodes[^1].GetNodePosition();

        startingNodeIndex = overriddenStartingNodeIndex;
        destinationNodeIndex = pathNodes[startingNodeIndex].GetNextActiveNodeIndex();

        pointsAlongPath = new Vector3[PathController.Resolution];

        Init(); 
    }

    private Vector3 lastVisitedPointOnSegmentPosition;
    void Update()
    {
        if (!simulateMovement)
        {
            if (CurrentNavigationState == NavigationState.Backward)
            {
                AbsoluteForwardDirection = NormalizedRequiredDirection * -1;
                lookAtDirection = AbsoluteForwardDirection;

                GameDrawDebugger.DrawRay(entityToMoveTransform.position, lookAtDirection * 5f, Color.cyan);
            }

            if (destinationNodeIndex != -1 && !waitForNextFixedUpdate && _navigationSpeedMultiplier >= 0.05f)
            {
                CheckMicroDistance();
                MoveEntityAlongPath();
            }
        }
        else // TEMPORARY DEBUG
        {
            CurrentNavigationState = simulateBackward ? NavigationState.Backward : NavigationState.Forward;
            _navigationSpeedMultiplier = 1f; 

            if (CurrentNavigationState == NavigationState.Backward)
            {
                AbsoluteForwardDirection = NormalizedRequiredDirection * -1;
                lookAtDirection = AbsoluteForwardDirection;

                GameDrawDebugger.DrawRay(entityToMoveTransform.position, lookAtDirection * 5f, Color.cyan);
            }

            if (destinationNodeIndex != -1 && !waitForNextFixedUpdate && _navigationSpeedMultiplier >= 0.05f)
            {
                CheckMicroDistance();
                MoveEntityAlongPath();
            }
        }

        GameDrawDebugger.DrawRay(entityToMoveTransform.position, NormalizedRequiredDirection * 5f, Color.blue);
    }

    private void Init()
    {
        GetNewPointsOnReachingDestinationNode();
        entityToMoveTransform.position = new Vector3(pointsAlongPath[0].x, entityToMoveTransform.position.y, pointsAlongPath[0].z);

        SetSubDestination();
    }

    public void UpdateOnDirectionChange()
    {
        if (CurrentNavigationState == NavigationState.Backward)
        {
            // here, I know my startingNodeIndex, but destination == -1
            if (destinationNodeIndex == -1)
            {
                // GameLogger.Log($"previous starting node index (BACKWARD): {startingNodeIndex}");
                // GameLogger.Log($"previous destination node index (BACKWARD): {destinationNodeIndex}");

                destinationNodeIndex = pathNodes[startingNodeIndex].GetPreviousNodeIndex();
                // Debug.Break();
                StoreLastVisitedPointOnSegmentPosition();
                SetInversionState();

                // GameLogger.Log($"current starting node index (BACKWARD): {startingNodeIndex}");
                // GameLogger.Log($"current destination node index (BACKWARD): {destinationNodeIndex}");
            }
            else
            {
                // GameLogger.Log($"previous starting node index (BACKWARD): {startingNodeIndex}");
                // GameLogger.Log($"previous destination node index (BACKWARD): {destinationNodeIndex}");

                startingNodeIndex = destinationNodeIndex;
                destinationNodeIndex = pathNodes[startingNodeIndex].GetPreviousNodeIndex();

                StoreLastVisitedPointOnSegmentPosition();
                SetInversionState();

                // GameLogger.Log($"current starting node index (BACKWARD): {startingNodeIndex}");
                // GameLogger.Log($"current destination node index (BACKWARD): {destinationNodeIndex}");
            }
        }
        else if (CurrentNavigationState == NavigationState.Forward)
        {
            // coming from root node 
            if (destinationNodeIndex == -1 || PreviousNavigationState == NavigationState.NONE)
            {
                // GameLogger.Log($"previous starting node index (FORWARD): {startingNodeIndex}");
                // GameLogger.Log($"previous destination node index (FORWARD): {destinationNodeIndex}");

                destinationNodeIndex = pathNodes[startingNodeIndex].GetNextActiveNodeIndex();

                if (pathNodes[startingNodeIndex].IsRoot())
                {
                    Init();
                }

                // GameLogger.Log($"previous starting node index (FORWARD): {startingNodeIndex}");
                // GameLogger.Log($"previous destination node index (FORWARD): {destinationNodeIndex}");
            }
            else
            {
                // GameLogger.Log($"previous starting node index (FORWARD): {startingNodeIndex}");
                // GameLogger.Log($"previous destination node index (FORWARD): {destinationNodeIndex}");

                startingNodeIndex = destinationNodeIndex;
                destinationNodeIndex = pathNodes[startingNodeIndex].GetNextActiveNodeIndex();

                StoreLastVisitedPointOnSegmentPosition();
                SetInversionState();

                // GameLogger.Log($"previous starting node index (FORWARD): {startingNodeIndex}");
                // GameLogger.Log($"previous destination node index (FORWARD): {destinationNodeIndex}");
            }
        }

        PreviousNavigationState = CurrentNavigationState;
    }

    private void StoreLastVisitedPointOnSegmentPosition()
    {
        overrideLastVisitedSegment = false;
        lastVisitedPointOnSegmentPosition = new Vector3(pointsAlongPath[lastVisitedPointOnSegmentIndex].x,
                                                        pointsAlongPath[lastVisitedPointOnSegmentIndex].y,
                                                        pointsAlongPath[lastVisitedPointOnSegmentIndex].z);
    }

    private void SetInversionState(bool calledFromReachingDestinationNode = false)
    {
        InvertArray(pointsAlongPath);

        if (!calledFromReachingDestinationNode)
        {

            // to find what is the new index of subDestination
            for (int i = 0; i < pointsAlongPath.Length; i++)
            {
                if (Vector3.Distance(lastVisitedPointOnSegmentPosition, pointsAlongPath[i]) <= SNAP_VALUE)
                {
                    lastVisitedPointOnSegmentIndex = i;
                }
            }

            StartCoroutine(DelayNextFixedUpdate());
            SetSubDestination();
        }

        // Debug.Break(); 
    }

    private bool waitForNextFixedUpdate; 
    private IEnumerator DelayNextFixedUpdate()
    {
        waitForNextFixedUpdate = true; 
        yield return new WaitForFixedUpdate();
        waitForNextFixedUpdate = false; 
    }

    // called on Start and every time you reach target node
    private int destinationNodeIndex, startingNodeIndex; 
    private void GetNewPointsOnReachingDestinationNode()
    {
        if (CurrentNavigationState == NavigationState.Backward)
        {
            // terrible code duplication
            pointsAlongPath = pathController.GetPointsAlongPathBetweenNodes(pathNodes[startingNodeIndex],
                pathNodes[destinationNodeIndex], ref pointsAlongPath, true);

            SetInversionState(true); 
        }
        else 
        {
            pointsAlongPath = pathController.GetPointsAlongPathBetweenNodes(pathNodes[startingNodeIndex],
                pathNodes[destinationNodeIndex], ref pointsAlongPath, false);
        }      

        if (showDebugPoints)
        {
            for (int i = 0; i < pointsAlongPath.Length; i++)
            {
                debugObject_PathPoints[i].transform.position = pointsAlongPath[i];
            }
        }
    }

    private int subDestinationIndex;
    private Vector3 SubDestination;
    private float distanceFromNextSubNode;
    public static Vector3 NormalizedRequiredDirection { get; private set; }
    private void SetSubDestination()
    {
        if (pointsAlongPath.Length == 0)
            return;

        if (overrideLastVisitedSegment)
        {
            lastVisitedPointOnSegmentIndex = subDestinationIndex;
        }

        overrideLastVisitedSegment = true;
        subDestinationIndex = (lastVisitedPointOnSegmentIndex + 1) % pointsAlongPath.Length;
        SubDestination = pointsAlongPath[subDestinationIndex];

        if (subDestinationIndex != 0)
        {
            NormalizedRequiredDirection = (pointsAlongPath[subDestinationIndex] - pointsAlongPath[subDestinationIndex - 1]).normalized;
        }
        /* else
        {
            NormalizedRequiredDirection = (pointsAlongPath[subDestinationIndex + 1] - pointsAlongPath[subDestinationIndex]).normalized;
        } */
    }

    private void InvertArray(Vector3[] array)
    {
        Array.Reverse(array);
    }

    // avoid calling BOTH this and the update
    private void CheckMicroDistance()
    {
        distanceFromNextSubNode = Vector3.Distance(entityToMoveTransform.position, new Vector3(
                                                                                    SubDestination.x, 
                                                                                    entityToMoveTransform.position.y, 
                                                                                    SubDestination.z));

        if (distanceFromNextSubNode <= SNAP_VALUE)
        {
            if (subDestinationIndex < PathController.Resolution - 1)
            {
                    entityToMoveTransform.position = new Vector3(pointsAlongPath[subDestinationIndex].x, 
                                                                 entityToMoveTransform.position.y, 
                                                                 pointsAlongPath[subDestinationIndex].z);
            }
            else // arrived at the end of path
            {
                if (CurrentNavigationState == NavigationState.Backward)
                {
                    // GameLogger.Log($"previous starting node index (BACKWARD): {startingNodeIndex}");
                    // GameLogger.Log($"previous destination node index (BACKWARD): {destinationNodeIndex}");

                    startingNodeIndex = destinationNodeIndex;
                    destinationNodeIndex = pathNodes[startingNodeIndex].GetPreviousNodeIndex();

                    // GameLogger.Log($"current starting node index (BACKWARD): {startingNodeIndex}");
                    // GameLogger.Log($"current destination node index (BACKWARD): {destinationNodeIndex}");
                }
                else if (CurrentNavigationState == NavigationState.Forward)
                {
                    // GameLogger.Log($"previous starting node index (FORWARD): {startingNodeIndex}");
                    // GameLogger.Log($"previous destination node index (FORWARD): {destinationNodeIndex}");

                    startingNodeIndex = destinationNodeIndex;
                    destinationNodeIndex = pathNodes[startingNodeIndex].GetNextActiveNodeIndex();

                    // GameLogger.Log($"current starting node index (FORWARD): {startingNodeIndex}");
                    // GameLogger.Log($"current destination node index (FORWARD): {destinationNodeIndex}");
                }

                // Debug.Break();

                if (destinationNodeIndex != -1)
                {
                    GetNewPointsOnReachingDestinationNode();

                    // repositions the entity at the start of path
                    entityToMoveTransform.position = new Vector3(pointsAlongPath[0].x, entityToMoveTransform.position.y, pointsAlongPath[0].z);
                }
            }

            if (destinationNodeIndex != -1)
            {
                SetSubDestination();
            }
        }
    }

    public void UpdateNavigationSpeed(float lerpedValue = 1f)
    {
        _navigationSpeedMultiplier = navigationSpeedMultiplier * lerpedValue;
    }

    private Vector3 lookAtDirection;
    public static Vector3 AbsoluteForwardDirection { get; private set; }
    private void MoveEntityAlongPath()
    {
        entityToMoveTransform.position += Time.fixedDeltaTime * _navigationSpeedMultiplier * NormalizedRequiredDirection;
        lookAtDirection = new Vector3(SubDestination.x, entityToMoveTransform.position.y, SubDestination.z);
        // GameDrawDebugger.DrawRay(entityToMoveTransform.position, lookAtDirection * 5f, Color.cyan);

        if (distanceFromNextSubNode >= SNAP_VALUE)
        {
            entityToMoveTransform.LookAt(lookAtDirection);
        }
    }
}
