using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField, Range(15, 40)] private float maxZoomIn = 25;
    [SerializeField, Range (70, 120)] private float maxZoomOut = 100;
    [SerializeField, Range(1f, 10f)] private float zoomForceSensibility = 3f;

    private Vector2 touch0PinchStart;
    private Vector2 touch1PinchStart;

    private float touch0PinchCurrent;
    private float touch1PinchCurrent;

    private bool startIsRegistered;

    private float top;
    private float bottom;

    private float currentTopPosition;
    private float currentBottomPosition;

    private float camFOV; 

    private float zoomForce; // not used
    // private float distance; // not set

    private Touch touch0;
    private Touch touch1;

    private bool zoomingIn; 

    // DEBUG
    private Vector2 bottomDelta, topDelta;

    private void Start()
    {
        mainCam = Camera.main;
        Input.multiTouchEnabled = true;
        camFOV = mainCam.fieldOfView;
    }

    private void Update()
    {
        if (mainCam.fieldOfView > maxZoomOut)
        {

        }
        else if (mainCam.fieldOfView < maxZoomIn)
        {

        }
    }

    public void SetPinchRegisterValue(bool value)
    {
        startIsRegistered = value;
    }

    public void UpdatePinch(Touch _touch0, Touch _touch1)
    {
        touch0 = _touch0;
        touch1 = _touch1;

        // on first frame
        if (!startIsRegistered)
        {
            touch0PinchStart = touch0.position; 
            touch1PinchStart = touch1.position;

            touch0PinchCurrent = touch0PinchStart.y; 
            touch1PinchCurrent = touch1PinchStart.y;
        }

        // it can be weird to zoom like crazy even though only ONE finger from the pinch moved
        if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
        {
            Debug.Log("updating zoom force");  
            touch0PinchCurrent = touch0.position.y; // useless ?
            touch1PinchCurrent = touch1.position.y; // useless ?

            top = Mathf.Max(touch0PinchCurrent, touch1PinchCurrent);
            bottom = Mathf.Min(touch0PinchCurrent, touch1PinchCurrent);

            // LOGIC ERROR FOR ZOOMINGIN
            zoomingIn = top < currentTopPosition || bottom > currentBottomPosition; 
            // too sensible !! (sometimes zoom in for two or three frames during zoom out or vice versa
            if (zoomingIn)
            {
                Debug.Log("zooming in");
                mainCam.fieldOfView += Time.deltaTime * zoomForceSensibility;
            }

             if (!zoomingIn)
            {
                Debug.Log("zooming out");
                mainCam.fieldOfView += Time.deltaTime * -zoomForceSensibility;
            }   
                
            // DEBUG
            bottomDelta = touch0.deltaPosition;
            topDelta = touch1.deltaPosition;
            // end of DEBUG

            currentTopPosition = Mathf.Max(touch0PinchCurrent, touch1PinchCurrent);
            currentBottomPosition = Mathf.Min(touch0PinchCurrent, touch1PinchCurrent);
        }
    }
}
