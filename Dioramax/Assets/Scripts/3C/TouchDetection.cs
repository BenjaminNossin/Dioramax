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
    private const float RAY_LENGTH = 250f;
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

        // use Physics.RaycastAll instead to see if the object detected is the first or hidden behind others
        // NEED REFACTORING too much raycasts
        buttonDetected = Physics.Raycast(touchStart, (toucheEnd - touchStart), out RaycastHit buttonHitInfo, RAY_LENGTH, buttonMask);
        carrouselBearDetected = Physics.Raycast(touchStart, (toucheEnd - touchStart), out RaycastHit bearHitInfo, RAY_LENGTH, carrouselPropMask);
        tweenableTouchDetected = Physics.Raycast(touchStart, (toucheEnd - touchStart), out RaycastHit tweenableTouchHitInfo, RAY_LENGTH, tweenableTouchMask);
        tweenableOursonDetected = Physics.Raycast(touchStart, (toucheEnd - touchStart), out RaycastHit tweenableOursonHitInfo, RAY_LENGTH, tweenableOursonMask);
        ratMaskDetected = Physics.Raycast(touchStart, (toucheEnd - touchStart), out RaycastHit ratHitInfo, RAY_LENGTH, ratMask);
        finishMaskDetected = Physics.Raycast(touchStart, (toucheEnd - touchStart), out RaycastHit finishHitInto, RAY_LENGTH, finishMask);

        if (finishMaskDetected && LevelManager.LevelIsFinished)
        {
            // show victory UI
            EndOfLevelUI.Instance.ShowEndOfLevelPanel();
        }

        if (tweenableTouchDetected)
        {
            print("Touch Tween");
            Debug.DrawRay(touchStart, (toucheEnd - touchStart) * RAY_LENGTH, Color.green, RAY_DEBUG_DURATION);
            tweenableTouchHitInfo.transform.GetComponent<TweenTouch>().Tween();
        }

        if (LevelManager.IsPhase3)
        {
            if (tweenableOursonDetected)
            {
                print("Ourson Tween");
                Debug.DrawRay(touchStart, (toucheEnd - touchStart) * RAY_LENGTH, Color.green, RAY_DEBUG_DURATION);
                tweenableOursonHitInfo.transform.GetComponent<Select_Ours>().enabled = true;
            }

            if (carrouselBearDetected)
            {
                print("carrousel bear detected");
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
       
        return buttonDetected || carrouselBearDetected;
    }

    System.Collections.IEnumerator CanCast()
    {
        canCast = false; 
        yield return new WaitForSeconds(0.2f);

        canCast = true; 
    }
}
