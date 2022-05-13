using UnityEngine;

// because there is too much hardcoded stuff in the LevelManager. This should only be temporary
public class TutorialWinConditionController : MonoBehaviour
{
    [SerializeField] private DioramaInfos dioramaInfos; 
    private int tutorialPhase;
    public static bool OverrideIsDone { get; set; }

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

    // Touch
    [Header("Touch")]
    [SerializeField] private Collider buttonCollider;

    // Unfreeze
    [Header("Unfreeze")]
    [SerializeField] private Collider ballCollider;

    [Header("Final")]
    [SerializeField] private Collider starCollider;

    /* [Header("-- DEBUG --")]
    [SerializeField] private bool overridePhaseIndex; 
    [SerializeField] private int fakePhase; */


    private void OnEnable()
    {
        TouchDetection.OnTutorialButtonDetection += TutorialButtonDetected;
        HideObjectOnTriggerEnter.OnBallTutorialComplete += TutorialBallDetected;
    }

    private void OnDisable()
    {
        TouchDetection.OnTutorialButtonDetection -= TutorialButtonDetected;
        HideObjectOnTriggerEnter.OnBallTutorialComplete -= TutorialBallDetected;
    }

    private void Start()
    {
        cameraCraneZRotation.SetActive(false);
        buttonCollider.enabled = false;
        ballCollider.enabled = false;
        starCollider.enabled = false;
    }

    void Update()
    {
        if (LevelManager.GameState != GameState.Playing) return; 

        /* if (overridePhaseIndex && !OverrideIsDone)
        {
            tutorialPhase = fakePhase; 

            if (fakePhase == 3)
            {
                TutorialButtonDetected(); 
            }
            else if (fakePhase >= 4)
            {
                LevelInfosUI.Instance.ActivatePuzzleUIOnWin(0);
                TutorialBallDetected(); 
            }
        } */

        if (tutorialPhase == 0)
        {
            // XY rotation

            if (DioravityCameraCraneRotation.YXRotation)
            {
                xyRotationCounter += 0.02f;

                if (!OverrideIsDone && xyRotationCounter >= requiredRotationDuration * 0.5f)
                {
                    OverrideIsDone = true;
                    TutorialPromptsUI.Instance.OverrideHidePanelDelay(); 
                }

                if (xyRotationCounter >= requiredRotationDuration)
                {
                    // winConditionHolders[tutorialPhase].winCondition.SetWinCondition(true);
                    GameLogger.Log("suceeded XY rotation tutorial");

                    OverrideIsDone = false;
                    tutorialPhase = 1;
                    TutorialPromptsUI.Instance.ShowPrompt(tutorialPhase, 0);
                }
            }
        }
        else if (tutorialPhase == 1)
        {
            // Zoom

            if (CameraZoom.ZoomingIn)
            {
                zoomInCounter += 0.02f;

                if (!OverrideIsDone && zoomInCounter >= requiredIndividualZoomDuration * 0.4f)
                {
                    OverrideIsDone = true;
                    Debug.Log("zooming out"); 
                    TutorialPromptsUI.Instance.OverrideHidePanelDelay();
                }
            }

            if (CameraZoom.ZoomingOut)
            {
                zoomOutCounter += 0.02f;

                if (!OverrideIsDone && zoomOutCounter >= requiredIndividualZoomDuration * 0.4f)
                {
                    OverrideIsDone = true;
                    Debug.Log("zooming in");
                    TutorialPromptsUI.Instance.OverrideHidePanelDelay();
                }
            }

            if (zoomInCounter >= requiredIndividualZoomDuration && zoomOutCounter >= requiredIndividualZoomDuration)
            {
                GameLogger.Log("suceeded Zoom tutorial");

                OverrideIsDone = false;
                tutorialPhase = 2;
                TutorialPromptsUI.Instance.ShowPrompt(tutorialPhase, 0);
            }
        }
        else if (tutorialPhase == 2)
        {
            cameraCraneZRotation.SetActive(true);
            // Z Rotation

            if (ZRotationButton.LeftIsSelected)
            {
                zRotationLeftCounter += 0.02f;

                if (!OverrideIsDone && zRotationLeftCounter >= requiredIndividualZRotation * 0.4f)
                {
                    OverrideIsDone = true;
                    TutorialPromptsUI.Instance.OverrideHidePanelDelay();
                }
            }

            if (ZRotationButton.RightIsSelected)
            {
                zRotationRightCounter += 0.02f;

                if (!OverrideIsDone && zRotationRightCounter >= requiredIndividualZRotation * 0.4f)
                {
                    OverrideIsDone = true;
                    TutorialPromptsUI.Instance.OverrideHidePanelDelay();
                }
            }

            if (zRotationRightCounter >= requiredIndividualZRotation && zRotationLeftCounter >= requiredIndividualZRotation)
            {
                GameLogger.Log("suceeded Z Rotation tutorial");

                OverrideIsDone = false;
                tutorialPhase = 3;
                TutorialPromptsUI.Instance.ShowPrompt(tutorialPhase, 0);
            }
        }
        else if (tutorialPhase == 3)
        {
            // Touch 
            buttonCollider.enabled = true;
        }
        else if (tutorialPhase == 4)
        {
            // Unfreeze
            ballCollider.enabled = true;
        }
    }

    private void TutorialButtonDetected()
    {
        // dissolve shield a bit
        // puzzle completion vfx etc.. 

        if (tutorialPhase != 3) return; 

        GameLogger.Log("suceeded Button Touch tutorial");
        LevelInfosUI.Instance.ActivatePuzzleUIOnWin(0);

        OverrideIsDone = false;
        tutorialPhase = 4;
        TutorialPromptsUI.Instance.ShowPrompt(tutorialPhase, 0);
    }

    private void TutorialBallDetected()
    {
        // totally dissolve shield
        // deactivate shield collider
        // activate star collider + fx etc..

        if (tutorialPhase != 4) return;

        GameLogger.Log("suceeded Unfreeze tutorial");
        LevelInfosUI.Instance.ActivatePuzzleUIOnWin(1);

        OverrideIsDone = false;
        tutorialPhase = 5;
        TutorialPromptsUI.Instance.ShowPrompt(tutorialPhase, 0);
        LevelManager.LevelIsFinished = true;

        starCollider.enabled = true; 
    }
}