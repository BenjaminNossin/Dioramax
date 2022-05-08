using UnityEngine;

// makes an entity move along a path
public class EntityPathNavigation : MonoBehaviour
{
    public static EntityPathNavigation Instance;

    [SerializeField, Range(0.25f, 5f)] private float moveSpeed = 1f; // on the object, not the path
    [SerializeField] private bool loopPath; 

    public float MoveSpeed { get; set; }
    public int DestPoint { get; private set; }
    private float remainingDistance;
    private Vector3 destinationDirection;

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
    }


    void Update()
    {
        /* 
        if (Vector3.Distance(transform.position, entityPathController.Nodes[^1].position) < 0.05f &&
            DestPoint == entityPathController.Points.Length-1 && !loopPath) return; // arrived at the end of path

        MoveSpeed = moveSpeed; 
        remainingDistance = Vector3.Distance(transform.position, entityPathController.Nodes[DestPoint].position);
        if (remainingDistance < 0.05)
        {
            GotoNextPoint();
        }
        else
        {
            transform.position += destinationDirection * Time.deltaTime * moveSpeed;
        } */
    } 

    // go if more than 90°
    private int nodeArraySize; 
    /* private void GotoNextPoint()
    {
        nodeArraySize = PathController.Instance.GetNodeArraySize();
        if (nodeArraySize == 0)
            return;

        DestPoint = (DestPoint + 1) % nodeArraySize;
        destinationDirection = (entityPathController.Nodes[DestPoint].position - transform.position).normalized;
        transform.LookAt(entityPathController.Nodes[DestPoint].position);
    } */

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
