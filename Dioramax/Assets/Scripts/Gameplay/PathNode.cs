using UnityEngine;

// holds reference to next possible nodes
public class PathNode : MonoBehaviour
{
    [SerializeField] private Transform previousNode;
    [SerializeField] private Transform[] nextPossibleNodes; // A = 0, B = 1; 
    public bool IsActiveNode { get; set; }

    private Transform nextNode; // updated via switching
    private Vector3 selfPosition; // caching to avoid costly calls to the C++ side of engine
    private Vector3 previousNodePosition; // idem

    public void SetNextNode(int index)
    {
        nextNode = nextPossibleNodes[index]; 
    }

    private void OnValidate()
    {
        Init();
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        IsActiveNode = true;
        selfPosition = transform.position;
        if (previousNode)
        {
            previousNodePosition = previousNode.transform.position;
        }
    }

    public Vector3 GetNodePosition() => selfPosition;
    public Vector3 GetPreviousNodePosition() => previousNodePosition; 
    public Transform[] GetNextPossibleNodesTransform() => nextPossibleNodes;
    public int GetNextPossibleNodesArraySize() => nextPossibleNodes.Length;
}
