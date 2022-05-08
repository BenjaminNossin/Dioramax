using UnityEngine;

// stores all the node data and update path according to switches
// this script is NOT in charge of the entity navigation along the path
[ExecuteAlways]
public class PathController : MonoBehaviour
{
    public static PathController Instance;
    private PathNode[] Nodes; // even though it changes in editor, the value is reset to O when  hitting Play

    private Vector3 currentIndexNodePosition;

    [Header("DEBUG")]
    public bool refreshNodesArrayOnReferenceLoss;

    private void OnValidate()
    {
        if (refreshNodesArrayOnReferenceLoss)
        {
            PopulateArray();
        }
    }

    private void OnTransformChildrenChanged()
    {
        GameLogger.Log("Child count changed. Updating array");
        PopulateArray();
    }

    private void OnDrawGizmos()
    {
        if (Nodes.Length != 0 && refreshNodesArrayOnReferenceLoss)
        {
            for (var i = 0; i < Nodes.Length; i++)
            {
                currentIndexNodePosition = Nodes[i].GetNodePosition();

                Gizmos.color = i == Nodes.Length - 1 ? Color.red : (i == 0 ? Color.white : Color.yellow);

                if (Nodes[i].IsActiveNode)
                {
                    Gizmos.DrawWireSphere(currentIndexNodePosition, 0.25f);
                }

                for (int j = 0; j < Nodes[i].GetNextPossibleNodesArraySize(); j++)
                {
                    try
                    {
                        Gizmos.DrawLine(currentIndexNodePosition, Nodes[i].GetNextPossibleNodesTransform()[j].position);
                    }
                    catch (MissingReferenceException)
                    {
                        GameLogger.Log($"{Nodes[i].gameObject} is missing one or more references in its nextPossibleNodes array. Fill or remove them."); 
                    }

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

    private void Start()
    {
        PopulateArray();
    }

    private void PopulateArray()
    {
        Nodes = new PathNode[transform.childCount];

        if (Nodes.Length != 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                try
                {
                    Nodes[i] = transform.GetChild(i).GetComponent<PathNode>();
                }
                catch (System.Exception)
                {
                    GameLogger.Log("A PathNode component has been added to the new object. \n If that object is not supposed to have that component, " +
                        "please remove it from the PathController child hierarchy.");
                }
            }
        }
    }

    public Vector3 GetNodePosition(int index) => Nodes[index].GetNodePosition();

    public int GetNodeArraySize() => Nodes.Length;

    public void AddNode()
    {

    }

    public void RemoveNode()
    {

    }

    public PathNode[] GetPathNodes() => Nodes; 
}
