using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This script allows a camera to zoom in and out with a dynamic direction. 
/// It uses translation instead of fov modification.
/// Put it on the object with camera componenet, NOT the crane
/// </summary>
public class CameraZoom : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField] private Transform dioramaTransf;
    [SerializeField] private Transform transfToMove; 
    [SerializeField, Range(0.1f, 50)] private float maxZoomIn = 45;
    [SerializeField, Range (70, 150)] private float maxZoomOut = 70;
    [SerializeField, Range(10f, 60f)] private float zoomSpeed = 40f;
    private float curvedSpeed;
    private Vector3 dioramaPosition;
    private float lerpedDistance0;
    private float lerpedDistance;
    private float currentDistance; 

    private Touch touchTop;
    private Touch touchBottom; 

    private bool zoomStartIsRegistered;

    public static bool ZoomingOut { get; private set; }
    public static bool ZoomingIn { get; private set; } // for tutorial. !ZoomingOut != ZoomingIn

    private bool zoomingIn, zoomingOut; // DEBUG

    private bool canZoomIn;
    private bool canZoomOut;

    private Vector3 zoomPointStart;
    private Vector3 zoomPointEnd;

    private float zoomValue;

    [Header("Gamefeel")]
    [SerializeField] CurveEvaluator gamefeelCurve;
    private bool updateGamefeelCurve;

    UnityAction OnEvaluationEndedCallback;

    private void OnEnable()
    {
        Controls.OnTouchStarted += InterruptPreviousCurveOnNewTouch;
        Controls.OnTouchEnded += TriggerGamefeelCurveOnInputStateChange;


        OnEvaluationEndedCallback += ResetBoolValues;
    }

    private void OnDisable()
    {
        Controls.OnTouchStarted -= InterruptPreviousCurveOnNewTouch;
        Controls.OnTouchEnded -= TriggerGamefeelCurveOnInputStateChange;


        OnEvaluationEndedCallback -= ResetBoolValues;
    } 

    private void Start()
    {
        mainCam = Camera.main;
        Input.multiTouchEnabled = true;
        zoomValue = mainCam.fieldOfView;
        canZoomIn = canZoomOut = true;

        dioramaPosition = dioramaTransf.position;
    }

    private void Update()
    {
        zoomingIn = ZoomingIn;
        zoomingOut = ZoomingOut;

        if (updateGamefeelCurve)
        {
            curvedSpeed = zoomSpeed * gamefeelCurve.DoZoomCurve(OnEvaluationEndedCallback);
            ZoomInOrOut(touchTop, touchBottom);
        }
    }

    // NEVER USED
    public void SetPinchRegisterValue(bool value)
    {
        zoomStartIsRegistered = value;
    }

    private float topPosition;
    // private Vector2 previousDelta, currentDelta;
    //private float xDelta, yDelta;
    private Vector2 middlePoint; 

    public Vector2 GetMiddlePoint(Touch _touch0, Touch _touch1)
    {
        topPosition = Mathf.Max(_touch0.position.y, _touch1.position.y);

        // stupid to do this every frame. But how to keep reference to the arguments each frame ?
        if (topPosition == _touch0.position.y)
        {
            touchTop = _touch0;
            touchBottom = _touch1;
        }
        else if (topPosition == _touch1.position.y)
        {
            touchTop = _touch1;
            touchBottom = _touch0;
        }

        middlePoint = Vector3.Lerp(touchTop.position, touchBottom.position, 0.5f);
        return middlePoint;
    }

    private Vector2 currentTouch0Delta;
    private float dotProduct; 
    public void ZoomInOrOut(Touch _touch0, Touch _touch1)
    {
        GetMiddlePoint(_touch0, _touch1); 
        //currentDelta = touchTop.deltaPosition;
        currentTouch0Delta = _touch0.deltaPosition; // DEBUG

        // distance entre maxZoomOut et dioramaTransf.position - maxZoomIn
        currentDistance = Vector3.Distance(transfToMove.position, dioramaTransf.position) - maxZoomIn; 

        zoomPointEnd = mainCam.ScreenToWorldPoint(new Vector3(middlePoint.x, middlePoint.y, 10f)); // hardcoded 10f CAN BE PROBLEMATIC
        // it can be weird to zoom like crazy even though only ONE finger from the pinch moved
        // use touchTop.deltaPosition : NO NEED FOR IF/ELSE
        if (touchTop.phase == TouchPhase.Moved || touchBottom.phase == TouchPhase.Moved || updateGamefeelCurve)
        {
            if (!updateGamefeelCurve)
            {
                dotProduct = Vector2.Dot(Controls.InitialTouch0Direction.normalized, (currentTouch0Delta).normalized);
                ZoomingOut = Mathf.Sign(dotProduct) == -1;
            }

            canZoomIn = zoomValue > maxZoomIn;
            canZoomOut = zoomValue < maxZoomOut;
            if (ZoomingOut)
            {
                if (canZoomOut)
                {
                    zoomValue++;
                    ZoomingIn = false; 

                    transfToMove.position -= (zoomPointEnd - mainCam.transform.position).normalized * Time.deltaTime * 
                        (updateGamefeelCurve ?
                        curvedSpeed :
                        zoomSpeed);
                }
            }
            else 
            {
                if (canZoomIn)
                {
                    zoomValue--;
                    ZoomingIn = true;

                    transfToMove.position += (zoomPointEnd - mainCam.transform.position).normalized * Time.deltaTime * 
                        (updateGamefeelCurve ?
                        curvedSpeed :
                        zoomSpeed);
                }
            }
        }

        // previousDelta = currentDelta;
    }

    private void InterruptPreviousCurveOnNewTouch()
    {
        if (gamefeelCurve.EvaluateCurve)
        {
            gamefeelCurve.EndGamefeelCurve();
        }
    }

    private void TriggerGamefeelCurveOnInputStateChange(TouchState currentState)
    {
        if (currentState == TouchState.Zooming)
        {
            updateGamefeelCurve = true;
        }
    }

    private void ResetBoolValues()
    {
        GameLogger.Log("on zoom ended callback");
        updateGamefeelCurve = false;
        ZoomingOut = ZoomingIn = false; 
    }
}
