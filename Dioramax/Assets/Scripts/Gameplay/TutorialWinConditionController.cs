using UnityEngine;

// because there is too much hardcoded stuff in the LevelManager. This should only be temporary
public class TutorialWinConditionController : MonoBehaviour
{
    [SerializeField] private DioramaInfos dioramaInfos;
    private int tutorialPhase;
    public static bool Phase1IsRead { get; set; }

    [Space, SerializeField, Range(0f, 1f)] private float promptDisappearDelay = 0.5f;
    [SerializeField] private AudioSource catButtonAudioSource;

    // XY rotation
    [Header("XY Rotation Tutorial")]
    [SerializeField, Range(0.5f, 5f)] private float requiredRotationDuration = 1f;
    private float xyRotationCounter;

    // Zoom
    [Header("Zoom Tutorial")]
    [SerializeField, Range(0.5f, 5f)] private float requiredIndividualZoomDuration = 1f;
    private float zoomOutCounter, zoomInCounter;

    // Z Rotation
    [Header("Z Rotation Tutorial")]
    [SerializeField] private GameObject cameraCraneZRotation;
    [SerializeField, Range(0.5f, 5f)] private float requiredIndividualZRotation = 1f;
    private float zRotationLeftCounter, zRotationRightCounter;
    private bool cameraIsActive; 

    // Unfreeze
    [Header("Unfreeze")]
    [SerializeField] private Collider ballCollider;

    [Header("Final")]
    [SerializeField] private Collider starCollider;
    [SerializeField] private Tween_Star_Finish tweenStarFinish;


    /* [Header("-- DEBUG --")]
    [SerializeField] private bool overridePhaseIndex; 
    [SerializeField] private int fakePhase; */


    private void OnEnable()
    {
        TouchDetection.OnTutorialButtonDetection += TutorialButtonDetected;
        HideObjectOnTriggerEnter.OnBallTutorialComplete += TutorialBallDetected;
        LevelManager.OnTutorialTweenStarFinish += ActivateOnFinish; 
    }

    private void OnDisable()
    {
        TouchDetection.OnTutorialButtonDetection -= TutorialButtonDetected;
        HideObjectOnTriggerEnter.OnBallTutorialComplete -= TutorialBallDetected;
        LevelManager.OnTutorialTweenStarFinish -= ActivateOnFinish;
    }

    private void Start()
    {
        cameraCraneZRotation.SetActive(false);
        ballCollider.enabled = false;
        starCollider.enabled = false;
        tweenStarFinish.enabled = false; 
    }

    void FixedUpdate()
    {
        if (LevelManager.GameState != GameState.Playing) return; 

        if (tutorialPhase == 0)
        {
            // XY rotation

            if (DioravityCameraCraneRotation.YXRotation)
            {
                xyRotationCounter += 0.02f;
                if (xyRotationCounter >= requiredRotationDuration)
                {
                    // winConditionHolders[tutorialPhase].winCondition.SetWinCondition(true);
                    GameLogger.Log("suceeded XY rotation tutorial");
                    tutorialPhase = 1;
                    TutorialPromptsUI.Instance.ShowNextPrompt(tutorialPhase, 0);
                }
            }
        }
        else if (tutorialPhase == 1)
        {
            // Zoom

            if (CameraZoom.ZoomingIn)
            {
                zoomInCounter += 0.02f;
            }

            if (CameraZoom.ZoomingOut)
            {
                zoomOutCounter += 0.02f;
            }

            if (zoomInCounter >= requiredIndividualZoomDuration && zoomOutCounter >= requiredIndividualZoomDuration)
            {
                GameLogger.Log("suceeded Zoom tutorial");

                tutorialPhase = 2;
                TutorialPromptsUI.Instance.ShowNextPrompt(tutorialPhase, 0);
            }
        }
        else if (tutorialPhase == 2)
        {
            // Z Rotation
            if (!cameraIsActive)
            {
                cameraIsActive = true; 
                cameraCraneZRotation.SetActive(true);
            }

            if (!ZRotationButton.ButtonIsSelected) return;

            if (ZRotationButton.LeftIsSelected)
            {
                zRotationLeftCounter += 0.02f;
            }

            if (ZRotationButton.RightIsSelected)
            {
                zRotationRightCounter += 0.02f;
            }

            if (zRotationRightCounter >= requiredIndividualZRotation && zRotationLeftCounter >= requiredIndividualZRotation)
            {
                GameLogger.Log("suceeded Z Rotation tutorial");
                tutorialPhase = 3;
                TutorialPromptsUI.Instance.ShowNextPrompt(tutorialPhase, 0);
            }
        }
        else if (tutorialPhase == 4) 
        {
            // Unfreeze
            ballCollider.enabled = true;
        }
    }

    private void TutorialButtonDetected()
    {
        if (tutorialPhase != 3) return; 

        GameLogger.Log("suceeded Button Touch tutorial");
        catButtonAudioSource.Play();

        LevelInfosUI.Instance.ActivatePuzzleUIOnWin(0);

        LevelManager.Instance.ValidatedPuzzleAmount = 1;
        LevelManager.Instance.ActivatePuzzleCompleteVFX(0);
        LevelManager.Instance.TriggerStarPhase(PhaseHolderName.Etoile, 0);

        tutorialPhase = 4;
        TutorialPromptsUI.Instance.ShowNextPrompt(tutorialPhase, 0);
    }

    private void TutorialBallDetected()
    {
        if (tutorialPhase != 4) return;

        GameLogger.Log("suceeded Unfreeze tutorial");
        LevelInfosUI.Instance.ActivatePuzzleUIOnWin(1);

        LevelManager.Instance.ValidatedPuzzleAmount = 2;
        LevelManager.Instance.ActivatePuzzleCompleteVFX(1);
        LevelManager.Instance.TriggerStarPhase(PhaseHolderName.Etoile);

        tutorialPhase = 5;
        TutorialPromptsUI.Instance.ShowNextPrompt(tutorialPhase, 0);

        LevelManager.LevelIsFinished = true;
        starCollider.enabled = true; 
    }

    private void ActivateOnFinish()
    {
        tweenStarFinish.enabled = true;
    }
}
