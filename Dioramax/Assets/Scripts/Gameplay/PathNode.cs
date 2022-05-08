using UnityEngine;

// holds reference to next possible nodes
public class PathNode : MonoBehaviour
{
    [SerializeField] private Transform[] nextPossibleNodes;
    // A = 0, B = 1; 
    private Transform nextNode; // via switching

    public void SetNextNode(int index)
    {
        nextNode = nextPossibleNodes[index]; 
    }

    public Vector3 GetNodePosition() => transform.position; 
    public Transform[] GetNextPossibleNodesTransform() => nextPossibleNodes;
    public int GetNextPossibleNodesArraySize() => nextPossibleNodes.Length;
}
