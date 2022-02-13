using System;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private GameObject diorama; 
    [SerializeField, Range(5f, 50f)] private float sensitivity = 20f;
    public Vector2 swipeDirection; 

    private Camera mainCam;
    private Vector3 cameraOrigin;

    // important (but still not used)
    private Vector2 rotationAxis;

    private void Start()
    {
        mainCam = Camera.main; 
    }

    private void Update()
    {
        // Debug.Log($"sin value is " + MathF.Sin(Vector2.Angle(Vector3.down, swipeDirection)));
        // Debug.Log($"Cos value is " + MathF.Cos(Vector2.Angle(Vector3.down, swipeDirection)));

        // if swipe direction = 0,1, sin should be -1
        // generated 90° angle should be -1,0 (by multiplying x by sign of sin


        // swipe right 1,0,0
        // mainCam.transform.RotateAround(diorama.transform.position, Vector3.up, Time.deltaTime * -sensitivity * Mathf.Sign(Vector3.right.x));

        // swipe up 0,1,0
        // mainCam.transform.RotateAround(diorama.transform.position, Vector3.right, Time.deltaTime * sensitivity * Mathf.Sign(Vector3.up.x));

        // swipe diag top/right 1,1,0 
        // -1,1,0
        // mainCam.transform.RotateAround(diorama.transform.position, new Vector3(-1,1,0), Time.deltaTime * -sensitivity * Mathf.Sign(1));

        // swipe diag bot/left -1,1,0
        // 1,1,0
        // mainCam.transform.RotateAround(diorama.transform.position, new Vector3(1, 1, 0), Time.deltaTime * sensitivity * Mathf.Sign(-1));
    }


    public void Initialize(Vector3 touchStart, Vector3 currentTouchPosition)
    {
        // initialRotation = diorama.transform.rotation.eulerAngles;
        // Debug.Log($"touch start at {camera.ScreenToWorldPoint(new Vector3(touchStart.x, touchStart.y, 30f))}");
    }

    public void UpdateRotation(Vector3 rotationDirection, float rotationForce)
    {
        // rotation axis is always 90° compared to swipe direction 
        rotationAxis = new Vector2(rotationDirection.y, -rotationDirection.x);
        float angle = Vector2.Angle(rotationAxis, swipeDirection); 

        // Debug.Log("angle is " + angle);
        // Debug.Log($"rotation axis is {rotationAxis}"); 
        Debug.Log("rotating"); 

        mainCam.transform.RotateAround(
            diorama.transform.position,
            rotationAxis,
            Time.deltaTime * sensitivity * rotationForce); // * Mathf.Sign(Vector3.right.x));
    }

    public void EndRotationUpdate(Vector3 touchStart, Vector3 currentTouchPosition)
    {
        currentTouchPosition = Input.GetTouch(0).position;
    }
}
