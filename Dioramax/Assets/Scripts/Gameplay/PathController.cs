using UnityEngine;
using DG.Tweening; 

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
    }

    private void Start()
    {
        PopulateArray();
    }

   
    // call this from EntityPathNavigation.
    // f is data on train
    // reinit f from train when nextNode is reached
    public Vector3 GetPointAlongPathBetweenNodes(PathNode node1, PathNode node2, float f) // entre 0 et 1
    {
        return DOCurve.CubicBezier.GetPointOnSegment(node1.GetNodePosition(), node1.GetControlPointOUT(), node2.GetNodePosition(),
            node2.GetControlPointIN(), f); 
    }

    private void DrawPath(PathNode node1, PathNode node2)
    {
        Vector3 v1;
        Vector3 v2;
        // i IN
        // i+1 OUT
        for (float f = 0; f < 1.0f; f += 0.1f)
        {
            v1 = DOCurve.CubicBezier.GetPointOnSegment(node1.GetNodePosition(), node1.GetControlPointOUT(), node2.GetNodePosition(),
                node2.GetControlPointIN(), f);
            v2 = DOCurve.CubicBezier.GetPointOnSegment(node1.GetNodePosition(), node1.GetControlPointOUT(), node2.GetNodePosition(),
                 node2.GetControlPointIN(), f + 0.1f);

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
