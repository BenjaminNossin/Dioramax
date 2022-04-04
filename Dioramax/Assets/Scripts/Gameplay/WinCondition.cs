using UnityEngine;

public class WinCondition : MonoBehaviour
{
    [Space, SerializeField] private DioramaPuzzleName entityPuzzleName; // PLACEHOLDER
    [SerializeField] private int entityNumber; // PLACEHOLDER

    private bool winConditionIsMet;
    private bool WinConditionEventIsRegistered;

    void Update()
    {
        if (winConditionIsMet && !WinConditionEventIsRegistered)
        {
            WinConditionEventIsRegistered = true;
            WinConditionController.Instance.ValidateWinCondition((int)entityPuzzleName, entityNumber);
        }
        else if (!winConditionIsMet && WinConditionEventIsRegistered)
        {
            WinConditionEventIsRegistered = false;
            WinConditionController.Instance.InvalidateWinCondition((int)entityPuzzleName, entityNumber);
        }
    } 

    public void UpdateWinCondition(bool conditionStatus)
    {
        winConditionIsMet = conditionStatus;
    } 
}
