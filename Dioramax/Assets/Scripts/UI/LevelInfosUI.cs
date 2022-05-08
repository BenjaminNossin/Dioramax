using UnityEngine;
using UnityEngine.UI; 

public class LevelInfosUI : MonoBehaviour
{
    public static LevelInfosUI Instance;

    [SerializeField] private Image[] puzzleImages = new Image[3];
    [SerializeField] private Sprite[] winPuzzleSprite = new Sprite[3];

    [Space, SerializeField] private DioramaInfos previousDioramaInfos; 
    private int offset;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    private void Start()
    {
        offset =  previousDioramaInfos != null ? previousDioramaInfos.puzzleAmount : 0;
    }

    public void ActivatePuzzleUIOnWin(int index)
    {
        puzzleImages[index - offset].sprite = winPuzzleSprite[index - offset]; 
    }
}
