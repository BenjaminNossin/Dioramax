using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField, Range(15, 40)] private float maxZoomIn = 25;
    [SerializeField, Range (70, 120)] private float maxZoomOut = 100;
    [SerializeField, Range(0.5f, 5f)] private float zoomForceSensibility = 2.5f;

    private Touch touchTop;
    private Touch touchBottom; 

    private bool zoomStartIsRegistered;

    private float topPositionStartY;
    private bool zoomingIn;

    // DEBUG
    private float touchTopDelta, touchBottomDelta; 

    private void Start()
    {
        mainCam = Camera.main;
        Input.multiTouchEnabled = true;
    }

    private void Update()
    {
        mainCam.fieldOfView = Mathf.Clamp(mainCam.fieldOfView, maxZoomIn, maxZoomOut);
    }

    public void SetPinchRegisterValue(bool value)
    {
        zoomStartIsRegistered = value;
    }

    public void UpdatePinch(Touch _touch0, Touch _touch1)
    {
        // on first frame

        topPositionStartY = Mathf.Max(_touch0.position.y, _touch1.position.y);

        // stupid to do this every frame. But how to keep reference to the arguments each frame ?
        if (topPositionStartY == _touch0.position.y)
        {
            touchTop = _touch0;
            touchBottom = _touch1;
        }
        else if (topPositionStartY == _touch1.position.y)
        {
            touchTop = _touch1;
            touchBottom = _touch0;
        }

        // it can be weird to zoom like crazy even though only ONE finger from the pinch moved
        if (touchTop.phase == TouchPhase.Moved || touchBottom.phase == TouchPhase.Moved)
        {
            zoomingIn = Mathf.Sign(touchTop.deltaPosition.y) == -1 || Mathf.Sign(touchBottom.deltaPosition.y) == 1;
            touchTopDelta = touchTop.deltaPosition.y;
            touchBottomDelta = touchBottom.deltaPosition.y; 
            // too sensible !! (sometimes zoom in for two or three frames during zoom out or vice versa
            if (zoomingIn)
            {
                Debug.Log("zooming in");
                mainCam.fieldOfView += zoomForceSensibility;
            }
            else
            {
                Debug.Log("zooming out");
                mainCam.fieldOfView -= zoomForceSensibility;
            }
        }
    }
}
