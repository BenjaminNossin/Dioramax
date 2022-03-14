using System;
using UnityEngine;
using UnityEngine.Events; 

/// <summary>
/// This Class allows to rotate a camera around a crane, that acts as it center of rotation.
/// Put this script on the crane, with a camera as child
/// </summary>
public class CameraRotation : MonoBehaviour
{
    [SerializeField] private GameObject diorama;

    [Space, SerializeField, Range(0.2f, 5f)] private float XYForceMultiplier = 2f;
    [SerializeField, Range(3f, 12f)] private float ZForceMultiplier = 8f;

    [Space, SerializeField, Range(0, 50)] private float rotationSensitivity = 5f;
    public float RotationSensitivity { get; set; }
    private Vector2 rotationAxis;
    private Touch touchTop;

    private bool yxRotation, zRotation;

    UnityAction OnEvaluationEndedCallback;
    private float toucheMoveForceOnEnded;

    public static float ZRotation; 

    [Header("Gamefeel")]
    [SerializeField] CurveEvaluator gamefeelCurve;
    private bool updateGamefeelCurve; 

    // TODO : Subscribing for the gamefeel event should NOT be done here, but on an interface or CurveEvaluator. 
    private void OnEnable()
    {
        Controls.OnTouchStarted += InterruptPreviousCurveOnNewTouch; 
        Controls.OnTouchEnded += TriggerGamefeelCurveOnInputStateChange;
        OnEvaluationEndedCallback += SetToFalse; 
    }

    private void OnDisable()
    {
        Controls.OnTouchStarted -= InterruptPreviousCurveOnNewTouch;
        Controls.OnTouchEnded -= TriggerGamefeelCurveOnInputStateChange;
        OnEvaluationEndedCallback -= SetToFalse;
    }

    private void Start()
    {
        transform.position = diorama.transform.position; // TODO : set dynamically at the start of a level
        RotationSensitivity = rotationSensitivity;
    }

    private float forceDebugFloat; 
    private void Update()
    {
        if (updateGamefeelCurve)
        {
            if (yxRotation)
            {
                //Debug.Log("yx rotation gamefeel"); 
                UpdateXYRotation(rotationDirection, rotationForce * gamefeelCurve.Evaluate(OnEvaluationEndedCallback)); 
            }
            else if (zRotation)
            {
                //Debug.Log("z rotation gamefeel");
                UpdateZRotation(touch0, touch1, rotationForce); 
            }
        }
    }

    Vector3 rotationDirection;
    float rotationForce;
    /// <summary>
    /// Move the parent along X and Y axis. If you want to only rotate the camera frame, use "UpdateZRotation instead
    /// </summary>
    /// <param name="rotationDirection">Direction of camera displacement, based on direction of swipe</param>
    /// <param name="rotationForce">The speed of displacement</param>
    /// <returns>Wether the swipe force is greater than the sensibility settings. Otherwise, it won't rotate</returns>
    public void UpdateXYRotation(Vector3 _rotationDirection, float _rotationForce)
    {
        yxRotation = true;
        zRotation = false;

        rotationDirection = _rotationDirection;
        rotationForce = _rotationForce;

        // to always get an axis that is 90° more than direction
        rotationAxis = new Vector2(-rotationDirection.y, rotationDirection.x); // -y
        transform.Rotate(rotationAxis, Time.deltaTime * XYForceMultiplier * rotationForce);
    }

    Touch touch0, touch1;
    float topPosition;
    /// <summary>
    /// No object moves during this rotation. It is applied to the camera parent, around its Z axis. This is how the physics-based mechanic is triggered
    /// </summary>
    /// <param name="topDirection">The direction of swipe from the top finger, usually the index</param>
    /// <param name="bottomDirection">The direction of swipe from the thumb</param>
    public void UpdateZRotation(Touch _touch0, Touch _touch1, float _rotationForce)
    {
        yxRotation = false;
        zRotation = true;

        topPosition = Mathf.Max(_touch0.position.y, _touch1.position.y);
        touch0 = _touch0;
        touch1 = _touch1;
        rotationForce = _rotationForce; 

        // stupid to do this every frame. But how to keep reference to the arguments each frame ?
        if (topPosition == _touch0.position.y)
        {
            touchTop = _touch0;
        }
        else if (topPosition == _touch1.position.y)
        {
            touchTop = _touch1;
        }

        transform.Rotate(transform.forward, Time.deltaTime * ZForceMultiplier * _rotationForce * MathF.Sign(touchTop.deltaPosition.x));
        ZRotation = transform.rotation.eulerAngles.z; 
    }   

    private void InterruptPreviousCurveOnNewTouch()
    {
        if (gamefeelCurve.EvaluateCurve)
        {
            gamefeelCurve.EndGamefeelCurve(); 
        }
    }

    private void TriggerGamefeelCurveOnInputStateChange(TouchState previous)
    {
        if (previous == TouchState.Rotating)
        {
            updateGamefeelCurve = true; 
        }
    }

    private void SetToFalse()
    {
        Debug.Log("on ended rotation callback"); 
        updateGamefeelCurve = false;
    }
}
    