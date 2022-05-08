using UnityEngine;

// makes an entity move along a path
public class EntityPathNavigation : MonoBehaviour
{
    public static EntityPathNavigation Instance;

    [SerializeField] private bool loopPath;
    private PathNode[] pathNodes;

    private int destinationNodeIndex;
    private float distanceFromNextNode;
    private int nodeArraySize;

    private Vector3 nextNodePosition, lastNodePosition; 

    private const float SNAP_VALUE = 0.2f; 

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
        transform.position = new Vector3(pathNodes[0].GetNodePosition().x, transform.position.y, pathNodes[0].GetNodePosition().z);

        SetNextDestination();
    }


    void Update()
    {         
        if (Vector3.Distance(transform.position, lastNodePosition) < SNAP_VALUE &&
            destinationNodeIndex == pathNodes.Length-1 && !loopPath) return; // arrived at the end of path

        distanceFromNextNode = Vector3.Distance(transform.position, nextNodePosition);

        // enter here only once
        if (distanceFromNextNode < SNAP_VALUE)
        {
            GameLogger.Log("snapping to current node");
            transform.position = new Vector3(pathNodes[destinationNodeIndex].GetNodePosition().x, transform.position.y, pathNodes[destinationNodeIndex].GetNodePosition().z); 
            SetNextDestination(); // this is called too often
        }
    } 

    // go if more than 90°
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
