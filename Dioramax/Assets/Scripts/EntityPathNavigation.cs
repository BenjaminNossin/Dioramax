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

            Instantiate(previousDestination, pointsAlongPath[subDestinationIndex] + new Vector3(0f, 0.1f, 0f), Quaternion.identity); 
             
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
            
            Instantiate(nextDestination, pointsAlongPath[subDestinationIndex] + new Vector3(0f, 0.1f, 0f), Quaternion.identity);
            Debug.Break(); 
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
    private int currentNodeIndex, targetNodeIndex; 
    private void GetNewPointsOnReachingDestinationNode()
    {
        pointsAlongPath = PathController.Instance.GetPointsAlongPathBetweenNodes(pathNodes[currentNodeIndex],
            pathNodes[currentNodeIndex].GetNextActiveNode(), ref pointsAlongPath);

        for (int i = 0; i < pointsAlongPath.Length; i++)
        {
            Instantiate(debugObject, pointsAlongPath[i], Quaternion.identity);
        } 
    }

    private int subDestinationIndex;
    private Vector3 SubDestination;
    private float distanceFromNextSubNode;
    private Vector3 normalizedMoveDirection;

    private void InvertArray(Vector3[] array)
    {
        Debug.Log("INVERTING");
        Array.Reverse(array);

        // subDestinationIndex = 
    }

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
        normalizedMoveDirection = (pointsAlongPath[subDestinationIndex] - pointsAlongPath[subDestinationIndex - 1]).normalized;

        /* if (subDestinationIndex != 0)
        {
            normalizedMoveDirection = (pointsAlongPath[subDestinationIndex] - pointsAlongPath[subDestinationIndex - 1]).normalized;
        }
        else
        {
            Debug.Log("subDest index was 0"); 
            normalizedMoveDirection = (pointsAlongPath[subDestinationIndex + 1] - pointsAlongPath[subDestinationIndex]).normalized;
        } */
    }

    // distance between two points on a segment
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

    private float angleBetweenStartAndTargedNode; // hardcoded placeholder 
    // duration = 6
    private float framesToReachTargetNode = 300; // 50 * (1/NavigationSpeedMultiplier)
                                                 // private
    private void MoveEntityAlongPath()
    {
        Debug.DrawRay(entityToMoveTransform.position, normalizedMoveDirection * 5f, Color.blue); 
        entityToMoveTransform.position += Time.fixedDeltaTime * navigationSpeedMultiplier * normalizedMoveDirection;

        // entityToMoveTransform.LookAt(new Vector3(SubDestination.x, entityToMoveTransform.position.y, SubDestination.z) * (hasInverted ? -1 : 1)); 

        //Debug.Break(); 
    }

    // distance from target node (end of current segment)
    private void CheckMacroDistance()
    {

    }

    // need rework. This will become CheckMacroDistance
    private void CheckDistanceFromNextNode()
    {
        if (Vector3.Distance(transform.position, lastNodePosition) < SNAP_VALUE &&
            destinationNodeIndex == pathNodes.Length - 1 && !loopPath) return; // arrived at the end of path

        distanceFromNextNode = Vector3.Distance(transform.position, nextNodePosition);

        // enter here only once
        if (distanceFromNextNode < SNAP_VALUE)
        {
            GameLogger.Log("snapping to current node");
            transform.position = new Vector3(pathNodes[destinationNodeIndex].GetNodePosition().x, transform.position.y, pathNodes[destinationNodeIndex].GetNodePosition().z);
            // SetNextDestination(); // this is called too often
        }
    }

    // when you reached target of macroDistance
    private void SetNextDestination()
    {
        if (nodeArraySize == 0)
            return;

        GameLogger.Log("setting next destination node");

        destinationNodeIndex = (destinationNodeIndex + 1) % nodeArraySize;
        nextNodePosition = pathNodes[destinationNodeIndex].GetNodePosition();

        transform.LookAt(nextNodePosition);
    }

    // DEBUG for feature IV
    /* public void GoToPreviousPoint()
    {
        if (entityPathController.Points.Length == 0)
            return;

        DestPoint = (DestPoint - 1) % entityPathController.Points.Length;
        destinationDirection = (entityPathController.Nodes[DestPoint].position - transform.position).normalized;
        transform.LookAt(entityPathController.Nodes[DestPoint].position);
    } */
}
