using UnityEngine;

public class WinConditionController : MonoBehaviour
{
    public static WinConditionController Instance;

    [SerializeField] private DioramaInfos dioramaInfos;

    public static int[][] entitiesToValidate;
    private byte validatedPuzzleAmount; 

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }

        Instance = this;
    }

    private void Start()
    {
        // WIP : initializing all at 0. This will be changed by loading saved game state
        entitiesToValidate = new int[dioramaInfos.puzzleAmount][];
        for (int i = 0; i < entitiesToValidate.Length; i++)
        {
            entitiesToValidate[i] = new int[dioramaInfos.puzzleInfos[i].entitiesAmount];
            dioramaInfos.puzzleInfos[i].winConditionIsMet = false;

            //Debug.Log($"{dioramaInfos.puzzleInfos[i].puzzleName} is of size {dioramaInfos.puzzleInfos[i].entitiesAmount} and at index {i}");
            for (int j = 0; j < entitiesToValidate[i].Length; j++)
            {
                entitiesToValidate[i][j] = 0;
            }
        }
    }

    public void ValidateWinCondition(int array, int index)
    {
        Debug.Log($"puzzle {(DioramaPuzzleName)array} has validated item n° {index + 1}");

        // a puzzle piece is set to true (==1)
        entitiesToValidate[array][index] = 1;
        dioramaInfos.puzzleInfos[array].winConditionIsMet = true; // by default, we assume the puzzle is finished

        // check if all the pieces of the puzzle are validated (== 1)
        for (int i = 0; i < entitiesToValidate[array].Length; i++)
        {
            // if one or more puzzle pieces are still true, the puzzle is not finished yet
            if (entitiesToValidate[array][i] == 0)
            {
                dioramaInfos.puzzleInfos[array].winConditionIsMet = false;
            }
        }

        // if all the puzzle pieces are valid, the puzzle is completed
        if (dioramaInfos.puzzleInfos[array].winConditionIsMet == true)
        {
            Debug.Log($"puzzle {(DioramaPuzzleName)array} is finished");
            validatedPuzzleAmount++; 
        }
        else
        {
            Debug.Log($"puzzle {(DioramaPuzzleName)array} is NOT finished"); // debug
        }

        if (validatedPuzzleAmount == dioramaInfos.puzzleAmount)
        {
            Debug.Log("Level is FINISHED"); // debug
        }
    }

    public void InvalidateWinCondition(int array, int index)
    {
        Debug.Log($"puzzle {(DioramaPuzzleName)array} has invalidated item n° {index + 1}");

        entitiesToValidate[array][index] = 0;
    }
}


