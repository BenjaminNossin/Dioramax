using UnityEngine;

// stores all the node data and update path according to switches
[ExecuteAlways]
public class PathController : MonoBehaviour
{
    public static PathController Instance;
    private PathNode[] Nodes; 

    private Vector3 currentIndexNodePosition;

    [Header("DEBUG")]
    public bool refreshDrawGizmosOnNodePathChange;

    private void OnValidate()
    {
    }

    private void OnTransformChildrenChanged()
    {
        GameLogger.Log("Child count changed. Updating array"); 
        Nodes = new PathNode[transform.childCount];

        if (Nodes.Length != 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                try
                {
                    Nodes[i] = transform.GetChild(i).GetComponent<PathNode>();
                }
                catch (System.Exception e)
                {
                    GameLogger.Log($"{e.Message}. \n"); 
                    GameLogger.Log("A PathNode component has been added to the new object. \n If that object is not supposed to have that component, " +
                        "please remove it from the PathController child hierarchy.");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Nodes.Length != 0 && refreshDrawGizmosOnNodePathChange)
        {
            for (var i = 0; i < Nodes.Length; i++)
            {
                currentIndexNodePosition = Nodes[i].GetNodePosition();

                Gizmos.color = i == Nodes.Length - 1 ? Color.red : (i == 0 ? Color.white : Color.yellow);
                Gizmos.DrawWireSphere(currentIndexNodePosition, 0.25f);

                for (int j = 0; j < Nodes[i].GetNextPossibleNodesArraySize(); j++)
                {
                    try
                    {
                        Gizmos.DrawLine(currentIndexNodePosition, Nodes[i].GetNextPossibleNodesTransform()[j].position);
                    }
                    catch (MissingReferenceException)
                    {
                        GameLogger.Log($"{Nodes[i].gameObject} is missing one or more references in its nextPossibleNodes array."); 
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

    public Vector3 GetNodePosition(int index) => Nodes[index].GetNodePosition();
    public int GetNodeArraySize() => Nodes.Length;

    public void AddNode()
    {

    }

    public void RemoveNode()
    {

    }
}
