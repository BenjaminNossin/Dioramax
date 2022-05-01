using UnityEngine;
using System.Collections.Generic; 

// abstract this out to have one for each level
public class WinConditionController : MonoBehaviour
{
    public static WinConditionController Instance;

    [SerializeField] private DioramaInfos dioramaInfos;
    [SerializeField] private PhaseHolder[] phaseHolders = new PhaseHolder[5];
    [SerializeField] private GameObject[] puzzleCompleteVFXS = new GameObject[3]; 

    public static int[][] EntitiesToValidate { get; set; }
    private byte validatedPuzzleAmount; 
    public static bool LevelIsFinished { get; private set; }

    void Awake()
    {
        LevelIsFinished = false;

        if (Instance != null)
        {
            Destroy(this);
        }

        Instance = this;
    }

    private void Start()
    {
        // WIP : initializing all at 0. This will be changed by loading saved game state
        EntitiesToValidate = new int[dioramaInfos.puzzleAmount][];
        for (int i = 0; i < EntitiesToValidate.Length; i++)
        {
            EntitiesToValidate[i] = new int[dioramaInfos.puzzleInfos[i].entitiesAmount];
            dioramaInfos.puzzleInfos[i].winConditionIsMet = false;

            //Debug.Log($"{dioramaInfos.puzzleInfos[i].puzzleName} is of size {dioramaInfos.puzzleInfos[i].entitiesAmount} and at index {i}");
            for (int j = 0; j < EntitiesToValidate[i].Length; j++)
            {
                EntitiesToValidate[i][j] = 0;
            }
        }
    }

    // callback lorsqu'un ourson/bouton est touché ou tuyau dans le bon sens
    public void ValidateActivationState()
    {

    }

    public void ValidateWinCondition(int array, int index)
    {
        Debug.Log($"puzzle {(DioramaPuzzleName)array} has validated item n° {index + 1}");

        // a puzzle piece is set to true (==1)
        EntitiesToValidate[array][index] = 1;
        dioramaInfos.puzzleInfos[array].winConditionIsMet = true; // by default, we assume the puzzle is finished

        // check if all the pieces of the puzzle are validated (== 1)
        for (int i = 0; i < EntitiesToValidate[array].Length; i++)
        {
            // if one or more puzzle pieces are still true, the puzzle is not finished yet
            if (EntitiesToValidate[array][i] == 0)
            {
                dioramaInfos.puzzleInfos[array].winConditionIsMet = false;
            }
        }

        // if all the puzzle pieces are valid, the puzzle is completed
        if (dioramaInfos.puzzleInfos[array].winConditionIsMet == true)
        {
            Debug.Log($"puzzle {(DioramaPuzzleName)array} is finished");
            validatedPuzzleAmount++;
            ActivatePuzzleCompleteVFX(array); 

            TriggerStarPhase(0, validatedPuzzleAmount - 1); // PhaseHolderName.Etoile

            if (validatedPuzzleAmount == dioramaInfos.puzzleAmount)
            {
                Debug.Log("Level is FINISHED"); // debug
                LevelIsFinished = true; 
                // show victory UI
            }
        }
    }

    private void ActivatePuzzleCompleteVFX(int index)
    {
        Debug.Log("Puzzle complete vfx"); 
        puzzleCompleteVFXS[index].SetActive(true); 
    }

    public void InvalidateWinCondition(int array, int index)
    {
        Debug.Log($"puzzle {(DioramaPuzzleName)array} has invalidated item n° {index + 1}");

        EntitiesToValidate[array][index] = 0;
    }

    #region Phases
        #region Star
    private float currentDissolveAmount, minDissolveAmount, maxDissolveAmount;
    private Material dissolveMaterial; 
    public void TriggerStarPhase(PhaseHolderName phaseHolderName, int phaseNumber)
    {
        maxDissolveAmount = validatedPuzzleAmount == 1 ? 0.3f : validatedPuzzleAmount == 2 ? 0.4f : 1;

        if (validatedPuzzleAmount == 1)
        {
            dissolveMaterial = phaseHolders[(int)phaseHolderName].phases[phaseNumber].materialsToSet[0];
        }
        else if (validatedPuzzleAmount == 3)
        {
            phaseHolders[(int)phaseHolderName].phases[phaseNumber].objToSet[0].SetActive(false);
            phaseHolders[(int)phaseHolderName].phases[phaseNumber].objToSet[1].SetActive(true);

            phaseHolders[(int)phaseHolderName].phases[phaseNumber].scriptsToSet[0].enabled = true;
        }

        StartCoroutine(LerpStarDissolve(phaseHolderName, phaseNumber)); 
    }

    readonly WaitForFixedUpdate waitForFixedUpdate;
    private readonly float dissolveDuration = 2f;
    private float currentTime; 
    private System.Collections.IEnumerator LerpStarDissolve(PhaseHolderName phaseHolderName, int phaseNumber) // turn into async routine or separate thread
    {
        yield return waitForFixedUpdate;
        currentDissolveAmount = Mathf.Lerp(minDissolveAmount, maxDissolveAmount, currentTime);
        currentTime += Time.fixedDeltaTime / dissolveDuration;

        dissolveMaterial.SetFloat("DissolveAmount", currentDissolveAmount); 

        if (currentTime <= dissolveDuration)
        {
            StartCoroutine(LerpStarDissolve(phaseHolderName, phaseNumber));
        }
        else 
        {
            minDissolveAmount = maxDissolveAmount;
            currentTime = 0f; 
            StopCoroutine(LerpStarDissolve(phaseHolderName, phaseNumber)); 
        }
    }
    #endregion
    #endregion
}

#region Phase Holders
// "event" -> résolution de puzzle, selection d'ourson, tuyaux bien lock
// Bouche à incendie, Tuyaux, Manège, Etoile, Feu d'artifice    
public enum PhaseHolderName { NONE = -1, BoucheIncendie, Tuyau, Manège, Etoile, FeuArtifice}
[System.Serializable]
public class PhaseHolder
{
    [SerializeField] private string editorName; // for editor readability, instead of element 0 etc..
    public PhaseHolderName phaseHolderName;
    public List<Phase> phases = new(); 
}

[System.Serializable]
public class Phase
{
    public int phaseNumber; // index de tes array ? Sinon à virer
    public List<MonoBehaviour> scriptsToSet = new();
    public List<GameObject> objToSet = new();
    public List<ParticleSystem> particlesToSet = new();
    public List<Material> materialsToSet = new();
}

#endregion
