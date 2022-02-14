using UnityEngine;

public class TouchDetection : MonoBehaviour
{
    [SerializeField] private LayerMask cubeMask;
    [SerializeField] private PlaceholderFeedback placeholderFeedback;

    private Camera mainCam;
    private Vector3 cameraOrigin;
    private bool objectDetected; 

    void Start()
    {
        mainCam = Camera.main;
    }

    public bool TryCastToTarget(Vector3 touchStart, Vector3 toucheEnd)
    {
        cameraOrigin = transform.position;
        toucheEnd = mainCam.ScreenToWorldPoint(new Vector3(touchStart.x, touchStart.y, 1f));

        Debug.DrawRay(cameraOrigin, (toucheEnd - cameraOrigin) * 100f, Color.red, 0.5f);
        objectDetected = Physics.Raycast(cameraOrigin, (toucheEnd - cameraOrigin), out RaycastHit hitInfo, 100f, cubeMask); 

        if (objectDetected)
        {
            Debug.DrawRay(cameraOrigin, (toucheEnd - cameraOrigin) * 100f, Color.green, 0.5f);
            placeholderFeedback.ChangeColor();
        }
        return objectDetected;
    }
}
