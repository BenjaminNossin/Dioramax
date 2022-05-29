using UnityEngine;

public class HideObjectOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private ParticleSystem vfxRatTrou; 
    [SerializeField] private LayerMask objectToHideMask;
    [SerializeField] private WinCondition winCondition;
    [SerializeField] private bool isTutorial;
    [SerializeField] private AudioSource audioSource; 

    public static System.Action OnBallTutorialComplete { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == objectToHideMask)
        {
            other.gameObject.SetActive(false);
            other.transform.GetComponent<SimulateEntityPhysics>().RemoveRbFromList();
            audioSource.Play(); 

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
                else // holes for rat puzzle
                {
                    LevelManager.Instance.ValidateWinCondition((int)DioramaPuzzleName.Rats, LevelManager.OverrideWinConditionNumber);
                    LevelManager.OverrideWinConditionNumber++;
                }

                vfxRatTrou.Play();
            }

            GameLogger.Log("object has been hidden and removed from the physics simulation"); 
        }
    } 
}
