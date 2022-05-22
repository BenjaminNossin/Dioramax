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
    [SerializeField, Range(0.25f, 2f)] private float navigationSpeedMultiplier = 1f;

    private PathNode[] pathNodes;
    private float distanceFromNextNode;
    private int nodeArraySize;
    private Vector3 nextNodePosition, lastNodePosition; 

    private const float SNAP_VALUE = 0.05f;
    private Vector3[] pointsAlongPath;

    private int lastVisitedPointOnSegmentIndex; 

    [Header("--DEBUG--")]
    [SerializeField] private GameObject debugObject;
    [SerializeField] private GameObject previousDestination;
    [SerializeField] private GameObject nextDestination;
    private GameObject[] debugObject_PathPoints;
    private GameObject debugObjReference; 

    public bool invertDirection; 
    private bool isInverted; //
    private bool overrideLastVisitedSegment = true; 

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this;
        isInverted = invertDirection; 
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
        nodeArraySize = PathController.Instance.GetNodeArraySize();
        lastNodePosition = pathNodes[^1].GetNodePosition();

        startingNodeIndex = 0;
        destinationNodeIndex = pathNodes[startingNodeIndex].GetNextActiveNodeIndex();

        pointsAlongPath = new Vector3[PathController.Resolution];

        GetNewPointsOnReachingDestinationNode();
        entityToMoveTransform.position = new Vector3(pointsAlongPath[0].x, entityToMoveTransform.position.y, pointsAlongPath[0].z);

        SetSubDestination(); 
        // SetNextDestination();
    }

    private Vector3 lastVisitedPointOnSegmentPosition;
    private int tempNodeIndex; 
    private void Update()
    {
        if (invertDirection != isInverted)
        {
            isInverted = invertDirection;
            if (!isInverted)
            {
                GameLogger.Log($"previous starting node (INVERTED to DEFAULT): {startingNodeIndex}");
                GameLogger.Log($"previous destination node (INVERTED to DEFAULT): {destinationNodeIndex}");

                startingNodeIndex = destinationNodeIndex;
                destinationNodeIndex = pathNodes[startingNodeIndex].GetNextActiveNodeIndex();

                GameLogger.Log($"current starting node (INVERTED to DEFAULT): {startingNodeIndex}");
                GameLogger.Log($"current destination node (INVERTED to DEFAULT): {destinationNodeIndex}");
            }

            StoreLastVisitedPointOnSegmentPosition();
            SetInversionState();          
        }
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

    void FixedUpdate()
    {
        if (destinationNodeIndex != -1 && !waitForNextFixedUpdate)
        {
            CheckMicroDistance();
            MoveEntityAlongPath();
        }
    }

    // called on Start and every time you reach target node
    private int destinationNodeIndex, startingNodeIndex; 
    private void GetNewPointsOnReachingDestinationNode()
    {
        if (isInverted)
        {
            // terrible code duplication
            pointsAlongPath = PathController.Instance.GetPointsAlongPathBetweenNodes(pathNodes[startingNodeIndex],
                pathNodes[destinationNodeIndex], ref pointsAlongPath, isInverted);

            SetInversionState(true); 
        }
        else
        {
            pointsAlongPath = PathController.Instance.GetPointsAlongPathBetweenNodes(pathNodes[startingNodeIndex],
                pathNodes[destinationNodeIndex], ref pointsAlongPath, isInverted);
        }      

        for (int i = 0; i < pointsAlongPath.Length; i++)
        {
            debugObject_PathPoints[i].transform.position = pointsAlongPath[i];
        } 
    }

    private int subDestinationIndex;
    private Vector3 SubDestination;
    private float distanceFromNextSubNode;
    private Vector3 normalizedMoveDirection;
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
            normalizedMoveDirection = (pointsAlongPath[subDestinationIndex] - pointsAlongPath[subDestinationIndex - 1]).normalized;
        }
        else
        {
            normalizedMoveDirection = (pointsAlongPath[subDestinationIndex + 1] - pointsAlongPath[subDestinationIndex]).normalized;
        }
    }

    private void InvertArray(Vector3[] array)
    {
        Array.Reverse(array);
    }

    private void CheckMicroDistance()
    {
        distanceFromNextSubNode = Vector3.Distance(entityToMoveTransform.position, new Vector3(
                                                                                    SubDestination.x, 
                                                                                    entityToMoveTransform.position.y, 
                                                                                    SubDestination.z));

        /* if (hasInverted)
        {
            GameLogger.Log($"distance: {distanceFromNextSubNode}");
            Debug.Break(); 
        } */

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
                Debug.Break();

                GameLogger.Log($"previous starting node: {startingNodeIndex}");
                GameLogger.Log($"previous destination node: {destinationNodeIndex}");

                if (isInverted)
                {
                    destinationNodeIndex = pathNodes[startingNodeIndex].GetPreviousNodeIndex();
                }
                else
                {
                    startingNodeIndex = destinationNodeIndex;
                    destinationNodeIndex = pathNodes[startingNodeIndex].GetNextActiveNodeIndex();
                }

                GameLogger.Log($"current starting node: {startingNodeIndex}");
                GameLogger.Log($"current destination node: {destinationNodeIndex}");

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

    private void MoveEntityAlongPath()
    {
        Debug.DrawRay(entityToMoveTransform.position, normalizedMoveDirection * 5f, Color.blue); 
        entityToMoveTransform.position += Time.fixedDeltaTime * navigationSpeedMultiplier * normalizedMoveDirection;

        // entityToMoveTransform.LookAt(new Vector3(SubDestination.x, entityToMoveTransform.position.y, SubDestination.z) * (hasInverted ? -1 : 1)); 

        //Debug.Break(); 
    }

}
