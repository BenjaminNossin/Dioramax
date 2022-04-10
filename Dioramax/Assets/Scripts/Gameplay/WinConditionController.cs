using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinConditionController : MonoBehaviour
{
    public static WinConditionController Instance;

    [SerializeField] private DioramaInfos dioramaInfos;

    public static int[][] entitiesToValidate;

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

            Debug.Log($"{dioramaInfos.puzzleInfos[i].puzzleName} is of size {dioramaInfos.puzzleInfos[i].entitiesAmount} and at index {i}");
            for (int j = 0; j < entitiesToValidate[i].Length; j++)
            {
                entitiesToValidate[i][j] = 0;
            }
        }
    }

    public void ValidateWinCondition(int array, int index)
    {
        Debug.Log($"puzzle {(DioramaPuzzleName)array} has validated item n° {index + 1}");

        entitiesToValidate[array][index] = 1;
        dioramaInfos.puzzleInfos[array].winConditionIsMet = true;

        for (int i = 0; i < entitiesToValidate[array].Length; i++)
        {
            if (entitiesToValidate[array][i] == 0)
            {
                dioramaInfos.puzzleInfos[array].winConditionIsMet = false;
            }
        }

        if (dioramaInfos.puzzleInfos[array].winConditionIsMet == true)
        {
            Debug.Log($"puzzle {(DioramaPuzzleName)array} is finished");
        }
    }

    public void InvalidateWinCondition(int array, int index)
    {
        Debug.Log($"puzzle {(DioramaPuzzleName)array} has invalidated item n° {index + 1}");

        entitiesToValidate[array][index] = 0;
    }
}

