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
    private int destinationNodeIndex;
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
    private bool hasInverted; //
    private bool overrideLastVisitedSegment = true; 

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this;
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

        pointsAlongPath = new Vector3[PathController.Resolution];
        currentNodeIndex = 0; 

        GetNewPointsOnReachingDestinationNode();
        entityToMoveTransform.position = new Vector3(pointsAlongPath[0].x, entityToMoveTransform.position.y, pointsAlongPath[0].z);

        SetSubDestination(); 
        // SetNextDestination();
    }

    private Vector3 lastVisitedPointOnSegmentPosition; 
    private void Update()
    {
        if (invertDirection != hasInverted)
        {
            overrideLastVisitedSegment = false; 
            lastVisitedPointOnSegmentPosition = new Vector3(pointsAlongPath[lastVisitedPointOnSegmentIndex].x,
                                                            pointsAlongPath[lastVisitedPointOnSegmentIndex].y,
                                                            pointsAlongPath[lastVisitedPointOnSegmentIndex].z);

            // Instantiate(previousDestination, pointsAlongPath[subDestinationIndex] + new Vector3(0f, 0.1f, 0f), Quaternion.identity); 
             
            InvertArray(pointsAlongPath); 
            hasInverted = invertDirection;

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
            
            // Instantiate(nextDestination, pointsAlongPath[subDestinationIndex] + new Vector3(0f, 0.1f, 0f), Quaternion.identity);
            // Debug.Break(); 
        }
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
        if (currentNodeIndex != -1 && !waitForNextFixedUpdate)
        {
            CheckMicroDistance();
            MoveEntityAlongPath();
        }
    }

    // called on Start and every time you reach target node
    private int currentNodeIndex, previousNodeIndex; 
    private void GetNewPointsOnReachingDestinationNode()
    {
        if (hasInverted)
        {
            currentNodeIndex = previousNodeIndex;

            // terrible code duplication
            pointsAlongPath = PathController.Instance.GetPointsAlongPathBetweenNodes(pathNodes[currentNodeIndex],
                pathNodes[currentNodeIndex].GetPreviousNode(), ref pointsAlongPath, hasInverted);
        }
        else
        {
            pointsAlongPath = PathController.Instance.GetPointsAlongPathBetweenNodes(pathNodes[currentNodeIndex],
                pathNodes[currentNodeIndex].GetNextActiveNode(), ref pointsAlongPath, hasInverted);
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

        GameLogger.Log("setting sub destination");

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
        Debug.Log("INVERTING");
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
                previousNodeIndex = currentNodeIndex; 
                currentNodeIndex = pathNodes[currentNodeIndex].GetNextActiveNodeIndex();

                if (currentNodeIndex != -1 )
                {
                    GetNewPointsOnReachingDestinationNode();
                    entityToMoveTransform.position = new Vector3(pointsAlongPath[0].x, entityToMoveTransform.position.y, pointsAlongPath[0].z);
                }
            }

            if (currentNodeIndex != -1)
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
