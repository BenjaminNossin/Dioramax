using UnityEngine;
using System.Collections; 

// Keep controls and gamefeel SEPARATE. 
public enum TouchState { None, Tap, Hold, DoubleTap, Drag, XYRotating, Zooming }

public class Controls : MonoBehaviour
{
    [SerializeField] private DioravityCameraCraneRotation cameraRotation;
    [SerializeField] private CameraZoom cameraZoom;
    [SerializeField] private TouchDetection touchDetection;

    [SerializeField] private Camera mainCam;
    [SerializeField, Range(0.15f, 0.75f)] private float doubleTapWaitDelay = 0.35f;

    private Touch currentTouch0, currentTouch1; 
    public static TouchState CurrentState { get; private set; }
    public static TouchState PreviousState { get; private set; }

    public static Vector2 InitialTouch0Direction { get; set; }
    public static Vector2 Touch0DirectionOnZoomStart { get; set; }


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

    #region DEBUG
    private float updateSeconds;
    private float fixedUpdateSeconds;

    private byte mobileUpdateFPSCounter;
    private byte mobileFixedUpdateFPSCounter;
    #endregion

    #region DOUBLE TOUCH
    private bool touch1HasBeenUnregistered = true; // I couldn't call cameraZoom.SetPinchRegisterValue(false) otherwise.. 
                                                   // but maybe there is a better solution
    private Vector2 middlePoint;
    private bool middlePointIsSet; 
    private bool transitionningOutOfDoubleTouch;
    private int outOfDoubleTouchFrames;
    private const float CAMERA_SENSIBILITY = 5f;
    private const byte DOUBLETOUCH_FRAME_DELAY = 5;
    private const byte OUT_OF_DOUBLETOUCH_FRAME_DELAY = 10; 
    #endregion

    private void Awake()
    {
        CurrentState = TouchState.None;
    }

    // You should better differentiate what is in Update, FixedUpdate and LateUpdate.. 
  
    private void Update()
    {
        /* if (fixedUpdateSeconds >= 1f)
        {
            GameLogger.Log("mobile fixedUpdate fps : " + mobileFixedUpdateFPSCounter);
            fixedUpdateSeconds = 0f;
            mobileFixedUpdateFPSCounter = 0; 
        }

        mobileFixedUpdateFPSCounter++;
        fixedUpdateSeconds += Time.fixedDeltaTime; */

        if (LevelManager.GameState != GameState.Playing) return; 

        // Do Once
        if (Input.touchCount < 2 && !touch1HasBeenUnregistered)
        {
            if (CurrentState == TouchState.Zooming)
            {
                transitionningOutOfDoubleTouch = true;

                OnTouchEnded(CurrentState);

                ResetDoubleTouchValues();

                SetPinchValue(true, false);
            }
        }

        // delay to still allow double tap
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

                    // SHOULD BE DONE IN FIXED UPDATE
                    touchDetection.TryCastToTarget(cameraPosition, touch0CurrentPosition);

                    doubleTap = true; // for the next entering of Began if done within short delay
                    // GameLogger.Log("touch state is " + touchState);
                }
                else if (currentTouch0.phase == TouchPhase.Stationary)
                {
                    if (FrameCount >= FRAMES_DELAY_TO_HOLD)
                    {
                        // GameLogger.Log("transition to Hold state");
                        doubleTap = false;
                        SetTouchState(TouchState.Hold);
                    }
                }
                else if (currentTouch0.phase == TouchPhase.Moved)
                {
                    // all of this because Input.touches[1] sends wrong data from time to time..
                    if (currentTouchMoveForce >= cameraRotation.RotationSensitivity)
                    {
                        // GameLogger.Log("not swiping. State is now Rotating");
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
                    // GameLogger.Log("mono touch ended");
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
                    // GameLogger.Log("zooming");

                    if (!middlePointIsSet)
                    {
                        OnTouchStarted();

                        middlePointIsSet = true;
                        middlePoint = cameraZoom.GetMiddlePoint(currentTouch0, currentTouch1);
                        InitialTouch0Direction = (currentTouch0.position - middlePoint).normalized;
                        Touch0DirectionOnZoomStart = InitialTouch0Direction;
                    } 

                    SetTouchState(TouchState.Zooming);
                    cameraZoom.ZoomInOrOut(currentTouch0, currentTouch1);
                    SetPinchValue(false, true);
                }
            }
        }
    }

    private void ResetDoubleTouchValues()
    {
        middlePointIsSet = false;
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
            // GameLogger.Log("updating double touch");

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

    // NOT USED, ONLY SETTING SOME VALUES
    private void SetPinchValue(bool _touch1HasBeenUnregistered, bool _cameraPinchRegisterValueTo)
    {
        touch1HasBeenUnregistered = _touch1HasBeenUnregistered;
        cameraZoom.SetPinchRegisterValue(_cameraPinchRegisterValueTo);
    }
}
