using UnityEngine;
using DG.Tweening;

// makes an entity move along a path
public class EntityPathNavigation : MonoBehaviour
{
    public static EntityPathNavigation Instance;

    [SerializeField] private Transform entityToMove;
    [SerializeField] private bool loopPath;
    [SerializeField] private Transform[] initialNodesDebugArray;
    private PathNode[] currentAndNextNode; 

    private PathNode[] pathNodes;
    private int destinationNodeIndex;
    private float distanceFromNextNode;
    private int nodeArraySize;
    private Vector3 nextNodePosition, lastNodePosition; 

    private const float SNAP_VALUE = 0.05f; 

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
        /* transform.position = PathController.Instance.GetNodePosition(0);
        GotoNextPoint(); */
        // bad to call GetPathNodes() twice
        pathNodes = new PathNode[PathController.Instance.GetPathNodes().Length]; 
        PathController.Instance.GetPathNodes().CopyTo(pathNodes, 0);
        nodeArraySize = PathController.Instance.GetNodeArraySize();

        lastNodePosition = pathNodes[^1].GetNodePosition();
        entityToMove.position = new Vector3(pathNodes[0].GetNodePosition().x, transform.position.y, pathNodes[0].GetNodePosition().z);

        entityToMove.DOPath(
            GetCubicBezierNodeData(pathNodes[0]), 
            1f, 
            PathType.CubicBezier, 
            PathMode.Full3D, 
            PathController.Resolution, 
            Color.red);  

        // SetNextDestination();
    }
     
    void Update()
    {
        // CheckDistanceFromNextNode(); 
    }

    // infos always grouped by three : destNode, currNodeCtrlPoint, destNodeCtrlPoint; 
    // first node is the function caller's target position
    // called on start and on every SetNextDestination
    private readonly Vector3[] cubicBezierCurveNodeData = new Vector3[3];  
    private Vector3[] GetCubicBezierNodeData(PathNode node1)
    {
        currentAndNextNode = new PathNode[2];

        currentAndNextNode[0] = node1;
        currentAndNextNode[1] = node1.GetNextActiveNode();

        // data structure you use is poorly done for the kind of array iteration you would need here 
        cubicBezierCurveNodeData[0] = currentAndNextNode[1].GetNodePosition();
        cubicBezierCurveNodeData[1] = currentAndNextNode[0].GetControlPointOUTPosition();
        cubicBezierCurveNodeData[2] = currentAndNextNode[1].GetControlPointINPosition();

        return cubicBezierCurveNodeData; 
    }

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
            SetNextDestination(); // this is called too often
        }
    }

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
