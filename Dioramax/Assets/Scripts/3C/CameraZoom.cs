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
    [SerializeField, Range(15, 50)] private float maxZoomIn = 45;
    [SerializeField, Range (70, 120)] private float maxZoomOut = 70;
    [SerializeField, Range(0.5f, 5f)] private float zoomForceSensibility = 2.5f;
    [SerializeField, Range(1f, 20f)] private float moveSpeed = 10f;
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

    [Header("Gamefeel")]
    [SerializeField] CurveEvaluator gamefeelCurve;
    private bool updateGamefeelCurve;

    UnityAction OnEvaluationEndedCallback;

    private void OnEnable()
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
    }

    private void Start()
    {
        mainCam = Camera.main;
        Input.multiTouchEnabled = true;
        zoomValue = mainCam.fieldOfView; 
    }

    private void Update()
    {
        if (updateGamefeelCurve)
        {
            // Debug.Log("zoom gamefeel");
            currentMoveSpeed = moveSpeed * gamefeelCurve.Evaluate(OnEvaluationEndedCallback); 
            UpdatePinch(touchTop, touchBottom); // even more stupid to check tose again in the function..  
        }
    }

    public void SetPinchRegisterValue(bool value)
    {
        zoomStartIsRegistered = value;
    }

    private float topPosition; 
    public void UpdatePinch(Touch _touch0, Touch _touch1)
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

        Vector3 middlePoint = Vector3.Lerp(touchTop.position, touchBottom.position, 0.5f);
        zoomPointEnd = mainCam.ScreenToWorldPoint(new Vector3(middlePoint.x, middlePoint.y, 10f)); 
        // it can be weird to zoom like crazy even though only ONE finger from the pinch moved
        // use touchTop.deltaPosition : NO NEED FOR IF/ELSE
        if (touchTop.phase == TouchPhase.Moved || touchBottom.phase == TouchPhase.Moved)
        {
            zoomingOut = Mathf.Sign(touchTop.deltaPosition.y) == -1 || Mathf.Sign(touchBottom.deltaPosition.y) == 1;

            // too sensible !! (sometimes zoom in for two or three frames during zoom out or vice versa
            canZoomIn = zoomValue > maxZoomIn;
            canZoomOut = zoomValue < maxZoomOut; 
            if (zoomingOut)
            {
                if (canZoomOut)
                {
                    zoomValue++;

                    transform.position -= (zoomPointEnd - mainCam.transform.position).normalized * Time.deltaTime * 
                        (updateGamefeelCurve ? 
                        currentMoveSpeed : 
                        moveSpeed); 
                }
            }
            else
            {
                if (canZoomIn)
                {
                    zoomValue--;
                    transform.position += (zoomPointEnd - mainCam.transform.position).normalized * Time.deltaTime *
                        (updateGamefeelCurve ?
                        currentMoveSpeed :
                        moveSpeed);
                }
            }
        }
    }

    private void InterruptPreviousCurveOnNewTouch()
    {
        if (gamefeelCurve.EvaluateCurve)
        {
            gamefeelCurve.EndGamefeelCurve();
        }
    }

    private void TriggerGamefeelCurveOnInputStateChange(TouchState previous)
    {
        if (previous == TouchState.Zooming)
        {
            updateGamefeelCurve = true;
        }
    }

    private void SetToFalse()
    {
        Debug.Log("on ended zooming callback");
        updateGamefeelCurve = false;
    }
}
