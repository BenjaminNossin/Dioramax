using UnityEngine;
using UnityEngine.Events;
using System; 

/// <summary>
/// This script is in charge of managing interactable objects state change on tap/hold/drag.
/// It does NOT perform any logic on the objects except state change.
/// </summary>
public class TouchDetection : MonoBehaviour
{
    // Test screen distorsion effect 
    //[SerializeField] private ParticleSystem TouchDistorsion;
    //

    [SerializeField] private LayerMask buttonMask;
    [SerializeField] private LayerMask carrouselPropMask;
    [SerializeField] private LayerMask tweenableTouchMask;
    [SerializeField] private LayerMask tweenableOursonMask;
    [SerializeField] private LayerMask ratMask;
    [SerializeField] private LayerMask finishMask;


    private bool buttonDetected, carrouselBearDetected, tweenableTouchDetected, tweenableOursonDetected, finishMaskDetected, ratMaskDetected; // :D    
    private static ButtonProp DetectedButtonProp;
    private CarrouselProp detectedCarrouselProp; 

    private readonly UnityEvent<MeshRenderer[]> OnRequireSharedEvent = new();
    private UnityAction<MeshRenderer[]> OnRequireSharedCallback;

    public static Action<Vector3> OnDoubleTapDetection { get; set; } 
    public static int CarrouselPropActivated { get; set; }
    public static int ValidCarrouselPropAmount { get; set; }
    private int carrouselPropActivated; // DEBUG
    private bool canCast = true;
    private const float CAST_LENGTH = 250f;
    private const float RAY_DEBUG_DURATION = 0.5f;
    private const float CAST_RADIUS = 0.3f;

    void Start()
    {
        CarrouselPropActivated = 0; 
        OnRequireSharedEvent.AddListener(OnRequireSharedCallback); 
    }

    private void Update()
    {
        carrouselPropActivated = CarrouselPropActivated;
    }

    public bool TryCastToTarget(Vector3 touchStart, Vector3 toucheEnd, bool doubleTap)
    {
        if (!canCast) return false; // PLACEHOLDER until done via FixedUpdated and not LateUpdate

        GameDrawDebugger.DrawRay(touchStart, (toucheEnd - touchStart) * CAST_LENGTH, Color.red, RAY_DEBUG_DURATION);

        // use Physics.RaycastAll instead to see if the object detected is the first or hidden behind others
        // NEED REFACTORING too much raycasts
        buttonDetected = Physics.SphereCast(touchStart, CAST_RADIUS, (toucheEnd - touchStart), out RaycastHit buttonHitInfo, CAST_LENGTH, buttonMask);
        carrouselBearDetected = Physics.SphereCast(touchStart, CAST_RADIUS, (toucheEnd - touchStart), out RaycastHit bearHitInfo, CAST_LENGTH, carrouselPropMask);
        tweenableTouchDetected = Physics.SphereCast(touchStart, CAST_RADIUS, (toucheEnd - touchStart), out RaycastHit tweenableTouchHitInfo, CAST_LENGTH, tweenableTouchMask);
        tweenableOursonDetected = Physics.SphereCast(touchStart, CAST_RADIUS, (toucheEnd - touchStart), out RaycastHit tweenableOursonHitInfo, CAST_LENGTH, tweenableOursonMask);
        ratMaskDetected = Physics.SphereCast(touchStart, CAST_RADIUS, (toucheEnd - touchStart), out RaycastHit ratHitInfo, CAST_LENGTH, ratMask);
        finishMaskDetected = Physics.SphereCast(touchStart, CAST_RADIUS, (toucheEnd - touchStart), out RaycastHit finishHitInto, CAST_LENGTH, finishMask);

        if (finishMaskDetected && LevelManager.LevelIsFinished)
        {
            // show victory UI
            EndOfLevelUI.Instance.ShowEndOfLevelPanel();
            LevelManager.Instance.DeactivateZRotationUIOnLevelEnd();
        }

        if (tweenableTouchDetected)
        {
            GameLogger.Log("Touch Tween");
            GameDrawDebugger.DrawRay(touchStart, (toucheEnd - touchStart) * CAST_LENGTH, Color.green, RAY_DEBUG_DURATION);
            tweenableTouchHitInfo.transform.GetComponent<TweenTouch>().Tween();
        }

        if (LevelManager.IsPhase3)
        {
            if (tweenableOursonDetected)
            {
                GameLogger.Log("Ourson Tween");
                GameDrawDebugger.DrawRay(touchStart, (toucheEnd - touchStart) * CAST_LENGTH, Color.green, RAY_DEBUG_DURATION);
                tweenableOursonHitInfo.transform.GetComponent<Select_Ours>().enabled = true;
            }

            if (carrouselBearDetected)
            {
                GameLogger.Log("carrousel bear detected");
                detectedCarrouselProp = bearHitInfo.transform.GetComponent<CarrouselProp>();
                detectedCarrouselProp.SetActiveColor();
            }
        }

        // check that rat have been hit through the vent, by looking at them
        // STILL WIP
        // use list to avoid GetComponent all the time, and update it if the component is a new reference
        if (ratMaskDetected) 
        {
            ratHitInfo.transform.GetComponent<FreezeStateController>().InvertFreezeState(); 
        }

        if (buttonDetected)
        {
            GameDrawDebugger.DrawRay(touchStart, (toucheEnd - touchStart) * CAST_LENGTH, Color.green, RAY_DEBUG_DURATION);
            StartCoroutine(CanCast()); 

            DetectedButtonProp = buttonHitInfo.transform.GetComponent<ButtonProp>();
            ButtonPropsManager.Instance.SetCurrentButtonProp(DetectedButtonProp);

            if (doubleTap && DetectedButtonProp.CanOverrideCameraPositionOnDoubleTap())
            {
                GameLogger.Log("this was a double tap");
                OnDoubleTapDetection(DetectedButtonProp.GetCameraPositionOverride());
            }
        }
       
        return buttonDetected || carrouselBearDetected;
    }

    System.Collections.IEnumerator CanCast()
    {
        canCast = false; 
        yield return new WaitForSeconds(0.2f);

        canCast = true; 
    }
}
