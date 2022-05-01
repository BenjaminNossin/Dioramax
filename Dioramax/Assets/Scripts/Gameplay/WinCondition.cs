using UnityEngine;

public class WinCondition : MonoBehaviour
{
    [Space, SerializeField] private DioramaPuzzleName entityPuzzleName; // PLACEHOLDER
    [SerializeField] private int entityNumber; // PLACEHOLDER

    [Header("DEBUG")]
    public bool simulateWinConditionIsMet; 

    private bool winConditionIsMet;
    private bool WinConditionEventIsRegistered;

    void Update()
    {
        if (!simulateWinConditionIsMet)
        {
            if (winConditionIsMet && !WinConditionEventIsRegistered)
            {
                SetWinCondition(true);

                // if puzzle name == tuyaux, cf "Trucs à intégrer GA"
            }
            else if (!winConditionIsMet && WinConditionEventIsRegistered)
            {
                WinConditionEventIsRegistered = false;
                SetWinCondition(false);
            }
        }
        else if (!WinConditionEventIsRegistered)
        {
            SetWinCondition(true);
        }
    }

    private void SetWinCondition(bool winConditionState)
    {
        WinConditionEventIsRegistered = winConditionState;
        WinConditionController.Instance.ValidateWinCondition((int)entityPuzzleName, entityNumber);
    }

    public void UpdateWinCondition(bool conditionStatus)
    {
        winConditionIsMet = conditionStatus;
    } 
}
