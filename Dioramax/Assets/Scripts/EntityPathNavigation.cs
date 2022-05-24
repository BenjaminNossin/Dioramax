using UnityEngine;
using System;
using System.Collections; 

// makes an entity move along a path
public class EntityPathNavigation : MonoBehaviour
{
    public static EntityPathNavigation Instance;

    [SerializeField] private Transform entityToMoveTransform;
    [SerializeField] private bool loopPath;
    [SerializeField] private Transform[] initialNodesDebugArray;
    [SerializeField, Range(0f, 2f)] private float navigationSpeedMultiplier = 1f;
    private float _navigationSpeedMultiplier; 

    private PathNode[] pathNodes;
    private Vector3 lastNodePosition; // reached the end of track

    private const float SNAP_VALUE = 0.05f;
    private Vector3[] pointsAlongPath;

    private int lastVisitedPointOnSegmentIndex;

    public static bool InvertDirection { get; set; }
    public static bool IsInverted { get; set; }
    private bool overrideLastVisitedSegment = true;

    [Header("--DEBUG--")]
    [SerializeField] private GameObject debugObject;
    [SerializeField] private GameObject previousDestination;
    [SerializeField] private GameObject nextDestination;
    private GameObject[] debugObject_PathPoints;
    private GameObject debugObjReference;
    public int overriddenStartingNodeIndex;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this;
        IsInverted = InvertDirection; 
    }

    void Start()
    {
        debugObject_PathPoints = new GameObject[PathController.Resolution];
        for (int i = 0; i < debugObject_PathPoints.Length; i++)
        {
            debugObjReference = Instantiate(debugObject, Vector3.zero, Quaternion.identity);
            debugObject_PathPoints[i] = debugObjReference;
        }

        pathNodes = new PathNode[PathController.Instance.GetPathNodes().Length]; 
        PathController.Instance.GetPathNodes().CopyTo(pathNodes, 0);
        lastNodePosition = pathNodes[^1].GetNodePosition();

        startingNodeIndex = overriddenStartingNodeIndex;
        destinationNodeIndex = pathNodes[startingNodeIndex].GetNextActiveNodeIndex();

        pointsAlongPath = new Vector3[PathController.Resolution];

        Init(); 
        // SetNextDestination();
    }

    private Vector3 lastVisitedPointOnSegmentPosition;
    void Update()
    {
        GameDrawDebugger.DrawRay(entityToMoveTransform.position, NormalizedMoveDirection * 5f, Color.blue);
        if (destinationNodeIndex != -1 && !waitForNextFixedUpdate && _navigationSpeedMultiplier >= 0.05f)
        {
            CheckMicroDistance();
            MoveEntityAlongPath();
        }
    }

    public void UpdateOnDirectionChange()
    {
        IsInverted = InvertDirection;
        if (IsInverted)
        {
            // here, I know my startingNodeIndex, but destination == -1
            if (destinationNodeIndex == -1)
            {
                // cameFromLeafNode = true; 
                destinationNodeIndex = pathNodes[startingNodeIndex].GetPreviousNodeIndex();
                // Debug.Break();
                StoreLastVisitedPointOnSegmentPosition();
                SetInversionState();
            }
            else
            {
                startingNodeIndex = destinationNodeIndex;
                destinationNodeIndex = pathNodes[startingNodeIndex].GetPreviousNodeIndex();

                StoreLastVisitedPointOnSegmentPosition();
                SetInversionState();
            }
        }
        else
        {
            // coming from root node
            if (destinationNodeIndex == -1)
            {
                destinationNodeIndex = pathNodes[startingNodeIndex].GetNextActiveNodeIndex();

                if (pathNodes[startingNodeIndex].IsRoot())
                {
                    Init();
                }
            }
            else
            {
                startingNodeIndex = destinationNodeIndex;
                destinationNodeIndex = pathNodes[startingNodeIndex].GetNextActiveNodeIndex();

                StoreLastVisitedPointOnSegmentPosition();
                SetInversionState();
            }
        }
    }

    private void Init()
    {
        GetNewPointsOnReachingDestinationNode();
        entityToMoveTransform.position = new Vector3(pointsAlongPath[0].x, entityToMoveTransform.position.y, pointsAlongPath[0].z);

        SetSubDestination();
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
        if (IsInverted)
        {
            // terrible code duplication
            pointsAlongPath = PathController.Instance.GetPointsAlongPathBetweenNodes(pathNodes[startingNodeIndex],
                pathNodes[destinationNodeIndex], ref pointsAlongPath, IsInverted);

            SetInversionState(true); 
        }
        else
        {
            pointsAlongPath = PathController.Instance.GetPointsAlongPathBetweenNodes(pathNodes[startingNodeIndex],
                pathNodes[destinationNodeIndex], ref pointsAlongPath, IsInverted);
        }      

        for (int i = 0; i < pointsAlongPath.Length; i++)
        {
            debugObject_PathPoints[i].transform.position = pointsAlongPath[i];
        } 
    }

    private int subDestinationIndex;
    private Vector3 SubDestination;
    private float distanceFromNextSubNode;
    public static Vector3 NormalizedMoveDirection { get; private set; }
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
            NormalizedMoveDirection = (pointsAlongPath[subDestinationIndex] - pointsAlongPath[subDestinationIndex - 1]).normalized;
        }
        else
        {
            NormalizedMoveDirection = (pointsAlongPath[subDestinationIndex + 1] - pointsAlongPath[subDestinationIndex]).normalized;
        }
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
                if (IsInverted)
                {
                    startingNodeIndex = destinationNodeIndex;
                    destinationNodeIndex = pathNodes[startingNodeIndex].GetPreviousNodeIndex();
                }
                else
                {
                    GameLogger.Log("setting destination node on reaching end"); 
                    startingNodeIndex = destinationNodeIndex;
                    destinationNodeIndex = pathNodes[startingNodeIndex].GetNextActiveNodeIndex();
                }

                // Debug.Break();

                if (destinationNodeIndex != -1 )
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

    private void MoveEntityAlongPath()
    {
        Debug.Log("moving"); 
        entityToMoveTransform.position += Time.fixedDeltaTime * _navigationSpeedMultiplier * NormalizedMoveDirection;

        // entityToMoveTransform.LookAt(new Vector3(SubDestination.x, entityToMoveTransform.position.y, SubDestination.z) * (hasInverted ? -1 : 1)); 
    }

}
