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

    /// <summary>
    /// Move the parent along X and Y axis. If you want to only rotate the camera frame, use "UpdateZRotation instead
    /// </summary>
    /// <param name="rotationDirection">Direction of camera displacement, based on direction of swipe</param>
    /// <param name="rotationForce">The speed of displacement</param>
    public void UpdateXYRotation(Vector3 rotationDirection, float rotationForce)
    {
        if (rotationForce >= rotationSensitivity) 
        {
            // to always get an axis that is 90° more than direction
            rotationAxis = new Vector2(-rotationDirection.y, rotationDirection.x); // -y
            transform.Rotate(rotationAxis, Time.deltaTime * this.rotationForce * rotationForce); 
        }
    }

    /// <summary>
    /// No object moves during this rotation. It is applied to the camera parent, around its Z axis. This is how the physics-based mechanic is triggered
    /// </summary>
    /// <param name="topDirection">The direction of swipe from the top finger, usually the index</param>
    /// <param name="bottomDirection">The direction of swipe from the thumb</param>
    public void UpdateZRotation(Vector3 topDirection, Vector3 bottomDirection)
    {

    }
}
