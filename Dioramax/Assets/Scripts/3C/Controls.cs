using UnityEngine;

// Keep controls and gamefeel SEPARATE. 

public enum TouchState { None, Tap, Hold, DoubleTap, Drag, Rotating, Zooming, UNDEFINED }

public class Controls : MonoBehaviour
{
    [SerializeField] private CameraRotation cameraRotation;
    [SerializeField] private CameraZoom cameraZoom;
    [SerializeField] private TouchDetection touchDetection;
    [SerializeField, 
        Tooltip("How much before a tap is considered a hold"), Range(0, 20)] 
    private int frameCountBeforeTapToHold = 10; // no gameplay usage for now, just transition logic

    private Camera mainCam;

    private Touch currentTouch0, currentTouch1; 
    public static TouchState CurrentState { get; private set; }
    public static TouchState PreviousState { get; private set; }


    private Vector3 cameraPosition;
    private Vector3 touch0CurrentPosition;
    private Vector3 touch1CurrentPosition;

    private Vector3 touch0Direction;
    private Vector3 touch1Direction;

    private float maxTouchForce;

    private bool touch1HasBeenUnregistered = true; // I couldn't call cameraZoom.SetPinchRegisterValue(false) otherwise.. 
                                                   // but maybe there is a better solution

    private float topPosition;

    private int FrameCount { get; set; } 

    private void Start()
    {
        mainCam = Camera.main;
        CurrentState = TouchState.None; 
    }

    private void Update()
    {
        // MAJ 14.02.2022 -> untested
        if (Input.touchCount < 2 && !touch1HasBeenUnregistered)
        {
            // Debug.Log("unregistering touch 1"); 
            SetPinch(true, false);
        }

        // ROTATION
        if (Input.touchCount != 0)
        {
            UpdateTouch(Input.touchCount);

            if (Input.touchCount == 1)
            {
                if (currentTouch0.phase == TouchPhase.Began)
                {
                    SetTouchState(TouchState.Tap);
                    cameraPosition = mainCam.transform.position;

                    // if something detected, enter swipe and NOT rotating state
                    if (touchDetection.TryCastToTarget(cameraPosition, touch0CurrentPosition))
                    {
                        // Debug.Log("detected an interactable object");
                        SetTouchState(TouchState.Drag);
                    }

                    // Debug.Log("touch state is " + touchState);
                }
                else if (currentTouch0.phase == TouchPhase.Stationary)
                {
                    FrameCount++;

                    if (FrameCount >= frameCountBeforeTapToHold)
                    {
                        // Debug.Log("transition to Hold state");
                        SetTouchState(TouchState.Hold);
                    }
                }
                else if (currentTouch0.phase == TouchPhase.Moved)
                {
                    SetTouchState(TouchState.UNDEFINED); // one way to make up for the super high sensitivity of Stationary/Moved state (no dead zone)
                    if (maxTouchForce > 3f) // BAD HARDCODED. But this script shouldn't know about cameraRotation.rotationSensitivity
                    {
                        // Debug.Log("not swiping. State is now Rotating");
                        cameraRotation.UpdateXYRotation(touch0Direction.normalized, maxTouchForce); 
                        SetTouchState(TouchState.Rotating);
                    } 
                }
                else if (currentTouch0.phase == TouchPhase.Ended)
                {
                    // Debug.Log("finger was removed from screen");
                    FrameCount = 0;
                    SetTouchState(TouchState.None);
                }
            }
            // TOO ACCURATE. A single pixel-sized movement is enough -> feels like glitching when you put your fingers on the screen
            else if (Input.touchCount == 2)
            {
                if (currentTouch1.phase == TouchPhase.Ended)
                {
                    // Debug.Log("finger was removed from screen");
                    FrameCount = 0;
                    SetTouchState(TouchState.None);
                }

                // ZOOM IN/OUT 
                if (Mathf.Abs(currentTouch1.deltaPosition.x) > Mathf.Abs(currentTouch1.deltaPosition.y))
                {
                    Debug.Log("Z rotation");
                    cameraRotation.UpdateZRotation(currentTouch0, currentTouch1, out topPosition, maxTouchForce);
                }
                // Z ROTATION
                else
                {
                    Debug.Log("zooming");
                    SetPinch(false, true);
                    cameraZoom.UpdatePinch(currentTouch0, currentTouch1, out topPosition);
                }
            }
        }
    }

    private void SetTouchState(TouchState newState)
    {
        PreviousState = CurrentState; 
        CurrentState = newState;
    }

    // use input.touches:Touch[] instead ? But means I have to recreate an array every time touchCount changes.. 
    private void UpdateTouch(int touchCount)
    {
        currentTouch0 = Input.GetTouch(0);
        touch0CurrentPosition = mainCam.ScreenToWorldPoint(new Vector3(
            currentTouch0.position.x,
            currentTouch0.position.y,
            mainCam.nearClipPlane));

        touch0Direction = currentTouch0.deltaPosition;

        if (touchCount == 2)
        {
            // Debug.Log("updating double touch");

            currentTouch1 = Input.GetTouch(1);
            touch1CurrentPosition = mainCam.ScreenToWorldPoint(new Vector3(
                currentTouch1.position.x,
                currentTouch1.position.y,
                mainCam.nearClipPlane));

            touch1Direction = currentTouch1.deltaPosition;
        }
        // DEBUG
        else if (touchCount == 1)
        {
            // Debug.Log("updating mono touch");
        }

        maxTouchForce = touchCount == 1 ? touch0Direction.magnitude : Mathf.Max(touch0Direction.magnitude, touch1Direction.magnitude); 
    }

    private void SetPinch(bool _touch1HasBeenUnregistered, bool _cameraPinchRegisterValueTo)
    {
        touch1HasBeenUnregistered = _touch1HasBeenUnregistered;
        cameraZoom.SetPinchRegisterValue(_cameraPinchRegisterValueTo);
        SetTouchState(_cameraPinchRegisterValueTo == false ? TouchState.None : TouchState.Zooming); 
    }
}
