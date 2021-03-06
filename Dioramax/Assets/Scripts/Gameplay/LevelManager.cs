using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

// abstract this out to have one for each level
public enum GameState { NONE, Loading, Playing, Paused, Cinematic }
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public static GameState GameState { get; private set; }

    [Header("General")]
    [SerializeField] private DioramaInfos currentDioramaInfos;
    [SerializeField] private DioramaInfos previousDioramaInfos;

    [SerializeField] private DioramaName dioramaName;
    [SerializeField] private List<GameObject> objToDeactivateOnLevelEnd = new(); 
    [SerializeField, Range(0f, 5f)] private float phase2to3Delay = 1f;
    [SerializeField] private float[] shieldDissolveAmountPerPhase = new float[] { 0.3f, 0.4f }; 

    [Space, SerializeField] private PhaseHolder[] phaseHolders = new PhaseHolder[5];
    [SerializeField] private GameObject[] puzzleCompleteVFXS = new GameObject[3];
    public static System.Action OnTutorialTweenStarFinish; 

    [Header("Tuyaux")]
    [SerializeField] private ParticleSystem[] reussiteTuyauxVFX;

    public static int[][] EntitiesToValidate { get; set; }
    public byte ValidatedPuzzleAmount { get; set; }
    public static bool LevelIsFinished { get; set; } // should be private set. Changed because of tutorial
    public static bool IsPhase3 { get; private set; }

    [Header("Carrousel")]
    [SerializeField] GameObject ratAnimationObj;
    public static int OverrideWinConditionNumber { get; set; }

    [Header("Hydrant")]
    [SerializeField] private AudioSource hydrantAudioSource;
    [SerializeField] private AudioClip rumble;
    [SerializeField] private AudioClip flow;
    [SerializeField] private AudioMixerGroup rumbleGroup;
    [SerializeField] private AudioMixerGroup flowGroup;


    [Header("CAMERA")]
    [SerializeField] private Transform cameraCrane;
    [SerializeField] private Transform cameras;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform decoyCamera;
    [SerializeField] private Transform cameraTransformOnPhase2;

    [Header("AUDIO")]
    [SerializeField] private AudioSource shieldDissolveAudiosource; 

    [Header("DEBUG")]
    public bool skipIntroCinematic; 
    public bool overridePhaseSystem;

    #region Unity Callbacks

    void Awake()
    {
        LevelIsFinished = false;
        IsPhase3 = false;
        OverrideWinConditionNumber = 0; 

        if (Instance != null)
        {
            Destroy(this);
        }

        Instance = this;
    }

    private void OnEnable()
    {
        CameraCinematic.OnFadeOutComplete += OnFadeOutComplete;
        CameraCinematic.OnFadeInComplete += OnFadeInComplete;
    }

    private void OnDisable()
    {
        CameraCinematic.OnFadeOutComplete -= OnFadeOutComplete;
        CameraCinematic.OnFadeInComplete -= OnFadeInComplete;
    }

    private int offset;
    private void Start()
    {
        offset = previousDioramaInfos ? previousDioramaInfos.puzzleAmount : 0;

        if (dioramaName != DioramaName.Diorama2) // waiting to have dissolve material on diorama 2
        {
            dissolveMaterial = phaseHolders[(int)PhaseHolderName.Etoile].phases[0].materialsToSet[0];
            dissolveMaterial.SetFloat("DissolveAmount", 0);
        }

        if (skipIntroCinematic)
        {
            CameraCinematic.Instance.SetAnimatorState(0);
            SetGameState(GameState.Playing);

            if (dioramaName == DioramaName.Tutorial)
            {
                TutorialPromptsUI.OverridenCinematic = true; 
                TutorialPromptsUI.Instance.ShowStartingPrompt();
            }
        }

        // PALCEHOLDER
        if (dioramaName == DioramaName.Diorama1)
        {
            phaseHolders[0].phases[0].materialsToSet[0].SetFloat("DissolveAmount", 0);

            // WIP : initializing all at 0. This will be changed by loading saved game state
            EntitiesToValidate = new int[currentDioramaInfos.puzzleAmount][];
            for (int i = 0; i < EntitiesToValidate.Length; i++)
            {
                EntitiesToValidate[i] = new int[currentDioramaInfos.puzzleInfos[i].entitiesAmount];
                currentDioramaInfos.puzzleInfos[i].winConditionIsMet = false;

                //GameLogger.Log($"{dioramaInfos.puzzleInfos[i].puzzleName} is of size {dioramaInfos.puzzleInfos[i].entitiesAmount} and at index {i}");
                for (int j = 0; j < EntitiesToValidate[i].Length; j++)
                {
                    EntitiesToValidate[i][j] = 0;
                }
            }
        }
    }
    #endregion

    public void SetGameState(GameState gameState)
    {
        GameLogger.Log($"gamestate is now {gameState}."); 
        GameState = gameState; 
    }

    public void ValidateWinCondition(int array, int index)
    {
        GameLogger.Log($"puzzle {(DioramaPuzzleName)array} has validated item n? {index + 1}");
        array -= offset; 

        // a puzzle piece is set to true (==1)
        EntitiesToValidate[array][index] = 1;
        currentDioramaInfos.puzzleInfos[array].winConditionIsMet = true; // by default, we assume the puzzle is finished

        // check if all the pieces of the puzzle are validated (== 1)
        for (int i = 0; i < EntitiesToValidate[array].Length; i++)
        {
            // if one or more puzzle pieces are still true, the puzzle is not finished yet
            if (EntitiesToValidate[array][i] == 0)
            {
                currentDioramaInfos.puzzleInfos[array].winConditionIsMet = false;
            }
        }

        // if all the puzzle pieces are valid, the puzzle is completed
        // carrousel CANNOT be the first validated puzzle
        if (currentDioramaInfos.puzzleInfos[array].winConditionIsMet == true)
        {
            GameLogger.Log($"puzzle {(DioramaPuzzleName)array} is finished");
            LevelInfosUI.Instance.ActivatePuzzleUIOnWin(array); 
            ValidatedPuzzleAmount++;

            ActivatePuzzleCompleteVFX(array);
            if (!overridePhaseSystem) 
            {
                TriggerBoucheIncendiePhase(PhaseHolderName.BoucheIncendie, ValidatedPuzzleAmount - 1); 
            }

            if (ValidatedPuzzleAmount == currentDioramaInfos.puzzleAmount)
            {
                GameLogger.Log("Level is FINISHED"); 
                LevelIsFinished = true;
            }
        }
    }

    public void DeactivateObjectsOnLevelEnd()
    {
        foreach (GameObject go in objToDeactivateOnLevelEnd)
        {
            go.SetActive(false);
        }
    }

    public void InvalidateWinCondition(int array, int index)
    {
        GameLogger.Log($"puzzle {(DioramaPuzzleName)array} has invalidated item n? {index + 1}");

        EntitiesToValidate[array][index] = 0;
    }

    public void ActivatePuzzleCompleteVFX(int index)
    {
        GameLogger.Log("Puzzle complete vfx");
        puzzleCompleteVFXS[index].SetActive(true);
    }

    public void OnTuyauxValidPosition(int index)
    {
        GameLogger.Log("tuyaux valid position fx"); 
        reussiteTuyauxVFX[index].Play(); 
        // Activer le particle system ?VFX_ReussiteTuyau? a chaque fois qu?un tuyau est encastr? dans la position correcte (enfant du tuyau correspondant)
        // Desactiver le script TweenTouch sur le(s) bouton(s)  une fois qu?il n?est plus utile pour le puzzle et remettre sa / leurs scale ? 1 1 1(1 2 1 avant)
    }

    #region Phases
        #region Star
    private float currentDissolveAmount, minDissolveAmount, maxDissolveAmount;
    private Material dissolveMaterial; 
    public void TriggerStarPhase(PhaseHolderName phaseHolderName, int phaseNumber = -1)
    {
        maxDissolveAmount = ValidatedPuzzleAmount == 1 ? shieldDissolveAmountPerPhase[0] : 1;
        if (dioramaName != DioramaName.Tutorial)
        {
            maxDissolveAmount = ValidatedPuzzleAmount == 1 ? shieldDissolveAmountPerPhase[0] : ValidatedPuzzleAmount == 2 ? shieldDissolveAmountPerPhase[1] : 1;
            Debug.Log($"max dissolve amount : {maxDissolveAmount}");
        }

        if (ValidatedPuzzleAmount == 3)
        {
            // PLACEHOLDER
            GameLogger.Log("star final phase");

            phaseHolders[(int)phaseHolderName].phases[phaseNumber].objToSet[0].GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(DelayFinish(phaseHolderName, phaseNumber));
        }

        shieldDissolveAudiosource.Play();
        StartCoroutine(LerpStarDissolve()); 
    }

    // wait end of dissolve
    // here, add a cinematic camera focus if needed
    private System.Collections.IEnumerator DelayFinish(PhaseHolderName phaseHolderName, int phaseNumber)
    {
        yield return new WaitForSeconds(1f); 
        phaseHolders[(int)phaseHolderName].phases[phaseNumber].objToSet[0].SetActive(false);
        phaseHolders[(int)phaseHolderName].phases[phaseNumber].objToSet[1].SetActive(true);
        phaseHolders[(int)phaseHolderName].phases[phaseNumber].scriptsToSet[0].enabled = true;
    }

    readonly WaitForFixedUpdate waitForFixedUpdate;
    private readonly float dissolveDuration = 2f;
    private float currentTime; 
    private System.Collections.IEnumerator LerpStarDissolve() // turn into async routine or separate thread
    {
        yield return waitForFixedUpdate;

        currentDissolveAmount = Mathf.Lerp(minDissolveAmount, maxDissolveAmount, currentTime);
        currentTime += Time.fixedDeltaTime / dissolveDuration;

        dissolveMaterial.SetFloat("DissolveAmount", currentDissolveAmount); 

        if (currentTime <= dissolveDuration)
        {
            StartCoroutine(LerpStarDissolve());
        }
        else 
        {
            minDissolveAmount = maxDissolveAmount;
            currentTime = 0f; 
            StopCoroutine(LerpStarDissolve());

            if (dioramaName == DioramaName.Tutorial && ValidatedPuzzleAmount == currentDioramaInfos.puzzleAmount)
            {
                OnTutorialTweenStarFinish(); 
            }
        }
    }
    #endregion

        #region Bouche d'Incendie

    PhaseHolderName secondPuzzlePhaseHolderName;
    int secondPuzzlePhaseNumber;
    public void TriggerBoucheIncendiePhase(PhaseHolderName _phaseHolderName, int _phaseNumber)
    {
        // NEED REFACTORING
        if (ValidatedPuzzleAmount == 1)
        {
            // change to have something similar to phase 2 and better see star fade ?
            ActivateBoucheIncendiePhase1(_phaseHolderName, _phaseNumber);
        }
        else if (ValidatedPuzzleAmount == 2)
        {
            // phase 3 done within phase 2
            CameraCinematic.Instance.FadeCameraPanel();
            secondPuzzlePhaseHolderName = _phaseHolderName;
            secondPuzzlePhaseNumber = _phaseNumber; 
        }
        else if (ValidatedPuzzleAmount == 3)
        {
            TriggerStarPhase(PhaseHolderName.Etoile, 2);
        }
    }

    private void ActivateBoucheIncendiePhase1(PhaseHolderName phaseHolderName, int phaseNumber)
    {
        hydrantAudioSource.clip = rumble;
        hydrantAudioSource.outputAudioMixerGroup = rumbleGroup;
        hydrantAudioSource.Play();

        TriggerStarPhase(PhaseHolderName.Etoile, phaseNumber); // PLAYER SHOULD SEE THIS. temporary placement

        for (int i = 0; i < phaseHolders[(int)phaseHolderName].phases[phaseNumber].scriptsToSet.Count; i++)
        {
            phaseHolders[(int)phaseHolderName].phases[phaseNumber].scriptsToSet[i].enabled = true;
        }
    }

    // teleport cameras
    private void OnFadeOutComplete()
    {
        Debug.Log("teleporting camera on fade out complete"); 
        cameras.transform.position = cameraTransformOnPhase2.position;
        Debug.Log($"new position is {cameraTransformOnPhase2.position}"); 
        cameras.transform.localRotation = Quaternion.Euler(32.5f, 1.54f, 0f);
    }

    // star fade and THEN bouche d'incendie animation
    private void OnFadeInComplete()
    {
        Debug.Log("triggering star phase on fade in complete"); 
        TriggerStarPhase(PhaseHolderName.Etoile, ValidatedPuzzleAmount - 1); // PLAYER SHOULD ALWAYS SEE THIS
        StartCoroutine(ActivateSecondPhase());
    }

    // call this from animEvent if possible
    private WaitForSeconds WFS = new(0.5f);
    private System.Collections.IEnumerator ActivateSecondPhase()
    {
        yield return WFS;
        ActivateBoucheIncendiePhase2(secondPuzzlePhaseHolderName, secondPuzzlePhaseNumber);
        CameraCinematic.Instance.PlayPhase2Cinematic();
    }

    private void ActivateBoucheIncendiePhase2(PhaseHolderName phaseHolderName, int phaseNumber)
    {
        hydrantAudioSource.clip = flow;
        hydrantAudioSource.outputAudioMixerGroup = flowGroup;
        hydrantAudioSource.Play();

        phaseHolders[(int)phaseHolderName].phases[phaseNumber].objToSet[0].SetActive(true);

        // activate cinematic 
        // CameraCinematic.Instance.PlayCinematic(); 

        ratAnimationObj.SetActive(true);

        for (int i = 0; i < phaseHolders[(int)phaseHolderName].phases[phaseNumber].scriptsToSet.Count; i++)
        {
            if (i < 2)
            {
                // GameLogger.Log($"setting {i} to false");
                phaseHolders[(int)phaseHolderName].phases[phaseNumber].scriptsToSet[i].enabled = false;
            }
            else
            {
                // GameLogger.Log($"setting {i} to true");
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
        IsPhase3 = true; 
    }

        #endregion

    #endregion
}

#region Phase Holders
// "event" -> r?solution de puzzle, selection d'ourson, tuyaux bien lock
// Bouche ? incendie, Tuyaux, Man?ge, Etoile, Feu d'artifice    
public enum PhaseHolderName { NONE = -1, Etoile, BoucheIncendie, Man?ge }
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
    public int phaseNumber; // index de tes array ? Sinon ? virer
    public List<MonoBehaviour> scriptsToSet = new();
    public List<GameObject> objToSet = new();
    public List<ParticleSystem> particlesToSet = new();
    public List<Material> materialsToSet = new();
    public List<Collider> collidersToSet = new();
}

#endregion
