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

public enum TouchState { None, Tap, Hold, DoubleTap, Drag, Rotating, Zooming, UNDEFINED }

public class Controls : MonoBehaviour
{
    [SerializeField] private CameraRotation cameraRotation;
    [SerializeField] private CameraZoom cameraZoom;
    [SerializeField] private TouchDetection touchDetection;
    [SerializeField, 
        Tooltip("How much before a tap is considered a hold"), Range(0, 20)] 
    private int frameCountBeforeTapToHold = 10; // no gameplay usage for now, just transition logic

    private Camera mainCam;

    private Touch currentTouch0, currentTouch1; 
    private TouchState touchState; 
    // private Touch currentTouch1;

    private Vector3 cameraPosition;
    private Vector3 touch0CurrentPosition;


    private Vector3 swipeDirection;
    private float swipeForce;

    private bool touch1HasBeenUnregistered = true; // I couldn't call cameraZoom.SetPinchRegisterValue(false) otherwise.. 
                                                   // but maybe there is a better solution

    private int FrameCount { get; set; } 

    private void Start()
    {
        mainCam = Camera.main;
        touchState = TouchState.None; 
    }

    private void Update()
    {
        // MAJ 14.02.2022 -> untested
        if (Input.touchCount < 2 && !touch1HasBeenUnregistered)
        {
            Debug.Log("unregistering touch 1"); 
            SetPinch(true, false);
        }

        // ROTATION
        if (Input.touchCount == 1)
        {
            UpdateMonotouch();

            if (currentTouch0.phase == TouchPhase.Began)
            {
                SetTouchState(TouchState.Tap);
                cameraPosition = mainCam.transform.position;

                // if something detected, enter swipe and NOT rotating state
                if (touchDetection.TryCastToTarget(cameraPosition, touch0CurrentPosition))
                {
                    Debug.Log("detected an interactable object"); 
                    SetTouchState(TouchState.Drag);
                }

                Debug.Log("touch state is " + touchState);
            }
            else if (currentTouch0.phase == TouchPhase.Stationary)
            {
                FrameCount++; 

                if (FrameCount >= frameCountBeforeTapToHold)
                {
                    Debug.Log("transition to Hold state");
                    SetTouchState(TouchState.Hold);
                }
            }
            else if (currentTouch0.phase == TouchPhase.Moved) 
            {
                SetTouchState(TouchState.UNDEFINED); // one way to make up for the super high sensitivity of Stationary/Moved state (no dead zone)
                if (cameraRotation.UpdateXYRotation(swipeDirection.normalized, swipeForce))
                {
                    Debug.Log("not swiping. State is now Rotating");
                    SetTouchState(TouchState.Rotating);
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Debug.Log("finger was removed from screen");
                FrameCount = 0; 
                SetTouchState(TouchState.None);
            }
        }
        // ZOOM IN and OUT
        else if (Input.touchCount == 2)
        {
            Debug.Log("Two fingers are on the screen");

            currentTouch1 = Input.GetTouch(1); 
            SetPinch(false, true);
            cameraZoom.UpdatePinch(Input.GetTouch(0), currentTouch1); // bad to read Input.GetTouch() again 

            // Z rotation
        }
    }

    private void SetTouchState(TouchState newState)
    {
        touchState = newState;
    }

    // use input.touches:Touch[] instead ? But means I have to recreate an array every time touchCount changes.. 
    private void UpdateMonotouch()
    {
        Debug.Log("updating monotouch"); 

        currentTouch0 = Input.GetTouch(0);
        touch0CurrentPosition = mainCam.ScreenToWorldPoint(new Vector3(
            currentTouch0.position.x, 
            currentTouch0.position.y, 
            mainCam.nearClipPlane)); 

        swipeDirection = currentTouch0.deltaPosition;
        swipeForce = swipeDirection.magnitude; // can go up to 200
    }

    private void SetPinch(bool _touch1HasBeenUnregistered, bool _cameraPinchRegisterValueTo)
    {
        touch1HasBeenUnregistered = _touch1HasBeenUnregistered;
        cameraZoom.SetPinchRegisterValue(_cameraPinchRegisterValueTo);
        SetTouchState(_cameraPinchRegisterValueTo == false ? TouchState.None : TouchState.Zooming); 
    }
}
