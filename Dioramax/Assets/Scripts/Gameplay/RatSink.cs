using UnityEngine;

public class RatSink : MonoBehaviour
{
    [SerializeField] private LayerMask ratMask;
    [SerializeField] private WinCondition winCondition;

    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == ratMask)
        {
            other.gameObject.SetActive(false);
            other.transform.GetComponent<SimulateEntityPhysics>().RemoveRbFromList(); 

            if (!winCondition.OverrideWinConditionCall)
            {
                winCondition.SetWinCondition(true);
            }
            else
            {
                LevelManager.Instance.ValidateWinCondition((int)DioramaPuzzleName.Rats, LevelManager.OverrideWinConditionNumber);
                LevelManager.OverrideWinConditionNumber++; 
            }

            Debug.Log("rat has been sinked"); 
        }
    } 
}
