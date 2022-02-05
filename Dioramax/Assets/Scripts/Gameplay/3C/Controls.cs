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

    private Touch currentTouch1;
    // private Touch currentTouch2;

    private Vector3 touch1StartPosition;
    private Vector3 currentTouch1Position;

    // private Vector3 touch2StartPosition;
    // private Vector3 current2TouchPosition;

    private void Update()
    {
        if (Input.touchCount == 1)
        {
            currentTouch1 = Input.GetTouch(0);
            currentTouch1Position = currentTouch1.position; 

            if (currentTouch1.phase == TouchPhase.Began)
            {
                touch1StartPosition = currentTouch1Position; 
                characterRotation.Initialize(touch1StartPosition, currentTouch1Position); 
            }
            else if (currentTouch1.phase == TouchPhase.Moved && Vector2.Distance(touch1StartPosition, currentTouch1.deltaPosition) > 10f)
            {
                characterRotation.UpdateRotation(touch1StartPosition, currentTouch1Position);
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                characterRotation.EndRotationUpdate(touch1StartPosition, currentTouch1Position);
                // Debug.Log($"distance from start is {Vector2.Distance(touchStart, currentTouchPosition)}");
            }
        }
    }
}
