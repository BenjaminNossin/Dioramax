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

    private bool zoomingOut;
    private bool canZoomIn;
    private bool canZoomOut;

    private Vector3 zoomPoint;
    private float zoomValue; 

    private void Start()
    {
        mainCam = Camera.main;
        Input.multiTouchEnabled = true;
        zoomValue = mainCam.fieldOfView; 
    }

    public void SetPinchRegisterValue(bool value)
    {
        zoomStartIsRegistered = value;
    }

    private const float moveSpeed = 10f; 
    public void UpdatePinch(Touch _touch0, Touch _touch1, out float topPosition)
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
        zoomPoint = mainCam.ScreenToWorldPoint(new Vector3(middlePoint.x, middlePoint.y, 0f));
        zoomPoint += new Vector3(0f, 0f, transform.InverseTransformDirection(mainCam.transform.forward).z);

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
                Debug.Log("zooming out");
                /* mainCam.fieldOfView = currentCamFieldOfView < maxZoomOut - zoomForceSensibility ?
                    currentCamFieldOfView + zoomForceSensibility :
                    maxZoomOut; */

                if (canZoomOut)
                {
                    zoomValue++;
                    mainCam.transform.Translate((zoomPoint - mainCam.transform.position).normalized * -Time.deltaTime * moveSpeed, Space.Self);
                }
            }
            else
            {
                Debug.Log("zooming in");
                /* mainCam.fieldOfView = currentCamFieldOfView > maxZoomIn + zoomForceSensibility ?
                    currentCamFieldOfView - zoomForceSensibility :
                    maxZoomIn; */
                if (canZoomIn)
                {
                    zoomValue--;
                    mainCam.transform.Translate((zoomPoint - mainCam.transform.position).normalized * Time.deltaTime * moveSpeed, Space.Self);
                }
            }
        }
    }
}
