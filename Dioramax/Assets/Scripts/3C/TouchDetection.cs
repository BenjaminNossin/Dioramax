using UnityEngine;

/// <summary>
/// This script is in charge of managing interactable objects state change on tap/hold/drag.
/// It does NOT perform any logic on the objects except state change.
/// </summary>
public class TouchDetection : MonoBehaviour
{
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private PlaceholderFeedback placeholderFeedback;
    [SerializeField] private bool changeBackAfterDelay = true;

    private Camera mainCam;
    private bool objectDetected;
    private PlaceholderFeedback previous, currentTouched;

    void Start()
    {
        mainCam = Camera.main;
    }

    public bool TryCastToTarget(Vector3 touchStart, Vector3 toucheEnd)
    {
        Debug.DrawRay(touchStart, (toucheEnd - touchStart) * 100f, Color.red, 0.5f);
        objectDetected = Physics.Raycast(touchStart, (toucheEnd - touchStart), out RaycastHit hitInfo, 100f, interactableMask); 
        // use hit info

        if (objectDetected)
        {
            Debug.DrawRay(touchStart, (toucheEnd - touchStart) * 100f, Color.green, 0.5f);
            currentTouched = hitInfo.transform.GetComponent<PlaceholderFeedback>();
            currentTouched.ChangeColor(changeBackAfterDelay);

            SetPlaceholderReference(currentTouched);
            previous = currentTouched;
        }

        return objectDetected;
    }

    private void SetPlaceholderReference(PlaceholderFeedback current)
    {
        if (!previous) return; 

        if (previous != current)
        {
            previous.ChangeBack(); 
        }
    }
}
