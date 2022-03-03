using System;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private GameObject diorama;

    [SerializeField, Range(0.2f, 5f)] private float XYForceMultiplier = 2f;
    [SerializeField, Range(3f, 12f)] private float ZForceMultiplier = 8f;

    [SerializeField, Range(0, 50)] private float rotationSensitivity = 5f;

    private Vector2 rotationAxis;

    private Touch touchTop;

    private void Start()
    {
        transform.position = diorama.transform.position; // to set dynamically at the start of a level
    }

    /// <summary>
    /// Move the parent along X and Y axis. If you want to only rotate the camera frame, use "UpdateZRotation instead
    /// </summary>
    /// <param name="rotationDirection">Direction of camera displacement, based on direction of swipe</param>
    /// <param name="rotationForce">The speed of displacement</param>
    /// <returns>Wether the swipe force is greater than the sensibility settings. Otherwise, it won't rotate</returns>
    public bool UpdateXYRotation(Vector3 rotationDirection, float rotationForce)
    {
        if (rotationForce >= rotationSensitivity) 
        {
            // to always get an axis that is 90° more than direction
            rotationAxis = new Vector2(-rotationDirection.y, rotationDirection.x); // -y
            transform.Rotate(rotationAxis, Time.deltaTime * XYForceMultiplier * rotationForce);
        }

        return rotationForce >= rotationSensitivity; 
    }

    /// <summary>
    /// No object moves during this rotation. It is applied to the camera parent, around its Z axis. This is how the physics-based mechanic is triggered
    /// </summary>
    /// <param name="topDirection">The direction of swipe from the top finger, usually the index</param>
    /// <param name="bottomDirection">The direction of swipe from the thumb</param>
    public void UpdateZRotation(Touch _touch0, Touch _touch1, out float topPosition, float rotationForce)
    {
        topPosition = Mathf.Max(_touch0.position.y, _touch1.position.y);

        // stupid to do this every frame. But how to keep reference to the arguments each frame ?
        if (topPosition == _touch0.position.y)
        {
            touchTop = _touch0;
        }
        else if (topPosition == _touch1.position.y)
        {
            touchTop = _touch1;
        }

        transform.Rotate(transform.forward, Time.deltaTime * ZForceMultiplier * rotationForce * MathF.Sign(touchTop.deltaPosition.x));
    }   
}
    