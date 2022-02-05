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
    [SerializeField] private CharacterRotation characterRotation;
    [SerializeField] private CameraZoom cameraZoom;

    private Touch currentTouch0;
    // private Touch currentTouch1;

    private Vector3 touch0StartPosition;
    private Vector3 currentTouch0Position;

    // private Vector3 touch1StartPosition;
    // private Vector3 current1TouchPosition;

    private void Update()
    {
        // ROTATION
        if (Input.touchCount == 1)
        {
            currentTouch0 = Input.GetTouch(0);
            currentTouch0Position = currentTouch0.position; 

            if (currentTouch0.phase == TouchPhase.Began)
            {
                touch0StartPosition = currentTouch0Position; 
                characterRotation.Initialize(touch0StartPosition, currentTouch0Position); 
            }
            else if (currentTouch0.phase == TouchPhase.Moved && Vector2.Distance(touch0StartPosition, currentTouch0.deltaPosition) > 10f)
            {
                characterRotation.UpdateRotation(touch0StartPosition, currentTouch0Position);
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                characterRotation.EndRotationUpdate(touch0StartPosition, currentTouch0Position);
                // Debug.Log($"distance from start is {Vector2.Distance(touchStart, currentTouchPosition)}");
            }
        }
        // ZOOM IN and OUT
        else if (Input.touchCount == 2)
        {
            cameraZoom.UpdatePinch(Input.GetTouch(0), Input.GetTouch(1)); // bad to read Input.GetTouch() again 
            cameraZoom.SetPinchRegisterValue(true);

            if (Input.GetTouch(1).phase == TouchPhase.Ended)
            {
                cameraZoom.SetPinchRegisterValue(false);
            }
        }
    }
}
