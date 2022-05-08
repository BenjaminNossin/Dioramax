using UnityEngine;

// stores all the node data and update path according to switches
public class PathController : MonoBehaviour
{
    public static PathController Instance;
    [SerializeField] private PathNode[] Nodes; 

    private Vector3 currentIndexNodePosition;

    [Header("DEBUG")]
    public bool doDebugDrawGizmos;

    private void OnDrawGizmos()
    {
        if (Nodes.Length != 0 && doDebugDrawGizmos)
        {
            for (var i = 0; i < Nodes.Length; i++)
            {
                currentIndexNodePosition = Nodes[i].GetNodePosition();

                Gizmos.color = i == Nodes.Length - 1 ? Color.red : (i == 0 ? Color.white : Color.yellow);
                Gizmos.DrawWireSphere(currentIndexNodePosition, 0.25f);

                for (int j = 0; j < Nodes[i].GetNextPossibleNodesArraySize(); j++)
                {
                    Gizmos.DrawLine(currentIndexNodePosition, Nodes[i].GetNextPossibleNodesTransform()[j].position);
                }
            }
        } 
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this; 
    }

    public Vector3 GetNodePosition(int index) => Nodes[index].GetNodePosition();
    public int GetNodeArraySize() => Nodes.Length;

    public void AddNode()
    {

    }

    public void RemoveNode()
    {

    }
}
