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
    [SerializeField, Range(0.25f, 2f)] private float turnDuration = 0.5f;
    private Collider[] blockers = new Collider[2]; // A = 0; B = 1; 

    private PathNode pathNode; // own pathNode
    private PathNode[] nextPossibleNodes; 

    private bool goToPositionA; 
    private bool lerpDone; 
    private float lerper; 

    private void Start()
    {
        for (int i = 0; i < tranfToRotate.childCount; i++)
        {
            blockers[i] = tranfToRotate.GetChild(i).GetComponent<Collider>();
        }

        goToPositionA = AIsInitialPosition;
        tranfToRotate.localRotation = Quaternion.Euler(goToPositionA ? orientationA : orientationB); // almost same as Switch() except from bool inversion. Try to refactor
        pathNode = GetComponent<PathNode>();

        // bad to call GetNextPossibleNodesTransform() twice
        nextPossibleNodes = new PathNode[pathNode.GetNextPossibleNodes().Length];
        pathNode.GetNextPossibleNodes().CopyTo(nextPossibleNodes, 0);

        SetActiveNode();
        SetActiveBlocker();
    }

    public void InvertBoolAndDoSwitch()
    {
        InvertBool();
        DoSwitch();
        SetActiveNode();
        SetActiveBlocker();
    }

    private void InvertBool()
    {
        goToPositionA = !goToPositionA;
    }

    private void DoSwitch()
    {
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

    private void SetActiveBlocker()
    {
        for (int i = 0; i < blockers.Length; i++)
        {
            blockers[i].enabled = true; 
            if (goToPositionA)
            {
                blockers[0].enabled = false; 
            }
            else
            {
                blockers[1].enabled = false;
            }
        }
    }


    /* private void DoSwitch()
    {
        GameLogger.Log("lerping");        
        transformToRotate.localRotation = Quaternion.Slerp(Quaternion.Euler(goToPositionA ? orientationB : Vector3.zero),
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
