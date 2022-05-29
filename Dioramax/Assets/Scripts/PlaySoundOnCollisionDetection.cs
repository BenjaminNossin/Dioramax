using UnityEngine;

public class PlaySoundOnCollisionDetection : MonoBehaviour
{
    [SerializeField] private LayerMask objectMask;
    [SerializeField] private AudioSource audioSource; 

    private void OnCollisionEnter(Collision collision)
    {
        if (Mathf.Pow(2, collision.gameObject.layer) == objectMask)
        {
            audioSource.Play();
        }
    }
}
