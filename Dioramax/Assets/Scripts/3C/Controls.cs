using UnityEngine;
using System.Collections; 

// Keep controls and gamefeel SEPARATE. 
public enum TouchState { None, Tap, Hold, DoubleTap, Drag, XYRotating, Zooming, ZRotating }
[System.Flags] public enum SwipDirection { NONE, Linear, NonLinear } 

public class Controls : MonoBehaviour
{
    [SerializeField] private DioravityCameraCraneRotation cameraRotation;
    [SerializeField] private CameraZoom cameraZoom;
    [SerializeField] private TouchDetection touchDetection;
    [SerializeField, 
        Tooltip("How much frames before a finger on screen is considered a hold"), Range(0, 20)] 
    private int frameCountBeforeChangeState = 15; // no gameplay usage for now, just transition logic
    [SerializeField] private Camera mainCam;
    [SerializeField, Range(0.15f, 0.75f)] private float doubleTapWaitDelay = 0.35f;
    private SwipDirection swipeDirection; 

    private Touch currentTouch0, currentTouch1; 
    public static TouchState CurrentState { get; private set; }
    public static TouchState PreviousState { get; private set; }

    private TouchState currentState, previousState; // DEBUG


    private Vector3 cameraPosition;
    private Vector3 touch0CurrentPosition;
    private Vector3 touch1CurrentPosition;

    private Vector3 touch0Direction;
    private Vector3 touch1Direction;
    private Vector2 previousTouch0Direction; 

    private float currentTouchMoveForce;

    private bool touch1HasBeenUnregistered = true; // I couldn't call cameraZoom.SetPinchRegisterValue(false) otherwise.. 
                                                   // but maybe there is a better solution

    private bool doubleTap;

    // REFACTORING : uncouple this system (gamefeel) from 3C
    private int FrameCount { get; set; }
    public static System.Action OnTouchStarted { get; set; }
    public static System.Action<TouchState> OnTouchEnded { get; set; }

    // custom logic for Input.Touch[1].phase == TouchPhase.Ended because unity's does not work all the time
    private bool transitionningOutOfDoubleTouch;
    private int outOfDoubleTouchFrames;

    // DEBUG
    private float angle;
    private Vector2 middlePoint;
    private bool middlePointIsSet;
    public static Vector2 InitialTouch0Direction;
    private int doubleTouchFrameCount;
    private bool canDoZRotation;
    private int zoomAngleFrameCount; 

    private void Awake()
    {
        CurrentState = TouchState.None;
    }

    private void LateUpdate()
    {
        if (Input.touchCount < 2 && !touch1HasBeenUnregistered)
        {
            if (currentState == TouchState.Zooming || currentState == TouchState.ZRotating)
            {
                middlePointIsSet = false;
                canDoZRotation = false; 
                doubleTouchFrameCount = 0;
                zoomAngleFrameCount = 0; 
                transitionningOutOfDoubleTouch = true;
                SetPinchValue(true, false);
            }
        }

        if (transitionningOutOfDoubleTouch)
        {
            // Debug.Log("out of double touch frames"); 
            outOfDoubleTouchFrames++; 

            if (outOfDoubleTouchFrames >= 10)
            {
                outOfDoubleTouchFrames = 0;
                transitionningOutOfDoubleTouch = false;
                FrameCount = 0;
                SetTouchState(TouchState.None);
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
                        StopCoroutine(StopWaitingForDoubleTap());
                        OnTouchStarted();
                        cameraPosition = mainCam.transform.position;
                        SetTouchState(TouchState.Tap);

                        // if something detected, enter swipe and NOT rotating state
                        if (touchDetection.TryCastToTarget(cameraPosition, touch0CurrentPosition, doubleTap))
                        {
                            SetTouchState(TouchState.Drag);
                        }

                        doubleTap = true;
                        // Debug.Log("touch state is " + touchState);
                    }
                    else if (currentTouch0.phase == TouchPhase.Stationary)
                    {
                        if (FrameCount >= frameCountBeforeChangeState)
                        {
                            // Debug.Log("transition to Hold state");
                            doubleTap = false;
                            SetTouchState(TouchState.Hold);
                        }
                    }
                    else if (currentTouch0.phase == TouchPhase.Moved)
                    {
                        // all of this because Input.touches[1] sends wrong data from time to time..
                        if (currentTouchMoveForce >= cameraRotation.RotationSensitivity)
                        {
                            // Debug.Log("not swiping. State is now Rotating");
                            doubleTap = false;

                            cameraRotation.UpdateXYRotation(touch0Direction.normalized, currentTouchMoveForce);
                            SetTouchState(TouchState.XYRotating);
                        }
                        else if (FrameCount >= frameCountBeforeChangeState)
                        {
                            doubleTap = false;

                            SetTouchState(TouchState.Hold);
                        }
                    }
                    else if (Input.touches[0].phase == TouchPhase.Ended)
                    {
                        // Debug.Log("mono touch ended");
                        transitionningOutOfDoubleTouch = false;
                        FrameCount = 0;
                        StartCoroutine(StopWaitingForDoubleTap());
                        SetTouchState(TouchState.None); // ONLY PLACE where state can be set to none
                        OnTouchEnded(PreviousState); // was I zooming or rotating ? 
                    }
                }
            }
            // TOO ACCURATE. A single pixel-sized movement is enough -> feels like glitching when you put your fingers on the screen
            else if (Input.touchCount == 2)
            {
                FrameCount = 0; // can't do it from .Ended because of API sometimes sending weird data from Input.Touches[1].Phase
                doubleTap = false;

                if (currentTouchMoveForce >= 3f) // BAD : hardcoded -> const sensibility
                {
                    doubleTouchFrameCount++; 
                    if (!middlePointIsSet)
                    {
                        middlePointIsSet = true;
                        middlePoint = cameraZoom.GetMiddlePoint(currentTouch0, currentTouch1);
                        InitialTouch0Direction = (currentTouch0.position - middlePoint).normalized;
                    }

                    angle = Vector2.Angle(InitialTouch0Direction, (currentTouch0.position - middlePoint).normalized);
                    // Debug.Log($"angle : " + angle);

                    if (doubleTouchFrameCount < 5) return; 

                    if (angle <= 15f) // BAD : hardcoded -> const zoom to ZRotation threshold
                    {
                        // Debug.Log("zooming");
                        zoomAngleFrameCount++;

                        if (zoomAngleFrameCount < 5) return; 

                        SetTouchState(TouchState.Zooming);
                        cameraZoom.UpdatePinch(currentTouch0, currentTouch1);
                        SetPinchValue(false, true);
                    }
                    else
                    {
                        canDoZRotation = true;
                        zoomAngleFrameCount = 0; 

                        if (canDoZRotation)
                        {
                            Debug.Log("Z rotation");

                            SetTouchState(TouchState.ZRotating);
                            cameraRotation.UpdateZRotation(currentTouch0, currentTouch1, currentTouchMoveForce);
                            SetPinchValue(false, false);
                        }
                    } 
                }
            }
        }
    }

    private IEnumerator StopWaitingForDoubleTap()
    {
        yield return new WaitForSeconds(doubleTapWaitDelay);
        doubleTap = false;
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
