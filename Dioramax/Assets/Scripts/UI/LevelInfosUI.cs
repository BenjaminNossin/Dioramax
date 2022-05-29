using UnityEngine;

public class LevelInfosUI : MonoBehaviour
{
    [SerializeField] private AudioSource puzzleWinAudiosource; 

    public static LevelInfosUI Instance;

    [SerializeField] private GameObject[] puzzlesOn = new GameObject[3];

    [Space, SerializeField] private DioramaInfos previousDioramaInfos; 

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
        puzzleWinAudiosource.Play();
        puzzlesOn[index].SetActive(true);
    }
}
