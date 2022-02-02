using System;
using UnityEngine;

public class InputsController : MonoBehaviour
{
    [SerializeField] private LayerMask cubeMask;
    [SerializeField] private Camera camera;
    [SerializeField] private PlaceholderFeedback placeholderFeedback;

    private Vector3 origin;
    private Vector3 end;
    private Touch touch;

    private Vector3 touchStart;
    private Vector3 currentTouchPosition; 
    
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

    public void Update()
    {
        /* if (Input.GetMouseButtonDown(0))
        {
            origin = transform.position; 
            end = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 30f));
            Debug.DrawRay(origin, (end-origin) * 100f, Color.red, 0.5f); 
            
            if (Physics.Raycast(origin, (end-origin), out RaycastHit hitInfo, 100f, cubeMask))
            {
                Debug.DrawRay(origin, (end-origin), Color.green, 0.5f); 
                placeholderFeedback.ChangeColor();
            }
        } */ 

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touchStart = Input.GetTouch(0).position;
                Debug.Log($"touch start at {touchStart}");
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                currentTouchPosition = Input.GetTouch(0).position;
                Debug.Log($"finger is moving");
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                touchStart = camera.ScreenToWorldPoint(touchStart);
                currentTouchPosition = camera.ScreenToWorldPoint(currentTouchPosition);
                
                Debug.Log($"distance from start is {Vector3.Distance(touchStart, currentTouchPosition)}");
            }
        } 
    }
}
