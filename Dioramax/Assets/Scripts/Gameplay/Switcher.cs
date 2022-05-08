using UnityEngine;

// alternates between two positions based on player touch
// NEED REFACTORING : derive all buttons from a common abstract class
[RequireComponent(typeof(PathNode))]
public class Switcher : MonoBehaviour
{
    [SerializeField] private Vector3 orientationA;
    [SerializeField] private Vector3 orientationB;
    [SerializeField] private bool AIsInitialPosition;
    [SerializeField, Range(0.25f, 2f)] private float turnDuration = 0.5f;

    private PathNode pathNode; // own pathNode
    private Transform[] nextPossibleNodes; 

    private bool goToPositionA; 
    private bool lerpDone; 
    private float lerper; 

    private void Start()
    {
        goToPositionA = AIsInitialPosition;
        transform.localRotation = Quaternion.Euler(goToPositionA ? orientationA : orientationB); // almost same as Switch() except from bool inversion. Try to refactor
        pathNode = GetComponent<PathNode>();

        // bad to call GetNextPossibleNodesTransform() twice
        nextPossibleNodes = new Transform[pathNode.GetNextPossibleNodesTransform().Length];
        pathNode.GetNextPossibleNodesTransform().CopyTo(nextPossibleNodes, 0);

        SetActiveNode();
    }

    public void InvertBoolAndDoSwitch()
    {
        InvertBool();
        DoSwitch();
        SetActiveNode();
    }

    private void InvertBool()
    {
        goToPositionA = !goToPositionA;
    }

    private void DoSwitch()
    {
        transform.localRotation = Quaternion.Euler(goToPositionA ? orientationA : orientationB); 
    }

    // DEBUG
    private void SetActiveNode()
    {
        if (nextPossibleNodes.Length == 0) return; 

        for (int i = 0; i < nextPossibleNodes.Length; i++)
        {
            nextPossibleNodes[i].GetComponent<PathNode>().IsActiveNode = false; // default all to false

            if (goToPositionA)
            {
                nextPossibleNodes[0].GetComponent<PathNode>().IsActiveNode = true;
            }
            else
            {
                nextPossibleNodes[1].GetComponent<PathNode>().IsActiveNode = true;
            }
        }
    }

    /* private void DoSwitch()
    {
        GameLogger.Log("lerping");        
        transform.localRotation = Quaternion.Slerp(Quaternion.Euler(goToPositionA ? orientationB : Vector3.zero),
                                                   Quaternion.Euler(goToPositionA ? Vector3.zero : orientationB),
                                                   lerper);

        lerper += Time.deltaTime / turnDuration;
        if (lerper < 1)
        {
            DoSwitch();
        }
        else
        {
            GameLogger.Log("finished lerping");
            lerper = 0f;
        }
    } */
}
