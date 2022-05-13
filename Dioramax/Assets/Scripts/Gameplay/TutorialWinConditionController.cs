using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// because there is too much hardcoded stuff in the LevelManager. This should only be temporary
public class TutorialWinConditionController : MonoBehaviour
{
    [SerializeField] private DioramaInfos dioramaInfos; 
    [SerializeField] private WinConditionHolder[] winConditionHolders = new WinConditionHolder[4];
    private int tutorialPhase;

    // XY rotation
    [Header("XY Rotation Tutorial")]
    [SerializeField, Range(0.5f, 5f)] private float requiredRotationDuration = 1f;
    private float xyRotationCounter;

    // Zoom
    [Header("Zoom Tutorial")]
    [SerializeField, Range(0.5f, 5f)] private float requiredIndividualZoomDuration = 1f;
    private float zoomOutCounter, zoomInCounter; 

    // Z Rotation


    // Touch & Unfreeze

    void Update()
    {
        if (LevelManager.GameState != GameState.Playing) return; 

        if (tutorialPhase == 0)
        {
            if (DioravityCameraCraneRotation.YXRotation)
            {
                xyRotationCounter += 0.02f;

                if (xyRotationCounter >= requiredRotationDuration)
                {
                    // winConditionHolders[tutorialPhase].winCondition.SetWinCondition(true);
                    GameLogger.Log("suceeded XY rotation tutorial");
                    LevelInfosUI.Instance.ActivatePuzzleUIOnWin(tutorialPhase);

                    tutorialPhase = 1;
                }
            }
        }
        else if (tutorialPhase == 1)
        {
            if (CameraZoom.ZoomingOut)
            {
                zoomInCounter += 0.02f; 
            }

            if (CameraZoom.ZoomingIn)
            {
                zoomOutCounter += 0.02f; 
            }

            if (zoomInCounter >= requiredIndividualZoomDuration && zoomOutCounter >= requiredIndividualZoomDuration)
            {
                GameLogger.Log("suceeded Zoom tutorial");
                LevelInfosUI.Instance.ActivatePuzzleUIOnWin(tutorialPhase);

                tutorialPhase = 2;
            }
        }
        else if (tutorialPhase == 2)
        {

        }
        else if (tutorialPhase == 3)
        {
            /* if ()
            {
                // GameLogger.Log("Tutorial is FINISHED"); 
                LevelManager.LevelIsFinished = true;
            } */
        }
    }
}

[System.Serializable]
public class WinConditionHolder
{
    public string puzzleName;
    public WinCondition winCondition; 
}
