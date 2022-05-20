using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor; 
#endif

// holds reference to next possible nodes
public class PathNode : MonoBehaviour
{
    [SerializeField] private Transform previousNode;
    [SerializeField] private Transform[] neighboursTransform; // A = 0, B = 1; 
    [SerializeField] private PathNode[] neightboursNodes; // A = 0, B = 1; 
    [SerializeField] private Vector3 controlPointIn;
    [SerializeField] private Vector3 controlPointOut;


    [Space, SerializeField, Range(1f, 5f)] private float lineThickness = 1f; // A = 0, B = 1; 

    private static float LineThickness; 

    public bool IsActiveNode { get; set; }

    private PathNode nextActiveNode; // updated via switching
    private Vector3 selfPosition; // caching to avoid costly calls to the C++ side of engine
    private Vector3 previousNodePosition; // idem
    public int nodeIndex; 

    private void OnValidate()
    {
        InitOrUpdate();
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.white;
        Handles.DrawLine(transform.position, transform.TransformPoint(controlPointOut), LineThickness);
        Handles.DrawLine(transform.position, transform.TransformPoint(controlPointIn), LineThickness); 
    } 

    private void Awake()
    {
        if (neightboursNodes.Length == 1)
        {
            SetNextNode(0);
        }

        InitOrUpdate();
    }

    public void SetNextNode(int index)
    {
        nextActiveNode = neightboursNodes[index];
    }

    private void InitOrUpdate()
    {
        LineThickness = lineThickness; 

        IsActiveNode = true; // PROBABLY WRONG
        selfPosition = transform.position;
        if (previousNode)
        {
            previousNodePosition = previousNode.transform.position;
        }
    }
    public Vector3 GetNodePosition() => selfPosition;
    public Vector3 GetPreviousNodePosition() => previousNodePosition; 
    public Transform[] GetNextPossibleNodesTransform() => neighboursTransform;
    public PathNode[] GetNextPossibleNodes() => neightboursNodes;

    PathNode returnedNode; 
    public PathNode GetNextActiveNode()
    {
        if (neightboursNodes.Length == 1)
        {
            SetNextNode(0);
            return nextActiveNode; 
        }

        for (int i = 0; i < neightboursNodes.Length; i++)
        {
            if (neightboursNodes[i].IsActiveNode)
            {
                returnedNode = neightboursNodes[i]; 
            }
        }

        return returnedNode; 
    }
    public int GetNextPossibleNodesArraySize() => (int)(neighboursTransform?.Length);
    public Vector3 GetControlPointToWorld(bool getIn= true) => transform.TransformPoint(getIn ? controlPointIn : controlPointOut);
    public Vector3 GetControlPointINPosition()
    {
        return transform.TransformPoint(controlPointIn); 
    }
    public Vector3 GetControlPointOUTPosition()
    {
        return transform.TransformPoint(controlPointOut);
    }
}
