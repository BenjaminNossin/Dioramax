using UnityEngine;

public class RatSink : MonoBehaviour
{
    [SerializeField] private LayerMask ratMask;
    [SerializeField] private WinCondition winCondition;

    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == ratMask)
        {
            Destroy(other.gameObject); // DEBUG. I will not destroy object in the final version (too much gc)
            winCondition.UpdateWinCondition(true); 

            Debug.Log("rat has been destroyed"); 
        }
    }
}
