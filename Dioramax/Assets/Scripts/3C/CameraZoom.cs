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
    [SerializeField, Range(5, 50)] private float maxZoomIn = 45;
    [SerializeField, Range (70, 150)] private float maxZoomOut = 70;
    [SerializeField, Range(10f, 50f)] private float zoomSpeed = 10f;
    private float currentMoveSpeed; 


    private Touch touchTop;
    private Touch touchBottom; 

    private bool zoomStartIsRegistered;

    private bool zoomingOut;
    private bool canZoomIn;
    private bool canZoomOut;

    private Vector3 zoomPointStart;
    private Vector3 zoomPointEnd;

    private float zoomValue;

    // GAMEFEEL STILL WIP (need to check how far I am from min or max zoom value
    [Header("Gamefeel")]
    [SerializeField] CurveEvaluator gamefeelCurve;
    private bool updateGamefeelCurve;

    UnityAction OnEvaluationEndedCallback;

    /*private void OnEnable()
    {
        Controls.OnTouchStarted += InterruptPreviousCurveOnNewTouch;
        Controls.OnTouchEnded += TriggerGamefeelCurveOnInputStateChange;
        OnEvaluationEndedCallback += SetToFalse;
    }

    private void OnDisable()
    {
        Controls.OnTouchStarted -= InterruptPreviousCurveOnNewTouch;
        Controls.OnTouchEnded -= TriggerGamefeelCurveOnInputStateChange;
        OnEvaluationEndedCallback -= SetToFalse;
    } */

    private void Start()
    {
        mainCam = Camera.main;
        Input.multiTouchEnabled = true;
        zoomValue = mainCam.fieldOfView;
        canZoomIn = canZoomOut = true; 
    }

    /* private void Update()
    {
        if (updateGamefeelCurve)
        {
            // Debug.Log("zoom gamefeel");
            currentMoveSpeed = moveSpeed * gamefeelCurve.Evaluate(OnEvaluationEndedCallback); 
            UpdatePinch(touchTop, touchBottom); // even more stupid to check tose again in the function..  
        }
    } */

    // NEVER USED
    public void SetPinchRegisterValue(bool value)
    {
        zoomStartIsRegistered = value;
    }

    private float topPosition;
    private Vector2 previousDelta, currentDelta;
    private float xDelta, yDelta;
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
    public void UpdatePinch(Touch _touch0, Touch _touch1)
    {
        GetMiddlePoint(_touch0, _touch1); 
        currentDelta = touchTop.deltaPosition;
        currentTouch0Delta = _touch0.deltaPosition; // DEBUG

        zoomPointEnd = mainCam.ScreenToWorldPoint(new Vector3(middlePoint.x, middlePoint.y, 10f)); // hardcoded 10f CAN BE PROBLEMATIC
        // it can be weird to zoom like crazy even though only ONE finger from the pinch moved
        // use touchTop.deltaPosition : NO NEED FOR IF/ELSE
        if (touchTop.phase == TouchPhase.Moved || touchBottom.phase == TouchPhase.Moved)
        {
            dotProduct = Vector2.Dot(Controls.InitialTouch0Direction.normalized, (currentTouch0Delta).normalized);
            zoomingOut = Mathf.Sign(dotProduct) == -1; 
            // Debug.Log("dot product is : " + dotProduct);

            canZoomIn = zoomValue > maxZoomIn;
            canZoomOut = zoomValue < maxZoomOut;
            if (zoomingOut)
            {
                if (canZoomOut)
                {
                    // Debug.Log("zooming out");
                    zoomValue++;

                    transform.position -= (zoomPointEnd - mainCam.transform.position).normalized * Time.deltaTime *
                        (updateGamefeelCurve ?
                        currentMoveSpeed :
                        zoomSpeed);
                }
            }
            else 
            {
                if (canZoomIn)
                {
                    // Debug.Log("zooming in");
                    zoomValue--;

                    transform.position += (zoomPointEnd - mainCam.transform.position).normalized * Time.deltaTime *
                        (updateGamefeelCurve ?
                        currentMoveSpeed :
                        zoomSpeed);
                }
            }
        }

        previousDelta = currentDelta;
    }
}
