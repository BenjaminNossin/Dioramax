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
    [Space, SerializeField, Range(0.2f, 5f)] private float XYForceMultiplier = 2f;
    [SerializeField, Range(20f, 100f)] private float ZRotationForce = 50;

    [Space, SerializeField, Range(0, 50)] private float rotationSensitivity = 5f;
    public float RotationSensitivity { get; set; }

    public static bool YXRotation { get; set; }
    public static bool ZRotation_GFCurve { get; set; }

    private UnityAction OnEvaluationEndedCallback;

    public static float ZRotation { get; set; }
    public static float ZLocalRotation { get; set; }
    public static float ZAngleWithIdentityRotation { get; set; }

    [Header("Gamefeel")]
    [SerializeField] CurveEvaluator gamefeelCurve;
    [SerializeField] CurveEvaluator ZRotationGamefeelCurve; 
    [SerializeField, Range(50, 400)] private float maxAllowedSwipeForce = 200f;

    private bool updateGamefeelCurve;
    private float curveValue;
    private float force;
    private bool needToSetForce = true;

    // TODO : Subscribing for the gamefeel event should NOT be done here, but on an interface or CurveEvaluator.
    private void OnEnable()
    {
        Controls.OnTouchStarted += InterruptPreviousCurveOnNewTouch;
        Controls.OnTouchEnded += TriggerGamefeelCurveOnInputStateChange;
        TouchDetection.OnDoubleTapDetection += SetCameraRotationOnDoubleTap;

        ZRotationButton.OnPointerEnter_StopPreviousGamefeelCurve += InterruptPreviousCurveOnNewTouch; 
        ZRotationButton.OnPointerExit_GamefeelCurve += DoZRotationGamefeelCurve;

        OnEvaluationEndedCallback += ResetBoolValues;
    }

    private void OnDisable()
    {
        Controls.OnTouchStarted -= InterruptPreviousCurveOnNewTouch;
        Controls.OnTouchEnded -= TriggerGamefeelCurveOnInputStateChange;
        TouchDetection.OnDoubleTapDetection -= SetCameraRotationOnDoubleTap;

        ZRotationButton.OnPointerEnter_StopPreviousGamefeelCurve -= InterruptPreviousCurveOnNewTouch;
        ZRotationButton.OnPointerExit_GamefeelCurve -= DoZRotationGamefeelCurve; 

        OnEvaluationEndedCallback -= ResetBoolValues;
    }

    private void DoZRotationGamefeelCurve()
    {
        GameLogger.Log("on pointer exit gamefeelcurve");
        updateGamefeelCurve = true;
        ZRotation_GFCurve = true; 
        YXRotation = false; 
    }


    private void Start()
    {
        RotationSensitivity = rotationSensitivity;
        needToSetForce = true;
    }

    private void Update()
    {
        if (updateGamefeelCurve)
        {
            if (YXRotation)
            {
                // GameLogger.Log("yx rotation gamefeel");
                if (needToSetForce)
                {
                    needToSetForce = false;
                    force = Mathf.Clamp(swipeForce / maxAllowedSwipeForce, 0, 1);
                }

                curveValue = gamefeelCurve.DoXYRotationCurve(OnEvaluationEndedCallback, force); 
                UpdateXYRotation(swipeDirection, swipeForce * curveValue);
            }
            else if (ZRotation_GFCurve)
            {
                curveValue = ZRotationForce * ZRotationGamefeelCurve.DoZRotationCurve(OnEvaluationEndedCallback);
                UpdateZRotation();
            }
        }
        else if (ZRotationButton.ButtonIsSelected)
        {
            UpdateZRotation();
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
        /* for (int i = 0; i < Input.touchCount; i++)
        {
            if (PointIsUnderYValue(160, Input.GetTouch(i).position)) return; 
        } */

        if (Input.touchCount == 1 && PointIsUnderYValue(160, Input.GetTouch(0).position)) return;

        YXRotation = true;
        ZRotation_GFCurve = false;

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

    private bool PointIsUnderYValue(float y, Vector2 point) => point.y <= y;
    


    int direction, storedDirection; 
    /// <summary>
    /// No object moves during this rotation. It is applied to the camera parent, around its Z axis. This is how the physics-based mechanic is triggered
    /// </summary>
    /// <param name="topDirection">The direction of swipe from the top finger, usually the index</param>
    /// <param name="bottomDirection">The direction of swipe from the thumb</param>
    public void UpdateZRotation() // increase rotation speed over time (rotationForce = Lerp(min, max, t))
    {
        YXRotation = false;
        ZRotation_GFCurve = true; 

        if (updateGamefeelCurve)
        {
            transform.localEulerAngles += new Vector3(0f, 0f, Time.deltaTime * curveValue * storedDirection);
        }
        else
        {
            direction = ZRotationButton.LeftIsSelected ? -1 : ZRotationButton.RightIsSelected ? 1 : 0;
            storedDirection = direction; 

            transform.localEulerAngles += new Vector3(0f, 0f, Time.deltaTime * ZRotationForce * direction);
        }

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
        else if (ZRotationGamefeelCurve.EvaluateCurve)
        {
            ZRotationGamefeelCurve.EndGamefeelCurve();
        }
    }

    private void TriggerGamefeelCurveOnInputStateChange(TouchState previous)
    {
        if (previous == TouchState.XYRotating || ZRotationButton.ButtonIsSelected) 
        {
            updateGamefeelCurve = true;
        }
    }

    private void ResetBoolValues()
    {
        GameLogger.Log("on ended callback");
        updateGamefeelCurve = false;
        YXRotation = ZRotation_GFCurve = false;
        needToSetForce = true; 
    }
}
