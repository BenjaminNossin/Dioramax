using System;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private GameObject diorama;

    [SerializeField, Range(0.2f, 5f)] private float rotationForce = 2f;
    [SerializeField, Range(0, 50)] private float rotationSensitivity = 5f;

    private Camera mainCam;
    private Vector2 rotationAxis;

    private void Start()
    {
        mainCam = Camera.main;
        transform.position = diorama.transform.position; // to set dynamically at the start of a level
    }

    public void UpdateRotation(Vector3 rotationDirection, float rotationForce)
    {
        // grand-parent only Y axis
        // parent only X axis
        // camera does not rotate

        if (rotationForce >= rotationSensitivity) 
        {
            // to always get an axis that is 90° more than direction
            rotationAxis = new Vector2(-rotationDirection.y, rotationDirection.x); // -y

            /* transform.RotateAround(
                diorama.transform.position,
                rotationAxis,
                Time.deltaTime * this.rotationForce * rotationForce); */

            transform.Rotate(rotationAxis, Time.deltaTime * this.rotationForce * rotationForce); 
        }
    }

    public void EndRotationUpdate(Vector3 touchStart, Vector3 currentTouchPosition)
    {
        currentTouchPosition = Input.GetTouch(0).position;
    }
}
