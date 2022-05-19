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


    [Space, SerializeField, Range(0.25f, 2f)] private float lineThickness = 1f; // A = 0, B = 1; 

    private static float LineThickness; 

    public bool IsActiveNode { get; set; }

    private Transform nextNode; // updated via switching
    private Vector3 selfPosition; // caching to avoid costly calls to the C++ side of engine
    private Vector3 previousNodePosition; // idem

    private void OnValidate()
    {
        InitorUpdate();
    }

    private void Awake()
    {
        InitorUpdate();
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.TransformPoint(controlPointIn));
        Handles.DrawLine(transform.position, transform.TransformPoint(controlPointIn), LineThickness); 
    }

    private void Start()
    {
        if (neightboursNodes.Length == 1)
        {
            SetNextNode(0); 
        }
    }

    public void SetNextNode(int index)
    {
        nextNode = neighboursTransform[index];
    }

    private void InitorUpdate()
    {
        LineThickness = lineThickness; 

        IsActiveNode = true;
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
    public int GetNextPossibleNodesArraySize() => (int)(neighboursTransform?.Length);
    public Vector3 GetControlPointToWorld(bool getIn= true) => transform.TransformPoint(getIn ? controlPointIn : controlPointOut);
    public Vector3 GetControlPointIN()
    {
        return transform.TransformPoint(controlPointIn); 
    }
    public Vector3 GetControlPointOUT()
    {
        return transform.TransformPoint(controlPointOut);
    }
}
