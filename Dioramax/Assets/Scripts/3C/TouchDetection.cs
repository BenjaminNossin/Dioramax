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
    [SerializeField] private LayerMask tweenableMask;
    [SerializeField] private LayerMask finishMask;


    private bool buttonDetected, carrouselBearDetected, tweenableDetected, finishMaskDetected;   
    private static ButtonProp DetectedButtonProp;
    private CarrouselProp detectedCarrouselProp; 

    private readonly UnityEvent<MeshRenderer[]> OnRequireSharedEvent = new();
    private UnityAction<MeshRenderer[]> OnRequireSharedCallback;

    public static Action<Vector3> OnDoubleTapDetection { get; set; } 
    public static int CarrouselPropActivated { get; set; }
    public static int ValidCarrouselPropAmount { get; set; }
    private int carrouselPropActivated; // DEBUG
    private bool canCast = true;
    private const float RAY_LENGTH = 40f;
    private const float RAY_DEBUG_DURATION = 0.5f;


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

        Debug.DrawRay(touchStart, (toucheEnd - touchStart) * RAY_LENGTH, Color.red, RAY_DEBUG_DURATION);
        buttonDetected = Physics.Raycast(touchStart, (toucheEnd - touchStart), out RaycastHit buttonHitInfo, RAY_LENGTH, buttonMask);
        carrouselBearDetected = Physics.Raycast(touchStart, (toucheEnd - touchStart), out RaycastHit bearHitInfo, RAY_LENGTH, carrouselPropMask);
        tweenableDetected = Physics.Raycast(touchStart, (toucheEnd - touchStart), out RaycastHit tweenableHitInfo, RAY_LENGTH, tweenableMask);
        finishMaskDetected = Physics.Raycast(touchStart, (toucheEnd - touchStart), out RaycastHit finishHitInto, RAY_LENGTH, finishMask); 

        if (tweenableDetected)
        {
            print("Tweening");
            Debug.DrawRay(touchStart, (toucheEnd - touchStart) * RAY_LENGTH, Color.green, RAY_DEBUG_DURATION);
            tweenableHitInfo.transform.GetComponent<TweenTouch>().Tween();
        }

        if (finishMaskDetected && LevelManager.LevelIsFinished)
        {
            Debug.Log("Show Finish UI"); 
        }

        if (buttonDetected)
        {
            Debug.DrawRay(touchStart, (toucheEnd - touchStart) * RAY_LENGTH, Color.green, RAY_DEBUG_DURATION);
            StartCoroutine(CanCast()); 

            DetectedButtonProp = buttonHitInfo.transform.GetComponent<ButtonProp>();
            ButtonPropsManager.Instance.SetCurrentButtonProp(DetectedButtonProp);

            if (doubleTap && DetectedButtonProp.CanOverrideCameraPositionOnDoubleTap())
            {
                Debug.Log("this was a double tap");
                OnDoubleTapDetection(DetectedButtonProp.GetCameraPositionOverride());
            }
        } 
        else if (carrouselBearDetected)
        {
            detectedCarrouselProp = bearHitInfo.transform.GetComponent<CarrouselProp>();
            detectedCarrouselProp.SetActiveColor(); 
        }

        return buttonDetected || carrouselBearDetected;
    }

    System.Collections.IEnumerator CanCast()
    {
        canCast = false; 
        yield return new WaitForSeconds(0.5f);

        canCast = true; 
    }
}
