using UnityEngine;
using UnityEngine.UI; 

public class LevelInfosUI : MonoBehaviour
{
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
        puzzlesOn[index].SetActive(true);
    }
}
