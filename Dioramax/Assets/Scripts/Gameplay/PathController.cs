using UnityEngine;
using DG.Tweening; 

// stores all the node data and update path according to switches
// this script is NOT in charge of the entity navigation along the path
[ExecuteAlways]
public class PathController : MonoBehaviour
{
    public static PathController Instance;

    [Space, SerializeField, Range(1f, 5f)] private float lineThickness = 1f;
    [Space, SerializeField, Range(30, 100)] private int segmentResolution = 50;
    [SerializeField] private Color handlesColor = Color.white;

    public static float LineThickness { get; private set; }
    public static Color HandlesColor { get; private set; }

    private PathNode[] Nodes; // even though it changes in editor, the value is reset to O when  hitting Play

    private Vector3 currentIndexNodePosition;
    public static int Resolution; 

    [Header("DEBUG")]
    public bool refreshNodesArrayOnReferenceLoss;

    private void OnValidate()
    {
        LineThickness = lineThickness;
        HandlesColor = handlesColor;
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
        Resolution = segmentResolution;
    }

    private void Start()
    {
        PopulateArray();
    }

    // call this from EntityPathNavigation, on Start and every time you reach target node
    private float f;
    private PathNode _node1, _node2;
    public Vector3[] GetPointsAlongPathBetweenNodes(PathNode node1, PathNode node2, ref Vector3[] pointsAlongPath, bool getRevertPath = false)
    {
        f = 0f;

        _node1 = node1;
        _node2 = node2;
        if (getRevertPath)
        {
            _node1 = _node2;
            _node2 = node1;
        }

        for (int i = 0; i < Resolution; i++)
        {
            pointsAlongPath[i] = DOCurve.CubicBezier.GetPointOnSegment(_node1.GetNodePosition(), _node1.GetControlPointOUTPosition(), _node2.GetNodePosition(),
            _node2.GetControlPointINPosition(), f);
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
                Nodes[i].nodeIndex = i; 
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
