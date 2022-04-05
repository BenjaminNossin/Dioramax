using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This Class allows to rotate a camera around a crane, that acts as it center of rotation.
/// Put this script on the crane, with a camera as child
/// </summary>
public class DioravityCameraCraneRotation : MonoBehaviour
{
    [SerializeField] private GameObject diorama;
    [SerializeField] private Transform rotationAxis;

    [SerializeField] private Transform cameraTransform;

    [Space, SerializeField, Range(0.2f, 5f)] private float XYForceMultiplier = 2f;
    [SerializeField, Range(3f, 12f)] private float ZForceMultiplier = 8f;

    [Space, SerializeField, Range(0, 50)] private float rotationSensitivity = 5f;
    public float RotationSensitivity { get; set; }
    private Touch touchTop;

    private bool yxRotation, zRotation;

    UnityAction OnEvaluationEndedCallback;

    public static float ZRotation { get; set; }
    public static float ZLocalRotation { get; set; }
    public static float ZAngleWithIdentityRotation { get; set; }

    [Header("Gamefeel")]
    [SerializeField] CurveEvaluator gamefeelCurve;
    private bool updateGamefeelCurve;

    // TODO : Subscribing for the gamefeel event should NOT be done here, but on an interface or CurveEvaluator.
    private void OnEnable()
    {
        Controls.OnTouchStarted += InterruptPreviousCurveOnNewTouch;
        Controls.OnTouchEnded += TriggerGamefeelCurveOnInputStateChange;
        TouchDetection.OnDoubleTapDetection += SetCameraRotationOnDoubleTap;

        OnEvaluationEndedCallback += SetToFalse;
    }

    private void OnDisable()
    {
        Controls.OnTouchStarted -= InterruptPreviousCurveOnNewTouch;
        Controls.OnTouchEnded -= TriggerGamefeelCurveOnInputStateChange;
        TouchDetection.OnDoubleTapDetection -= SetCameraRotationOnDoubleTap;

        OnEvaluationEndedCallback -= SetToFalse;
    }

    private float[,] twoByTwoMatrix;
    private void Start()
    {
        transform.position = diorama.transform.position; // TODO : set dynamically at the start of a level
        RotationSensitivity = rotationSensitivity;
    }

    private void Update()
    {
        twoByTwoMatrix = new float[,] { { Mathf.Cos(ZRotation), -Mathf.Sin(ZRotation) }, { Mathf.Sin(ZRotation), Mathf.Cos(ZRotation) } };

        if (updateGamefeelCurve)
        {
            if (yxRotation)
            {
                // Debug.Log("yx rotation gamefeel");
                UpdateXYRotation(swipeDirection, swipeForce * gamefeelCurve.Evaluate(OnEvaluationEndedCallback));
            }

            if (zRotation)
            {
                // Debug.Log("z rotation gamefeel");
                UpdateZRotation(touch0, touch1, swipeForce * gamefeelCurve.Evaluate(OnEvaluationEndedCallback));
            }
        }
    }

    Vector3 swipeDirection, invertedSwipeDirection, finalRotationAxis; 
    float swipeForce;
    /// <summary>
    /// Move the parent along X and Y axis. If you want to only rotate the camera frame, use "UpdateZRotation instead
    /// </summary>
    /// <param name="rotationDirection">Direction of camera displacement, based on direction of swipe</param>
    /// <param name="rotationForce">The speed of displacement</param>
    public void UpdateXYRotation(Vector3 _swipeDirection, float _swipeForce)
    {
        yxRotation = true;
        zRotation = false;

        swipeDirection = _swipeDirection;
        swipeForce = _swipeForce;

        // to always get an axis that is 90° more than direction
        // rotationAxis = new Vector2(-rotationDirection.y, rotationDirection.x); 
        /* rotationAxis = new Vector3((rotationDirection.x * twoByTwoMatrix[1, 0]) - (rotationDirection.y * twoByTwoMatrix[1, 1]),
                                   (rotationDirection.x * twoByTwoMatrix[0, 0]) + (rotationDirection.y * twoByTwoMatrix[0, 1])); */

        // debugObj.transform.up = rotationAxis;

        // transform.eulerAngles =  rotationForce * Time.deltaTime * XYForceMultiplier * new Vector3(-rotationDirection.y, rotationDirection.x, 0f);
        // transform.Rotate(rotationAxis, Time.deltaTime * XYForceMultiplier * rotationForce);

        // use swipe direction for default rotation axis
        Vector3 defaultRotationAxis = new Vector2(swipeDirection.y, swipeDirection.x);

        // further rotate that axis based on your camera Z angle
        finalRotationAxis = Quaternion.AngleAxis(ZRotation, defaultRotationAxis) * finalRotationAxis;

        // rotate aroudn that axis
        transform.Rotate(new Vector2(finalRotationAxis.y, finalRotationAxis.x), 
                         Time.fixedDeltaTime * swipeForce * XYForceMultiplier, 
                         Space.Self);
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
        swipeForce = _rotationForce;

        // stupid to do this every frame. But how to keep reference to the arguments each frame ?
        if (topPosition == _touch0.position.y)
        {
            touchTop = _touch0;
        }
        else if (topPosition == _touch1.position.y)
        {
            touchTop = _touch1;
        }

        // cameraTransform.Rotate(cameraTransform.forward, Time.deltaTime * ZForceMultiplier * _rotationForce * MathF.Sign(touchTop.deltaPosition.x));

        cameraTransform.localEulerAngles += new Vector3(0f, 0f, Time.deltaTime * ZForceMultiplier * _rotationForce * MathF.Sign(touchTop.deltaPosition.x));
        ZLocalRotation = cameraTransform.localEulerAngles.z; // UNTESTED MAJ 04.04.2022
        ZRotation = cameraTransform.eulerAngles.z;
        ZAngleWithIdentityRotation = ZLocalRotation > 180f ?
                             360f - ZLocalRotation :
                             ZLocalRotation;
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
        if (previous == TouchState.XYRotating) // OR zoom 
        {
            updateGamefeelCurve = true;
        }
    }

    private void SetToFalse()
    {
        // Debug.Log("on ended rotation callback");
        updateGamefeelCurve = false;
    }

    // do a smooth lerp 
    private void SetCameraRotationOnDoubleTap(Vector3 newCameraRotation)
    {
        transform.rotation = Quaternion.Euler(newCameraRotation); 
    }
}
