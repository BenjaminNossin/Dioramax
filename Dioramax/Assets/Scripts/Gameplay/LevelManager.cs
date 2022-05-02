using UnityEngine;
using System.Collections.Generic; 

// abstract this out to have one for each level
public enum GameState { NONE, Loading, Playing, Paused, Cinematic }
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public static GameState GameState { get; private set; }

    [Header("General")]
    [SerializeField] private DioramaInfos dioramaInfos;
    [SerializeField] private GameObject objToDeactivateOnLevelEnd;
    [SerializeField, Range(0f, 5f)] private float phase2to3Delay = 1f; 

    [Space, SerializeField] private PhaseHolder[] phaseHolders = new PhaseHolder[5];
    [SerializeField] private GameObject[] puzzleCompleteVFXS = new GameObject[3];

    [Header("Tuyaux")]
    [SerializeField] private ParticleSystem[] reussiteTuyauxVFX;
    [SerializeField] private MonoBehaviour buttonTweenTouch; // stil WIP because logically hard to do with this version of puzzle mechanic
                                                             // Desactiver le script TweenTouch sur le(s) bouton(s)  une fois qu’il n’est plus utile pour le puzzle et remettre sa/leurs scale à 1 1 1 (1 2 1 avant)

    public static int[][] EntitiesToValidate { get; set; }
    private byte validatedPuzzleAmount;
    public static bool LevelIsFinished { get; private set; }

    [Header("Carrousel")]
    [SerializeField] GameObject carrouselToit; 

    [Header("CAMERA")]
    [SerializeField] private Transform cameraCrane;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform decoyCamera;
    [SerializeField] private Transform cameraTransformOnPhase2;

    [Header("DEBUG")]
    public bool overridePhaseSystem = true; 

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
        // PALCEHOLDER
        phaseHolders[0].phases[0].materialsToSet[0].SetFloat("DissolveAmount", 0);

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

    public void SetGameState(GameState gameState)
    {
        Debug.Log($"gamestate is now {gameState}."); 
        GameState = gameState; 
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
        // carrousel CANNOT be the first validated puzzle
        if (dioramaInfos.puzzleInfos[array].winConditionIsMet == true)
        {
            Debug.Log($"puzzle {(DioramaPuzzleName)array} is finished");
            LevelInfosUI.Instance.ActivatePuzzleUIOnWin(array); 
            validatedPuzzleAmount++;

            TriggerStarPhase(PhaseHolderName.Etoile, validatedPuzzleAmount - 1); // PhaseHolderName.Etoile
            ActivatePuzzleCompleteVFX(array);
            if (!overridePhaseSystem) 
            {
                TriggerBoucheIncendiePhase(PhaseHolderName.BoucheIncendie, validatedPuzzleAmount - 1); 
            }

            if (validatedPuzzleAmount == dioramaInfos.puzzleAmount)
            {
                Debug.Log("Level is FINISHED"); // debug
                LevelIsFinished = true;
                objToDeactivateOnLevelEnd.SetActive(false);
            }
        }
    }

    public void InvalidateWinCondition(int array, int index)
    {
        Debug.Log($"puzzle {(DioramaPuzzleName)array} has invalidated item n° {index + 1}");

        EntitiesToValidate[array][index] = 0;
    }

    private void ActivatePuzzleCompleteVFX(int index)
    {
        Debug.Log("Puzzle complete vfx");
        puzzleCompleteVFXS[index].SetActive(true);
    }

    public void OnTuyauxValidPosition(int index)
    {
        Debug.Log("tuyaux valid position fx"); 
        reussiteTuyauxVFX[index].Play(); 
        // Activer le particle system “VFX_ReussiteTuyau” a chaque fois qu’un tuyau est encastré dans la position correcte (enfant du tuyau correspondant)
        // Desactiver le script TweenTouch sur le(s) bouton(s)  une fois qu’il n’est plus utile pour le puzzle et remettre sa / leurs scale à 1 1 1(1 2 1 avant)
    }

    #region Phases
        #region Star
    private float currentDissolveAmount, minDissolveAmount, maxDissolveAmount;
    private Material dissolveMaterial; 
    public void TriggerStarPhase(PhaseHolderName phaseHolderName, int phaseNumber)
    {
        maxDissolveAmount = validatedPuzzleAmount == 1 ? 0.3f : validatedPuzzleAmount == 2 ? 0.4f : 1;

        // NEED REFACTORING
        if (validatedPuzzleAmount == 1)
        {
            dissolveMaterial = phaseHolders[(int)phaseHolderName].phases[phaseNumber].materialsToSet[0];
        }
        else if (validatedPuzzleAmount == 3)
        {
            // PLACEHOLDER
            phaseHolders[(int)phaseHolderName].phases[phaseNumber].objToSet[0].GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(DelayFinish(phaseHolderName, phaseNumber));
        }

        StartCoroutine(LerpStarDissolve(phaseHolderName, phaseNumber)); 
    }

    // wait end of dissolve
    // here, add a cinematic camera focus if needed
    private System.Collections.IEnumerator DelayFinish(PhaseHolderName phaseHolderName, int phaseNumber)
    {
        yield return new WaitForSeconds(1f); 
        phaseHolders[(int)phaseHolderName].phases[phaseNumber].objToSet[0].SetActive(false);
        phaseHolders[(int)phaseHolderName].phases[phaseNumber].objToSet[1].SetActive(true);

        // Tween_Star_Finish still WIP
        // phaseHolders[(int)phaseHolderName].phases[phaseNumber].scriptsToSet[0].enabled = true;
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

        #region Bouche d'Incendie
    public void TriggerBoucheIncendiePhase(PhaseHolderName phaseHolderName, int phaseNumber)
    {
        // NEED REFACTORING
        if (validatedPuzzleAmount == 1)
        {
            ActivateBoucheIncendiePhase1(phaseHolderName, phaseNumber);
        }
        else if (validatedPuzzleAmount == 2)
        {
            // phase 3 done within phase 2
            ActivateBoucheIncendiePhase2(phaseHolderName, phaseNumber); 
        }
    }

    private void ActivateBoucheIncendiePhase1(PhaseHolderName phaseHolderName, int phaseNumber)
    {
        for (int i = 0; i < phaseHolders[(int)phaseHolderName].phases[phaseNumber].scriptsToSet.Count; i++)
        {
            phaseHolders[(int)phaseHolderName].phases[phaseNumber].scriptsToSet[i].enabled = true;
        }
    }

    private void ActivateBoucheIncendiePhase2(PhaseHolderName phaseHolderName, int phaseNumber)
    {
        phaseHolders[(int)phaseHolderName].phases[phaseNumber].objToSet[0].SetActive(true);

        // activate cinematic 
        // CameraCinematic.Instance.PlayCinematic(); 

        // PLACEHOLDER
        cameraCrane.SetPositionAndRotation(cameraCrane.position, Quaternion.Euler(26.5f, -36f, 0f));
        mainCamera.transform.position = cameraTransformOnPhase2.position;
        mainCamera.transform.localRotation = Quaternion.identity;
        decoyCamera.transform.position = cameraTransformOnPhase2.position;
        decoyCamera.transform.localRotation = Quaternion.identity;

        StartCoroutine(SimulateBoucheIncendiePhase2Cinematic());

        for (int i = 0; i < phaseHolders[(int)phaseHolderName].phases[phaseNumber].scriptsToSet.Count; i++)
        {
            if (i < 2)
            {
                Debug.Log($"setting {i} to false");
                phaseHolders[(int)phaseHolderName].phases[phaseNumber].scriptsToSet[i].enabled = false;
            }
            else
            {
                Debug.Log($"setting {i} to true");
                phaseHolders[(int)phaseHolderName].phases[phaseNumber].scriptsToSet[i].enabled = true;
            }
        }

        StartCoroutine(ActivateBoucheIncendiePhase3(phaseHolderName, phaseNumber + 1));
    }

    private System.Collections.IEnumerator ActivateBoucheIncendiePhase3(PhaseHolderName phaseHolderName, int phaseNumber)
    {
        yield return new WaitForSeconds(phase2to3Delay);

        for (int i = 0; i < phaseHolders[(int)phaseHolderName].phases[phaseNumber].scriptsToSet.Count; i++)
        {
            if (i == 0)
            {
                phaseHolders[(int)phaseHolderName].phases[phaseNumber].scriptsToSet[i].enabled = false;
            }
            else
            {
                phaseHolders[(int)phaseHolderName].phases[phaseNumber].scriptsToSet[i].enabled = true;
            }
        }

        phaseHolders[(int)phaseHolderName].phases[phaseNumber].particlesToSet[0].Play();
        phaseHolders[(int)phaseHolderName].phases[phaseNumber].collidersToSet[0].enabled = false; // bears can now be detected  
    }

    // PLACEHOLDER
    private System.Collections.IEnumerator SimulateBoucheIncendiePhase2Cinematic()
    {
        GameState = GameState.Cinematic;

        yield return new WaitForSeconds(1f);
        carrouselToit.SetActive(false); 

        yield return new WaitForSeconds(2f);
        cameraCrane.SetPositionAndRotation(cameraCrane.position, Quaternion.identity);
        mainCamera.transform.position = new Vector3(0f, 0f, -45f);
        mainCamera.transform.localRotation = Quaternion.identity;
        decoyCamera.transform.position = new Vector3(0f, 0f, -45f);
        decoyCamera.transform.localRotation = Quaternion.identity;

        GameState = GameState.Playing; 
    }

        #endregion

    #endregion
}

#region Phase Holders
// "event" -> résolution de puzzle, selection d'ourson, tuyaux bien lock
// Bouche à incendie, Tuyaux, Manège, Etoile, Feu d'artifice    
public enum PhaseHolderName { NONE = -1, Etoile, BoucheIncendie, Manège }
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
    public List<Collider> collidersToSet = new();
}

#endregion
