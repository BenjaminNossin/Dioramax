using UnityEngine;

public class UI_OptionButton : MonoBehaviour
{
    [SerializeField] private GameObject self;
    [SerializeField] private GameObject optionsHolder;
    [SerializeField] private GameObject objToDeactivateOnPause;
    private bool isPaused; 

    public void SetPauseState()
    {
        isPaused = !isPaused;
        Invoke(nameof(Pause), 0.8f); 
    }

    private void Pause()
    {
        self.SetActive(!isPaused);
        optionsHolder.SetActive(isPaused);
        objToDeactivateOnPause.SetActive(!isPaused);
    }
}
