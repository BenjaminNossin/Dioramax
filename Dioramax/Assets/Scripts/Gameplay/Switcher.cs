using UnityEngine;

// alternates between two positions based on player touch
// NEED REFACTORING : derive all buttons from a common abstract class
public class Switcher : MonoBehaviour
{
    [SerializeField] private Vector3 orientationB;
    [SerializeField] private bool AIsInitialPosition;
    [SerializeField, Range(0.25f, 2f)] private float turnDuration = 0.5f; 

    private bool goToPositionA; // always Quaternion.Identity
    private bool lerpDone; 
    private float lerper; 

    private void Start()
    {
        goToPositionA = AIsInitialPosition;
        transform.localRotation = Quaternion.Euler(goToPositionA ? Vector3.zero : orientationB); // almost same as Switch() except from bool inversion. Try to refactor
    }

    public void InvertBoolAndDoSwitch()
    {
        InvertBool();
        DoSwitch();
    }

    private void InvertBool()
    {
        goToPositionA = !goToPositionA;
    }

    private void DoSwitch()
    {
        transform.localRotation = Quaternion.Euler(goToPositionA ? Vector3.zero : orientationB); 
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
