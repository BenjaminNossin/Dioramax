using UnityEngine;

// alternates between two positions based on player touch
// NEED REFACTORING : derive all buttons from a common abstract class
[RequireComponent(typeof(PathNode))]
public class Switcher : MonoBehaviour
{
    [SerializeField] private Transform tranfToRotate; 
    [SerializeField] private Vector3 orientationA;
    [SerializeField] private Vector3 orientationB;
    [SerializeField] private bool AIsInitialPosition;

    private PathNode selfPathNode; 
    private PathNode[] nextPossibleNodes; 

    private bool goToPositionA; 

    private void Start()
    {
        goToPositionA = AIsInitialPosition;
        tranfToRotate.localRotation = Quaternion.Euler(goToPositionA ? orientationA : orientationB); // almost same as Switch() except from bool inversion. Try to refactor
        selfPathNode = GetComponent<PathNode>();

        // bad to call GetNextPossibleNodesTransform() twice
        nextPossibleNodes = new PathNode[selfPathNode.GetNextPossibleNodes().Length];
        selfPathNode.GetNextPossibleNodes().CopyTo(nextPossibleNodes, 0);

        SetActiveNode();
    }

    public void DoSwitchAndSetActiveNode()
    {
        DoSwitch();
        SetActiveNode();
    }

    private void DoSwitch()
    {
        goToPositionA = !goToPositionA;
        tranfToRotate.localRotation = Quaternion.Euler(goToPositionA ? orientationA : orientationB); 
    }

    private void SetActiveNode()
    {
        if (nextPossibleNodes.Length == 0) return; 

        for (int i = 0; i < nextPossibleNodes.Length; i++)
        {
            nextPossibleNodes[i].IsActiveNode = false; 

            if (goToPositionA)
            {
                nextPossibleNodes[0].IsActiveNode = true;
            }
            else
            {
                nextPossibleNodes[1].IsActiveNode = true;
            }
        }
    }
}
