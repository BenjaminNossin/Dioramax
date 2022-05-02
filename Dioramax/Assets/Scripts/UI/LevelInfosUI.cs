using UnityEngine;
using UnityEngine.UI; 

public class LevelInfosUI : MonoBehaviour
{
    public static LevelInfosUI Instance;

    [SerializeField] private Image[] puzzleImages = new Image[3];
    [SerializeField] private Sprite[] winPuzzleSprite = new Sprite[3]; 

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    public void ActivatePuzzleUIOnWin(int index)
    {
        puzzleImages[index].sprite = winPuzzleSprite[index]; 
    }
}
