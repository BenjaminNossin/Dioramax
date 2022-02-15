using UnityEngine;

public class TouchDetection : MonoBehaviour
{
    [SerializeField] private LayerMask cubeMask;
    [SerializeField] private PlaceholderFeedback placeholderFeedback;

    private Camera mainCam;
    private bool objectDetected; 

    void Start()
    {
        mainCam = Camera.main;
    }

    public bool TryCastToTarget(Vector3 touchStart, Vector3 toucheEnd)
    {
        Debug.DrawRay(touchStart, (toucheEnd - touchStart) * 100f, Color.red, 0.5f);
        objectDetected = Physics.Raycast(touchStart, (toucheEnd - touchStart), out RaycastHit hitInfo, 100f, cubeMask); 
        // use hit info

        if (objectDetected)
        {
            Debug.DrawRay(touchStart, (toucheEnd - touchStart) * 100f, Color.green, 0.5f);
            placeholderFeedback.ChangeColor();
        }

        return objectDetected;
    }
}
