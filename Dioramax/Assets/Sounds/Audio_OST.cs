using UnityEngine;
using UnityEngine.SceneManagement; 

public class Audio_OST : MonoBehaviour
{
    private int audioclipIndex;

    void Start()
    {
        audioclipIndex = SceneManager.GetActiveScene().buildIndex; 
        if (audioclipIndex >= 4)
        {
            audioclipIndex = 0; 
        }

        if (audioclipIndex == 0 && AudioManager.Instance.PlayingSharedOST) return;
        AudioManager.Instance.PlayOST(audioclipIndex); 
    }
}
