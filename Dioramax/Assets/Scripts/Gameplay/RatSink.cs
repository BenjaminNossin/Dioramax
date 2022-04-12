using UnityEngine;

public class RatSink : MonoBehaviour
{
    [SerializeField] private LayerMask ratMask;

    private void OnTriggerEnter(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == ratMask)
        {
            Destroy(other.gameObject); // DEBUG. I will not destroy object in the final version (too much gc)
            Debug.Log("rat has been detected"); 
        }
    }
}
