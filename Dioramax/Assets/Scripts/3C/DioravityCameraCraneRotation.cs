using System;
using UnityEngine;
using UnityEngine.Events;

// REFACTORING : separate rotation from gamefeel 

/// <summary>
/// This Class allows to rotate a camera around a crane, that acts as it center of rotation.
/// Put this script on the crane, with a camera as child
/// </summary>
public class DioravityCameraCraneRotation : MonoBehaviour
{
    [SerializeField] private GameObject diorama;

    [Space, SerializeField, Range(0.2f, 5f)] private float XYForceMultiplier = 2f;
    [SerializeField, Range(1f, 50f)] private float ZRotationForce = 30;

    [Space, SerializeField, Range(0, 50)] private float rotationSensitivity = 5f;
    public float RotationSensitivity { get; set; }

    private bool yxRotation;

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


    private void Start()
    {
        RotationSensitivity = rotationSensitivity;
    }

    private void Update()
    {
        if (updateGamefeelCurve)
        {
            if (yxRotation)
            {
                // Debug.Log("yx rotation gamefeel");
                UpdateXYRotation(swipeDirection, swipeForce * gamefeelCurve.Evaluate(OnEvaluationEndedCallback));
            }
        }

        // diorama keeps rotating for some frames even after button up. Check for some calls that are performance heavy
        if (ZRotationButton.ButtonIsSelected)
        {
            UpdateZRotation(); //  * gamefeelCurve.Evaluate(OnEvaluationEndedCallback));
        }
    }

    Vector3 swipeDirection;
    float swipeForce;   
    /// <summary>
    /// Move the parent along X and Y axis. If you want to only rotate the camera frame, use "UpdateZRotation instead
    /// </summary>
    /// <param name="rotationDirection">Direction of camera displacement, based on direction of swipe</param>
    /// <param name="rotationForce">The speed of displacement</param>
    public void UpdateXYRotation(Vector3 _swipeDirection, float _swipeForce)
    {
        // not very opti. Find bottom touch once and then just check him
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (PointIsInsideRectangle(380, 35, 780, 160, Input.GetTouch(i).position)) return; 
        } 

        yxRotation = true;

        swipeDirection = _swipeDirection;
        swipeForce = _swipeForce;

        // rotate LOCAL based on WORLD direction of swipe
        transform.Rotate(new Vector2(-swipeDirection.y, swipeDirection.x), 
                         Time.fixedDeltaTime * swipeForce * XYForceMultiplier, 
                         Space.Self); 
    }

    private bool PointIsInsideRectangle(int xMin, int yMin, int xMax, int yMax, Vector2 point)
    {
        return point.x > xMin && point.x < xMax && point.y > yMin && point.y < yMax;
    }


    int direction;
    /// <summary>
    /// No object moves during this rotation. It is applied to the camera parent, around its Z axis. This is how the physics-based mechanic is triggered
    /// </summary>
    /// <param name="topDirection">The direction of swipe from the top finger, usually the index</param>
    /// <param name="bottomDirection">The direction of swipe from the thumb</param>
    public void UpdateZRotation() // increase rotation speed over time (rotationForce = Lerp(min, max, t))
    {
        yxRotation = false;
        direction = ZRotationButton.LeftIsSelected ? -1 : ZRotationButton.RightIsSelected ? 1 : 0; 

        transform.localEulerAngles += new Vector3(0f, 0f, Time.deltaTime * ZRotationForce * direction);

        // I just want two values : 0 to 360 (tuyaux) and 0 to 180/0 to -180 (train)
        ZLocalRotation = transform.localEulerAngles.z; 
        ZRotation = transform.eulerAngles.z;
        ZAngleWithIdentityRotation = ZLocalRotation > 180f ?
                             ZLocalRotation - 360f:
                             ZLocalRotation; 
    }

    // TODO : smooth lerp 
    private void SetCameraRotationOnDoubleTap(Vector3 newCameraRotation)
    {
        transform.rotation = Quaternion.Euler(newCameraRotation);
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
}
