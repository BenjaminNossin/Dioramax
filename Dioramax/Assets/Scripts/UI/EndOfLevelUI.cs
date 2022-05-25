using UnityEngine;

public class EndOfLevelUI : MonoBehaviour
{
    public static EndOfLevelUI Instance;

    [SerializeField] private GameObject endOfLevelPanel;
    [SerializeField] private Tween_EndOfLevel popupTween;


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
        popupTween.WinTween();
    }
}
