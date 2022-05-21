using UnityEngine;
using DG.Tweening; 

// stores all the node data and update path according to switches
// this script is NOT in charge of the entity navigation along the path
[ExecuteAlways]
public class PathController : MonoBehaviour
{
    [SerializeField, Range(4, 50)] private int pathResolution = 20;  
    public static PathController Instance;
    private PathNode[] Nodes; // even though it changes in editor, the value is reset to O when  hitting Play

    private Vector3 currentIndexNodePosition;
    public static int Resolution = 10; 

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
        if (Nodes.Length != 0)
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
                        // Gizmos.DrawLine(currentIndexNodePosition, Nodes[i].GetNextPossibleNodesTransform()[j].position);
                        if (i+1 < Nodes.Length)
                        {
                            DrawPath(Nodes[i], Nodes[i].GetNextPossibleNodes()[j]); 
                        }
                    }
                    catch (MissingReferenceException)
                    {
                        GameLogger.Log("The gameobject you added as child is missing a NodePath component. \n " +
                            "Please add it, or take the gameobject out of the child list. "); 
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
        Resolution = pathResolution; 
    }

    private void Start()
    {
        PopulateArray();
    }

    // call this from EntityPathNavigation, on Start and every time you reach target node
    private float f;
    public Vector3[] GetPointsAlongPathBetweenNodes(PathNode node1, PathNode node2, ref Vector3[] pointsAlongPath)
    {
        f = 0f; 
        for (int i = 0; i < Resolution; i++)
        {
            pointsAlongPath[i] = DOCurve.CubicBezier.GetPointOnSegment(node1.GetNodePosition(), node1.GetControlPointOUTPosition(), node2.GetNodePosition(),
            node2.GetControlPointINPosition(), f);
            f += (1f / Resolution); 
        }

        return pointsAlongPath; 
    }

    Vector3 v1, v2; 
    private void DrawPath(PathNode node1, PathNode node2)
    {
        // i IN
        // i+1 OUT 
        for (float f = 0; f < 1.0f; f += (1f/Resolution))
        {
            v1 = DOCurve.CubicBezier.GetPointOnSegment(node1.GetNodePosition(), node1.GetControlPointOUTPosition(), node2.GetNodePosition(),
                node2.GetControlPointINPosition(), f);
            v2 = DOCurve.CubicBezier.GetPointOnSegment(node1.GetNodePosition(), node1.GetControlPointOUTPosition(), node2.GetNodePosition(),
                 node2.GetControlPointINPosition(), f + (1f/Resolution));

            Gizmos.DrawLine(v1, v2);
        }
    }

    private void PopulateArray()
    {
        Nodes = new PathNode[transform.childCount];

        if (Nodes.Length != 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Nodes[i] = transform.GetChild(i).GetComponent<PathNode>();
                /* try
                {
                    Nodes[i] = transform.GetChild(i).GetComponent<PathNode>();
                }
                catch { } */
            }
        }
    }

    public int GetNodeArraySize() => Nodes.Length;

    public PathNode[] GetPathNodes() => Nodes; 
}
