using UnityEngine;

public class HideObjectOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private LayerMask objectToHideMask;
    [SerializeField] private WinCondition winCondition;
    [SerializeField] private bool isTutorial;

    public static System.Action OnBallTutorialComplete { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == objectToHideMask)
        {
            other.gameObject.SetActive(false);
            other.transform.GetComponent<SimulateEntityPhysics>().RemoveRbFromList(); 

            if (!winCondition.OverrideWinConditionCall)
            {
                winCondition.SetWinCondition(true);
            }
            else 
            {
                if (isTutorial)
                {
                    OnBallTutorialComplete(); 
                }
                else
                {
                    LevelManager.Instance.ValidateWinCondition((int)DioramaPuzzleName.Rats, LevelManager.OverrideWinConditionNumber);
                    LevelManager.OverrideWinConditionNumber++;
                }
            }

            GameLogger.Log("object has been hidden and removed from the physics simulation"); 
        }
    } 
}
