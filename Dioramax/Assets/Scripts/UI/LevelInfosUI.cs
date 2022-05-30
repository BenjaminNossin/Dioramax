using UnityEngine;

public class LevelInfosUI : MonoBehaviour
{
    [SerializeField] private AudioSource puzzleWinAudiosource; 

    public static LevelInfosUI Instance;

    [SerializeField] private GameObject[] puzzlesOn = new GameObject[3];
    [SerializeField] private GameObject[] puzzlesOff = new GameObject[3];

    [Space, SerializeField] private DioramaInfos previousDioramaInfos;

    private Tween_PuzzleComplete[] tween_PuzzleComplete;

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
        tween_PuzzleComplete = new Tween_PuzzleComplete[puzzlesOn.Length];
        for (int i = 0; i < tween_PuzzleComplete.Length; i++)
        {
            tween_PuzzleComplete[i] = puzzlesOn[i].GetComponent<Tween_PuzzleComplete>();
        }
    }

    public void ActivatePuzzleUIOnWin(int index)
    {
        puzzleWinAudiosource.Play();
        tween_PuzzleComplete[index].PuzzleCompleteTween();
        puzzlesOn[index].SetActive(true);
        puzzlesOff[index].SetActive(false);
    }
}
