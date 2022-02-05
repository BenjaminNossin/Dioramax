using System;
using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
    [SerializeField] private LayerMask cubeMask;
    [SerializeField] private PlaceholderFeedback placeholderFeedback;
    [SerializeField] private GameObject diorama; 
    [SerializeField, Range(0.25f, 5f)] private float sensitivity = 1.5f;

    private Camera mainCam;
    private Vector3 cameraOrigin;

    // important (but still not used)
    private Vector3 initialRotation;
    private float angularSpeed; 

    private void Start()
    {
        angularSpeed = sensitivity * 360f; 
        mainCam = Camera.main; 
    }

    // WARNING -> this rotation is mathematically accurate, but NOT intuitive at all from a player's perspective.. 

    public void Initialize(Vector3 touchStart, Vector3 currentTouchPosition)
    {
        initialRotation = diorama.transform.rotation.eulerAngles;
        // Debug.Log($"touch start at {camera.ScreenToWorldPoint(new Vector3(touchStart.x, touchStart.y, 30f))}");

        cameraOrigin = transform.position;
        currentTouchPosition = mainCam.ScreenToWorldPoint(new Vector3(touchStart.x, touchStart.y, 1f));
        Debug.DrawRay(cameraOrigin, (currentTouchPosition - cameraOrigin) * 100f, Color.red, 0.5f);

        if (Physics.Raycast(cameraOrigin, (currentTouchPosition - cameraOrigin), out RaycastHit hitInfo, 100f, cubeMask))
        {
            Debug.DrawRay(cameraOrigin, (currentTouchPosition - cameraOrigin) * 100f, Color.green, 0.5f);
            placeholderFeedback.ChangeColor();
        }
    }

    // problem on first update frame (goes back to init frame and not to initial rotation of diorama
    public void UpdateRotation(Vector3 touchStart, Vector3 currentTouchPosition)
    {
        touchStart = mainCam.ScreenToWorldPoint(new Vector3(touchStart.x, touchStart.y, 1f));

        currentTouchPosition = Input.GetTouch(0).position;
        currentTouchPosition = mainCam.ScreenToWorldPoint(new Vector3(currentTouchPosition.x, currentTouchPosition.y, 1f));

        // Quaternions are HARD -> problem comes probably from here (initialRotation and angularSpeed)
        Vector3 relativePos = (currentTouchPosition - transform.position);

        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        diorama.transform.rotation = Quaternion.Inverse(rotation); // how to increase rotation speed ?? 
    }

    public void EndRotationUpdate(Vector3 touchStart, Vector3 currentTouchPosition)
    {
        currentTouchPosition = Input.GetTouch(0).position;

        touchStart = mainCam.ScreenToWorldPoint(new Vector3(touchStart.x, touchStart.y, 1f));
        currentTouchPosition = mainCam.ScreenToWorldPoint(new Vector3(currentTouchPosition.x, currentTouchPosition.y, 1f));
    }
}
