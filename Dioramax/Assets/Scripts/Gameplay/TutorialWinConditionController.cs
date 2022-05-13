using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// because there is too much hardcoded stuff in the LevelManager. This should only be temporary
public class TutorialWinConditionController : MonoBehaviour
{
    [SerializeField] private DioramaInfos dioramaInfos; 
    [SerializeField] private WinConditionHolder[] winConditionHolders = new WinConditionHolder[4]; // peut-être pas besoin
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
    [SerializeField] private Collider ratCollider;

    [Header("Final")]
    [SerializeField] private Collider starCollider;


    private void OnEnable()
    {
        TouchDetection.OnTutorialButtonDetection += TutorialButtonDetected; 
    }

    private void OnDisable()
    {
        TouchDetection.OnTutorialButtonDetection -= TutorialButtonDetected;
    }

    private void Start()
    {
        cameraCraneZRotation.SetActive(false);
        buttonCollider.enabled = false;
        ratCollider.enabled = false;
        starCollider.enabled = false;
    }

    void Update()
    {
        if (LevelManager.GameState != GameState.Playing) return; 

        if (tutorialPhase == 0)
        {
            // XY rotation

            if (DioravityCameraCraneRotation.YXRotation)
            {
                xyRotationCounter += 0.02f;

                if (!OverrideIsDone)
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

            if (CameraZoom.ZoomingOut)
            {
                zoomInCounter += 0.02f;

                if (!OverrideIsDone)
                {
                    OverrideIsDone = true;
                    Debug.Log("zooming out"); 
                    TutorialPromptsUI.Instance.OverrideHidePanelDelay();
                }
            }

            if (CameraZoom.ZoomingIn)
            {
                zoomOutCounter += 0.02f;

                if (!OverrideIsDone)
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

                if (!OverrideIsDone)
                {
                    OverrideIsDone = true;
                    TutorialPromptsUI.Instance.OverrideHidePanelDelay();
                }
            }

            if (ZRotationButton.RightIsSelected)
            {
                zRotationRightCounter += 0.02f;

                if (!OverrideIsDone)
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
            ratCollider.enabled = true;
        }

        // GameLogger.Log("Tutorial is FINISHED"); 
        // LevelManager.LevelIsFinished = true;
    }

    private void TutorialButtonDetected()
    {
        // dissolve shield a bit
        // puzzle completion vfx etc.. 

        GameLogger.Log("suceeded Button Touch tutorial");
        LevelInfosUI.Instance.ActivatePuzzleUIOnWin(1);

        OverrideIsDone = false;
        tutorialPhase = 4;
        TutorialPromptsUI.Instance.ShowPrompt(tutorialPhase, 0);
    }

    private void TutorialRatDetected()
    {
        // totally dissolve shield
        // deactivate shield collider
        // activate star collider + fx etc..

        GameLogger.Log("suceeded Unfreeze tutorial");
        LevelInfosUI.Instance.ActivatePuzzleUIOnWin(2);

        OverrideIsDone = false;
        tutorialPhase = 5;
        TutorialPromptsUI.Instance.ShowPrompt(tutorialPhase, 0);
        LevelManager.LevelIsFinished = true;

        starCollider.enabled = true; 
    }
}

[System.Serializable]
public class WinConditionHolder
{
    public string puzzleName;
    public WinCondition winCondition; 
}
