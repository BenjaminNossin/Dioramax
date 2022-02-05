using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField, Range(15, 40)] private float maxZoomIn = 25;
    [SerializeField, Range (70, 120)] private float maxZoomOut = 100;

    private Vector2 bottomPinchStart;
    private Vector2 topPinchStart;

    private Vector2 bottomPinchCurrent;
    private Vector2 topPinchCurrent;

    private bool startIsRegistered;

    private float zoomForce;
    private float distance;

    private Touch bottomTouch;
    private Touch topTouch;

    private void Start()
    {
        mainCam = Camera.main;
        Input.multiTouchEnabled = true;
    }

    private void Update()
    {
        if (mainCam.fieldOfView < maxZoomIn)
        {
            mainCam.fieldOfView = maxZoomIn;
        }
        else if (mainCam.fieldOfView > maxZoomOut)
        {
            mainCam.fieldOfView = maxZoomOut;
        }

        if (Input.touchCount >= 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            startIsRegistered = false;
        }

        if (Input.touchCount == 2)
        {
            startIsRegistered = true; 

            if (!startIsRegistered)
            {
                bottomTouch = Input.GetTouch(0);
                topTouch = Input.GetTouch(1); 

                bottomPinchStart = bottomTouch.position;
                topPinchStart = topTouch.position;

                bottomPinchCurrent = bottomPinchStart;
                topPinchCurrent = topPinchStart; 
            }

            if (bottomTouch.phase == TouchPhase.Moved || topTouch.phase == TouchPhase.Moved) 
            {
                bottomPinchCurrent = bottomTouch.position;
                topPinchCurrent = topTouch.position;

                zoomForce = Vector2.Max(bottomTouch.deltaPosition, topTouch.deltaPosition).normalized.magnitude; // 0 to 1

                // update zoom -> mainCam.fieldOfView
            }
        }
    }
}
