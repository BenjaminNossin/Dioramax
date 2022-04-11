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

    private bool doubleTap;

    private int FrameCount { get; set; }

    // REFACTORING : uncouple this system (gamefeel) from 3C
    #region Gamefeel 
    public static System.Action OnTouchStarted { get; set; }
    public static System.Action<TouchState> OnTouchEnded { get; set; }
    #endregion

    // custom logic for Input.Touch[1].phase == TouchPhase.Ended because unity's does not work all the time
    private const byte FRAMES_DELAY_TO_HOLD = 15;

    #region DOUBLE TOUCH
    private bool touch1HasBeenUnregistered = true; // I couldn't call cameraZoom.SetPinchRegisterValue(false) otherwise.. 
                                                   // but maybe there is a better solution
    private bool transitionningOutOfDoubleTouch;
    private int outOfDoubleTouchFrames;
    private float currentAngle, previousAngle; 
    private Vector2 middlePoint;
    private bool middlePointIsSet;
    public static Vector2 InitialTouch0Direction;
    public static Vector2 Touch0DirectionOnZoomStart; 
    private int doubleTouchFrameCount;
    private bool canDoZRotation;
    private int zoomAngleFrameCount;
    private bool directionOnUpdatedZoom;
    private float angleDifference;
    private const float CAMERA_SENSIBILITY = 5f;
    private const byte DOUBLETOUCH_FRAME_DELAY = 5;
    private const float ZOOM_TO_ROTATION_THRESHOLD = 15f;
    private const float UPDATED_ZOOM_THRESHOLD = 1f;
    private const byte OUT_OF_DOUBLETOUCH_FRAME_DELAY = 10; 
    #endregion

    private void Awake()
    {
        CurrentState = TouchState.None;
    }

    private void LateUpdate()
    {
        // Do Once
        if (Input.touchCount < 2 && !touch1HasBeenUnregistered)
        {
            if (currentState == TouchState.Zooming || currentState == TouchState.ZRotating)
            {
                Debug.Log("calling out of double touch frames");

                transitionningOutOfDoubleTouch = true;

                ResetDoubleTouchValues();

                SetPinchValue(true, false);
            }
        }

        if (transitionningOutOfDoubleTouch)
        {
            outOfDoubleTouchFrames++; 

            if (outOfDoubleTouchFrames >= OUT_OF_DOUBLETOUCH_FRAME_DELAY)
            {
                transitionningOutOfDoubleTouch = false;

                outOfDoubleTouchFrames = 0;
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

                if (transitionningOutOfDoubleTouch) return;

                if (currentTouch0.phase == TouchPhase.Began)
                {
                    StopCoroutine(StopWaitingForDoubleTap());
                    OnTouchStarted();
                    cameraPosition = mainCam.transform.position;
                    SetTouchState(doubleTap ? TouchState.DoubleTap : TouchState.Tap); // technically double tap should only work when hitting specific objects

                    // if something detected, enter swipe and NOT rotating state
                    if (touchDetection.TryCastToTarget(cameraPosition, touch0CurrentPosition, doubleTap))
                    {
                        SetTouchState(doubleTap ? TouchState.DoubleTap : TouchState.Drag);
                    }

                    doubleTap = true;
                    // Debug.Log("touch state is " + touchState);
                }
                else if (currentTouch0.phase == TouchPhase.Stationary)
                {
                    if (FrameCount >= FRAMES_DELAY_TO_HOLD)
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
                    else if (FrameCount >= FRAMES_DELAY_TO_HOLD)
                    {
                        doubleTap = false;

                        SetTouchState(TouchState.Hold);
                    }
                }
                else if (Input.touches[0].phase == TouchPhase.Ended) 
                {
                    Debug.Log("mono touch ended");
                    ResetDoubleTouchValues();

                    transitionningOutOfDoubleTouch = false;
                    FrameCount = 0;
                    StartCoroutine(StopWaitingForDoubleTap());
                    SetTouchState(TouchState.None); // ONLY PLACE where state can be set to none
                    OnTouchEnded(PreviousState);  
                }
            }
            else if (Input.touchCount == 2)
            {
                FrameCount = 0; // can't do it from .Ended because of API sometimes sending weird data from Input.Touches[1].Phase
                doubleTap = false;

                if (currentTouchMoveForce >= CAMERA_SENSIBILITY)
                {
                    doubleTouchFrameCount++;
                    if (!middlePointIsSet)
                    {
                        middlePointIsSet = true;
                        middlePoint = cameraZoom.GetMiddlePoint(currentTouch0, currentTouch1);
                        InitialTouch0Direction = (currentTouch0.position - middlePoint).normalized;
                        Touch0DirectionOnZoomStart = InitialTouch0Direction;
                    }

                    currentAngle = Vector2.Angle(Touch0DirectionOnZoomStart, (currentTouch0.position - middlePoint).normalized);
                    angleDifference = Mathf.Abs(currentAngle - previousAngle);

                    // Debug.Log($"current angle : " + currentAngle);

                    if (doubleTouchFrameCount < DOUBLETOUCH_FRAME_DELAY) return;
                    // Debug.Log("angle difference : " + angleDifference);

                    if (currentAngle > ZOOM_TO_ROTATION_THRESHOLD)
                    {
                        zoomAngleFrameCount = 0; 
                    }

                    if (currentAngle <= ZOOM_TO_ROTATION_THRESHOLD || angleDifference <= UPDATED_ZOOM_THRESHOLD)
                    {
                        zoomAngleFrameCount++;

                        if (!directionOnUpdatedZoom)
                        {
                            directionOnUpdatedZoom = true;
                            Touch0DirectionOnZoomStart = (currentTouch0.position - middlePoint).normalized;
                            zoomAngleFrameCount = 0; 
                        }
                        // Debug.Log("zooming");

                        if (zoomAngleFrameCount < DOUBLETOUCH_FRAME_DELAY) return;

                        SetTouchState(TouchState.Zooming);
                        cameraZoom.UpdatePinch(currentTouch0, currentTouch1);
                        SetPinchValue(false, true);
                        // Debug.Break();
                    }
                    else
                    {
                        canDoZRotation = true;
                        zoomAngleFrameCount = 0;
                        directionOnUpdatedZoom = false;

                        if (canDoZRotation)
                        {
                            // Debug.Log("Z rotation");

                            SetTouchState(TouchState.ZRotating);
                            cameraRotation.UpdateZRotation(currentTouch0, currentTouch1, currentTouchMoveForce);
                            SetPinchValue(false, false);
                        }
                    }

                    previousAngle = currentAngle;
                }
            }
        }
    }

    private void ResetDoubleTouchValues()
    {
        middlePointIsSet = false;
        canDoZRotation = false;
        doubleTouchFrameCount = 0;
        zoomAngleFrameCount = 0;
        directionOnUpdatedZoom = false;

        currentAngle = previousAngle = angleDifference = 0f;
        middlePoint = Vector2.zero;

        currentTouchMoveForce = 0f;
        touch0CurrentPosition = 
            touch1CurrentPosition = 
            touch0Direction = 
            touch1Direction= Vector3.zero; 
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
