using UnityEngine;

public class WinCondition : MonoBehaviour
{
    [Space, SerializeField] private DioramaPuzzleName entityPuzzleName; // PLACEHOLDER
    public int entityNumber;
    [SerializeField] private bool overrideWinConditionCall; // for rats in case more than one fall in the same hole; 

    [Header("DEBUG")]
    public bool simulateWinConditionIsMet; 

    private bool winConditionIsMet;
    private bool WinConditionEventIsRegistered;

    void Update()
    {
        if (overrideWinConditionCall) return; 

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

    public void SetWinCondition(bool eventIsRegistered)
    {
        WinConditionEventIsRegistered = eventIsRegistered;
        LevelManager.Instance.ValidateWinCondition((int)entityPuzzleName, entityNumber);
    }

    public void UpdateWinCondition(bool conditionStatus)
    {
        winConditionIsMet = conditionStatus;
    } 
}
