using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Input.touches to track multiple fingers -> zoom
// touchCount
// touchSupported
// GetTouch -> .tapCount, .position, .phase, .deltaTime, .deltaPosition
/* The touch lifecycle describes the state of a touch in any given frame:

Began - A user has touched their finger to the screen this frame
Stationary - A finger is on the screen but the user has not moved it this frame
Moved - A user moved their finger this frame
Ended - A user lifted their finger from the screen this frame
Cancelled - The touch was interrupted this frame 
*/

public class Controls : MonoBehaviour
{
    [SerializeField] private CameraRotation cameraRotation;
    [SerializeField] private CameraZoom cameraZoom;
    [SerializeField] private TouchDetection touchDetection;

    private Camera mainCam;

    private Touch currentTouch0;
    // private Touch currentTouch1;

    private Vector3 touch0StartPosition;
    private Vector3 touch0CurrentPosition;
    private Vector3 swipeDirection;
    private float swipeForce; 

    private bool touch1HasBeenUnregistered; // I couldn't call cameraZoom.SetPinchRegisterValue(false) otherwise.. 
                                            // but maybe there is a better solution

    // DEBUG
    private float distance;
    private Vector3 screenToWorldPoint1, screentToWorldPoint2; 

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Input.touchCount == 0 && !touch1HasBeenUnregistered)
        {
            touch1HasBeenUnregistered = true;
            cameraZoom.SetPinchRegisterValue(false);
        }

        // ROTATION
        if (Input.touchCount == 1)
        {
            // end of zoom state
            if (!touch1HasBeenUnregistered)
            {
                touch1HasBeenUnregistered = true;
                cameraZoom.SetPinchRegisterValue(false);
            }

            currentTouch0 = Input.GetTouch(0);
            touch0CurrentPosition = new Vector3(currentTouch0.position.x, currentTouch0.position.y, mainCam.nearClipPlane);

            swipeDirection = currentTouch0.deltaPosition;
            swipeForce = swipeDirection.magnitude; // can go up to 200

            if (currentTouch0.phase == TouchPhase.Began)
            {
                touch0StartPosition = mainCam.ScreenToWorldPoint(touch0CurrentPosition);

                cameraRotation.Initialize(touch0StartPosition, touch0CurrentPosition); // usefull ?
                touchDetection.TouchFeedback(touch0StartPosition, touch0CurrentPosition);
            }
            else if (currentTouch0.phase == TouchPhase.Moved)
            {
                cameraRotation.UpdateRotation(swipeDirection.normalized, swipeForce * 0.01f);

                screenToWorldPoint1 = touch0StartPosition;
                screentToWorldPoint2 = mainCam.ScreenToWorldPoint(touch0CurrentPosition);

                // distance = Vector3.Distance(screenToWorldPoint1, screentToWorldPoint2);
                // delta can go up to 300
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                cameraRotation.EndRotationUpdate(touch0StartPosition, touch0CurrentPosition);
                // Debug.Log($"distance from start is {Vector2.Distance(touchStart, currentTouchPosition)}");
            }
        }
        // ZOOM IN and OUT
        else if (Input.touchCount == 2)
        {
            touch1HasBeenUnregistered = false; 
            cameraZoom.UpdatePinch(Input.GetTouch(0), Input.GetTouch(1)); // bad to read Input.GetTouch() again 
            cameraZoom.SetPinchRegisterValue(true);
        }
    }
}
