using UnityEngine;

// Keep controls and gamefeel SEPARATE. 

public enum TouchState { None, Tap, Hold, DoubleTap, Drag, Rotating, Zooming }

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

    private TouchState currentState, previousState; 


    private Vector3 cameraPosition;
    private Vector3 touch0CurrentPosition;
    private Vector3 touch1CurrentPosition;

    private Vector3 touch0Direction;
    private Vector3 touch1Direction;

    private float currentTouchMoveForce;

    private bool touch1HasBeenUnregistered = true; // I couldn't call cameraZoom.SetPinchRegisterValue(false) otherwise.. 
                                                   // but maybe there is a better solution

    private int FrameCount { get; set; }
    public static System.Action OnTouchStarted { get; set; }
    public static System.Action<TouchState> OnTouchEnded { get; set; }

    private Touch[] touchArray;
    private int touchCount;


    private TouchPhase touch0, Touch1; 

    private void Start()
    {
        mainCam = Camera.main;
        CurrentState = TouchState.None; 
    }

    private void Update()
    {
        Debug.Log("touch count is : " + Input.touchCount);
        if (Input.touchCount != 0)
        {
            touch0 = Input.touches[0].phase;

            if (Input.touchCount == 2)
            {
                Touch1 = Input.touches[1].phase;
            }
        }


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
                    OnTouchStarted();
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
                    FrameCount++;

                    if (currentTouchMoveForce >= cameraRotation.RotationSensitivity)
                    {
                        // Debug.Log("not swiping. State is now Rotating");
                        cameraRotation.UpdateXYRotation(touch0Direction.normalized, currentTouchMoveForce);
                        SetTouchState(TouchState.Rotating);
                    }
                    else if (FrameCount >= frameCountBeforeTapToHold)
                    {
                        SetTouchState(TouchState.Hold);
                    }
                }
                else if (Input.touches[0].phase == TouchPhase.Ended)
                {
                    // Debug.Log("finger was removed from screen");
                    FrameCount = 0;
                    Debug.Log("none from mono touch ended"); 
                    SetTouchState(TouchState.None); // ONLY PLACE where state can be set to none
                    OnTouchEnded(PreviousState); // was I zooming or rotating ? 
                }
            }
            // TOO ACCURATE. A single pixel-sized movement is enough -> feels like glitching when you put your fingers on the screen
            else if (Input.touchCount == 2)
            {
                if (currentTouch1.phase == TouchPhase.Ended)
                {
                    // Debug.Log("finger was removed from screen");
                    Debug.Log("REMOVING SECOND FINGER");
                    FrameCount = 0;
                }

                // ZOOM IN/OUT 
                if (Mathf.Abs(currentTouch1.deltaPosition.x) > Mathf.Abs(currentTouch1.deltaPosition.y))
                {
                    Debug.Log("Z rotation");
                    cameraRotation.UpdateZRotation(currentTouch0, currentTouch1, currentTouchMoveForce);
                    SetTouchState(TouchState.Rotating);
                }
                // Z ROTATION
                else if (currentTouch1.phase != TouchPhase.Ended)
                {
                    Debug.Log("zooming");
                    SetPinch(false, true);
                    cameraZoom.UpdatePinch(currentTouch0, currentTouch1);
                    SetTouchState(TouchState.Zooming);
                }
            }
        }
    }

    private void SetTouchState(TouchState newState)
    {
        PreviousState = CurrentState; 
        CurrentState = newState;

        previousState = PreviousState;
        currentState = CurrentState; 
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

        currentTouchMoveForce = touchCount == 1 ? touch0Direction.magnitude : Mathf.Max(touch0Direction.magnitude, touch1Direction.magnitude); 
    }

    private void SetPinch(bool _touch1HasBeenUnregistered, bool _cameraPinchRegisterValueTo)
    {
        touch1HasBeenUnregistered = _touch1HasBeenUnregistered;
        cameraZoom.SetPinchRegisterValue(_cameraPinchRegisterValueTo);

        if (_cameraPinchRegisterValueTo)
        {
            SetTouchState(TouchState.Zooming);
        }
    }
}
