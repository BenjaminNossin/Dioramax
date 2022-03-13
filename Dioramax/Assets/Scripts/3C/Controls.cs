using UnityEngine;

// Keep controls and gamefeel SEPARATE. 

public enum TouchState { None, Tap, Hold, DoubleTap, Drag, Rotating, Zooming }

public class Controls : MonoBehaviour
{
    [SerializeField] private CameraRotation cameraRotation;
    [SerializeField] private CameraZoom cameraZoom;
    [SerializeField] private TouchDetection touchDetection;
    [SerializeField, 
        Tooltip("How much frames before a finger on screen is considered a hold"), Range(0, 20)] 
    private int frameCountBeforeChangeState = 15; // no gameplay usage for now, just transition logic
    [SerializeField] private Camera mainCam;

    private Touch currentTouch0, currentTouch1; 
    public static TouchState CurrentState { get; private set; }
    public static TouchState PreviousState { get; private set; }

    private TouchState currentState, previousState; // DEBUG


    private Vector3 cameraPosition;
    private Vector3 touch0CurrentPosition;
    private Vector3 touch1CurrentPosition;

    private Vector3 touch0Direction;
    private Vector3 touch1Direction;

    private float currentTouchMoveForce;

    private bool touch1HasBeenUnregistered = true; // I couldn't call cameraZoom.SetPinchRegisterValue(false) otherwise.. 
                                                   // but maybe there is a better solution

    // REFACTORING : uncouple this system (gamefeel) from 3C
    private int FrameCount { get; set; }
    public static System.Action OnTouchStarted { get; set; }
    public static System.Action<TouchState> OnTouchEnded { get; set; }

    // custom logic for Input.Touch[1].phase == TouchPhase.Ended because unity's does not work all the time
    private bool transitionningOutOfDoubleTouch;
    private int outOfDoubleTouchFrames; 

    private void Awake()
    {
        CurrentState = TouchState.None; 
    }

    private void Update()
    {
        if (Input.touchCount < 2 && !touch1HasBeenUnregistered)
        {
            if (currentState == TouchState.Zooming)
            {
                Debug.Log("unregistering touch 1");
                transitionningOutOfDoubleTouch = true;
                SetPinchValue(true, false);
            }
        }

        if (transitionningOutOfDoubleTouch)
        {
            outOfDoubleTouchFrames++; 

            if (outOfDoubleTouchFrames >= 30)
            {
                transitionningOutOfDoubleTouch = false; 
            }
        }

        if (Input.touchCount != 0)
        {
            UpdateTouch(Input.touchCount);

            if (Input.touchCount == 1)
            {
                FrameCount++;

                if (!transitionningOutOfDoubleTouch)
                {
                    if (currentTouch0.phase == TouchPhase.Began)
                    {
                        OnTouchStarted();
                        cameraPosition = mainCam.transform.position;
                        SetTouchState(TouchState.Tap);

                        // if something detected, enter swipe and NOT rotating state
                        if (touchDetection.TryCastToTarget(cameraPosition, touch0CurrentPosition))
                        {
                            SetTouchState(TouchState.Drag);
                        }

                        // Debug.Log("touch state is " + touchState);
                    }
                    else if (currentTouch0.phase == TouchPhase.Stationary)
                    {
                        if (FrameCount >= frameCountBeforeChangeState)
                        {
                            // Debug.Log("transition to Hold state");
                            SetTouchState(TouchState.Hold);
                        }
                    }
                    else if (currentTouch0.phase == TouchPhase.Moved)
                    {
                        // all of this because Input.touches[1] sends wrong data from time to time..
                        if (currentTouchMoveForce >= cameraRotation.RotationSensitivity)
                        {
                            // Debug.Log("not swiping. State is now Rotating");
                            cameraRotation.UpdateXYRotation(touch0Direction.normalized, currentTouchMoveForce);
                            SetTouchState(TouchState.Rotating);
                        }
                        else if (FrameCount >= frameCountBeforeChangeState)
                        {
                            SetTouchState(TouchState.Hold);
                        }
                    }
                }                               

                if (Input.touches[0].phase == TouchPhase.Ended)
                {
                    Debug.Log("none from mono touch ended");
                    transitionningOutOfDoubleTouch = false; 
                    FrameCount = 0;
                    SetTouchState(TouchState.None); // ONLY PLACE where state can be set to none
                    OnTouchEnded(PreviousState); // was I zooming or rotating ? 
                }
            }
            // TOO ACCURATE. A single pixel-sized movement is enough -> feels like glitching when you put your fingers on the screen
            else if (Input.touchCount == 2)
            {
                // Z ROTATION
                FrameCount = 0; // can't do it from .Ended because of API sometimes not sending the right data on Input.Touches[1].Phase
                if (Mathf.Abs(currentTouch1.deltaPosition.x) >= Mathf.Abs(currentTouch1.deltaPosition.y)) // logically WRONG. What if I rotate with index and middle ?
                {
                    // Debug.Log("Z rotation");
                    if (currentTouchMoveForce >= 3f)
                    {
                        cameraRotation.UpdateZRotation(currentTouch0, currentTouch1, currentTouchMoveForce);
                        SetPinchValue(false, false);
                        SetTouchState(TouchState.Rotating);
                    }
                }
                // ZOOM IN/OUT 
                else if (currentTouchMoveForce >= 3f) // currentTouch1.phase != TouchPhase.Ended 
                {
                    // Debug.Log("zooming");
                    cameraZoom.UpdatePinch(currentTouch0, currentTouch1);
                    SetPinchValue(false, true);
                    SetTouchState(TouchState.Zooming);
                }
                else
                {
                    SetTouchState(TouchState.Hold);
                }
            }
        }
        else if (transitionningOutOfDoubleTouch)
        {
            Debug.Log("none from out of double touch");
            transitionningOutOfDoubleTouch = false;
            FrameCount = 0;
            SetTouchState(TouchState.None);
        }
    }

    private void SetTouchState(TouchState newState)
    {
        PreviousState = CurrentState; 
        CurrentState = newState;

        previousState = PreviousState;
        currentState = CurrentState;
    }


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

        currentTouchMoveForce = touchCount == 1 ? 
            touch0Direction.magnitude : 
            Mathf.Max(touch0Direction.magnitude, touch1Direction.magnitude); 
    }

    private void SetPinchValue(bool _touch1HasBeenUnregistered, bool _cameraPinchRegisterValueTo)
    {
        touch1HasBeenUnregistered = _touch1HasBeenUnregistered;
        cameraZoom.SetPinchRegisterValue(_cameraPinchRegisterValueTo);
    }
}
