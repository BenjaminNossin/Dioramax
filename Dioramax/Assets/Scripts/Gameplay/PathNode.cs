using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor; 
#endif

// holds reference to next possible nodes
public class PathNode : MonoBehaviour
{
    [SerializeField] private PathNode previousNode;
    [SerializeField] private Transform[] neighboursTransform; // A = 0, B = 1; 
    [SerializeField] private PathNode[] neightboursNodes; // A = 0, B = 1; 
    [SerializeField] private Vector3 controlPointIn;
    [SerializeField] private Vector3 controlPointOut;

    public bool IsActiveNode { get; set; }
    public bool IsLeafNode { get; private set;  }  

    private PathNode nextActiveNode; // updated via switching
    private Vector3 selfPosition; // caching to avoid costly calls to the C++ side of engine
    public int nodeIndex; 

    private void OnValidate()
    {
        InitOrUpdate();
    }

    private void OnDrawGizmos()
    {
        Handles.color = PathController.HandlesColor;
        Handles.DrawLine(transform.position, transform.TransformPoint(controlPointOut), PathController.LineThickness);
        Handles.DrawLine(transform.position, transform.TransformPoint(controlPointIn), PathController.LineThickness); 
    } 

    private void Awake()
    {
        IsLeafNode = neightboursNodes.Length == 0;

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
        IsActiveNode = true; // PROBABLY WRONG
        selfPosition = transform.position;
    }
    public Vector3 GetNodePosition() => selfPosition;
    public Transform[] GetNextPossibleNodesTransform() => neighboursTransform;
    public PathNode[] GetNextPossibleNodes() => neightboursNodes;

    PathNode returnedNode; 
    public PathNode GetNextActiveNode()
    {
        if (IsLeafNode) return null; 

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
    public PathNode GetPreviousNode() => previousNode;
    public int GetNodeIndex() => returnedNode.nodeIndex;
    public int GetPreviousNodeIndex() => previousNode.nodeIndex;
    public int GetNextActiveNodeIndex()
    {
        if (IsLeafNode) return -1; 

        if (neightboursNodes.Length == 1)
        {
            SetNextNode(0);
        }

        for (int i = 0; i < neightboursNodes.Length; i++)
        {
            if (neightboursNodes[i].IsActiveNode)
            {
                returnedNode = neightboursNodes[i];
            }
        }

        return returnedNode.nodeIndex;
    }
    public int GetNextPossibleNodesArraySize() => (int)(neighboursTransform?.Length);
    public Vector3 GetControlPointToWorld(bool getIn = true) => transform.TransformPoint(getIn ? controlPointIn : controlPointOut);
    public Vector3 GetControlPointINPosition() => transform.TransformPoint(controlPointIn); 
    public Vector3 GetControlPointOUTPosition() => transform.TransformPoint(controlPointOut);
}
