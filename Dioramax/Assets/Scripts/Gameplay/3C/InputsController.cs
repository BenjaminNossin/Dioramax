using System;
using UnityEngine;

public class InputsController : MonoBehaviour
{
    [SerializeField] private LayerMask cubeMask;
    [SerializeField] private Camera camera;
    [SerializeField] private PlaceholderFeedback placeholderFeedback;
    [SerializeField] private GameObject diorama;
    [SerializeField, Range(0.25f, 5f)] private float sensitivity = 1.5f; 

    private Vector3 cameraOrigin;
    private Vector3 end;
    private Touch touch;

    private Vector3 touchStart;
    private Vector3 currentTouchPosition;

    private Vector3 swipeDirection;
    private Vector3 initialRotation;

    private float angularSpeed; 

    public enum SpeedType { One, Two, Three }
    public SpeedType speedType; 

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

    private void Start()
    {
        angularSpeed = sensitivity * 360f; 
    }

    public void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touchStart = Input.GetTouch(0).position;
                initialRotation = diorama.transform.rotation.eulerAngles;
                // Debug.Log($"touch start at {camera.ScreenToWorldPoint(new Vector3(touchStart.x, touchStart.y, 30f))}");

                cameraOrigin = transform.position;
                currentTouchPosition = camera.ScreenToWorldPoint(new Vector3(touchStart.x, touchStart.y, 1f));
                Debug.DrawRay(cameraOrigin, (currentTouchPosition - cameraOrigin) * 100f, Color.red, 0.5f);

                if (Physics.Raycast(cameraOrigin, (currentTouchPosition - cameraOrigin), out RaycastHit hitInfo, 100f, cubeMask))
                {
                    Debug.DrawRay(cameraOrigin, (currentTouchPosition - cameraOrigin) * 100f, Color.green, 0.5f);
                    placeholderFeedback.ChangeColor();
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved && Vector2.Distance(touchStart, Input.GetTouch(0).deltaPosition) > 10f)
            {
                touchStart = camera.ScreenToWorldPoint(new Vector3(touchStart.x, touchStart.y, 1f));

                currentTouchPosition = Input.GetTouch(0).position;
                currentTouchPosition = camera.ScreenToWorldPoint(new Vector3(currentTouchPosition.x, currentTouchPosition.y, 1f));

                swipeDirection = (currentTouchPosition - touchStart).normalized; 
                // Debug.Log("swipde direction is " + new Vector2(swipeDirection.x, swipeDirection.y));
                UpdateRotation(); 
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                currentTouchPosition = Input.GetTouch(0).position;

                touchStart = camera.ScreenToWorldPoint(new Vector3(touchStart.x, touchStart.y, 1f));
                currentTouchPosition = camera.ScreenToWorldPoint(new Vector3(currentTouchPosition.x, currentTouchPosition.y, 1f));
                
                // Debug.Log($"distance from start is {Vector2.Distance(touchStart, currentTouchPosition)}");
            }
        } 
    }

    // MOVE TO CHARACTER CONTROLLER (rotation/zoom)
    // WARNING -> this rotation is mathematically accurate, but NOT intuitive at all from a player's perspective.. 
    private void UpdateRotation()
    {
        Vector3 relativePos = (currentTouchPosition - transform.position);

        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        diorama.transform.rotation = Quaternion.Inverse(rotation); // how to increase rotation speed ?? 
    }
}
