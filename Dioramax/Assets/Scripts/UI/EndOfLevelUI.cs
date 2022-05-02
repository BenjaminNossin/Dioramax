using UnityEngine;

public class EndOfLevelUI : MonoBehaviour
{
    public static EndOfLevelUI Instance;

    [SerializeField] private GameObject endOfLevelPanel; 

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance); 
        }
        Instance = this;
    }

    public void ShowEndOfLevelPanel()
    {
        endOfLevelPanel.SetActive(true);
        LevelManager.Instance.SetGameState(GameState.Paused); 
    }
}
