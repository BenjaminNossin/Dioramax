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
    [SerializeField] private DioramaName dioramaName; 

    [Header("General")]
    [SerializeField] private LayerMask tweenableTouchMask;
    [SerializeField] private LayerMask finishMask;

    [Header("Tutorial")]
    [SerializeField] private LayerMask tutorialButtonMask; 

    [Header("Diorama 1")]
    [SerializeField] private LayerMask tuyauMask; 
    [SerializeField] private LayerMask carrouselPropMask;
    [SerializeField] private LayerMask ratMask;
    public static Action<Collider> OnTuyauDetected { get; set; }

    [Header("Diorama 2")]
    [SerializeField] private LayerMask switchMask;
    // fire (draggable)

    private bool tuyauDetected, carrouselBearDetected, tweenableTouchDetected, finishMaskDetected, ratMaskDetected,
        switchDetected, tutorialButtonDetected; // :D    
    private CarrouselProp detectedCarrouselProp; 

    private readonly UnityEvent<MeshRenderer[]> OnRequireSharedEvent = new();
    private UnityAction<MeshRenderer[]> OnRequireSharedCallback;

    public static Action OnTutorialButtonDetection { get; set; }

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

    private Collider detectedCollider = null; 
    public void TryCastToTarget(Vector3 touchStart, Vector3 toucheEnd)
    {
        if (!canCast) return; // PLACEHOLDER until done via FixedUpdated and not LateUpdate

        GameDrawDebugger.DrawRay(touchStart, (toucheEnd - touchStart) * CAST_LENGTH, Color.red, RAY_DEBUG_DURATION);

        // use Physics.RaycastAll instead to see if the object detected is the first or hidden behind others
        // NEED REFACTORING too much raycasts
        #region General Casts
        finishMaskDetected = Physics.SphereCast(touchStart, CAST_RADIUS, (toucheEnd - touchStart), out RaycastHit finishHitInto, CAST_LENGTH, finishMask);
        tweenableTouchDetected = Physics.SphereCast(touchStart, CAST_RADIUS, (toucheEnd - touchStart), out RaycastHit tweenableTouchHitInfo, CAST_LENGTH, tweenableTouchMask);
        ratMaskDetected = Physics.SphereCast(touchStart, CAST_RADIUS, (toucheEnd - touchStart), out RaycastHit ratHitInfo, CAST_LENGTH, ratMask);

        if (finishMaskDetected && LevelManager.LevelIsFinished)
        {
            // show victory UI
            EndOfLevelUI.Instance.ShowEndOfLevelPanel();
            LevelManager.Instance.DeactivateObjectsOnLevelEnd();
        }

        if (tweenableTouchDetected)
        {
            GameLogger.Log("Touch Tween");
            GameDrawDebugger.DrawRay(touchStart, (toucheEnd - touchStart) * CAST_LENGTH, Color.green, RAY_DEBUG_DURATION);

            if (tweenableTouchHitInfo.collider.GetComponent<TweenTouch>() != null)
            {
                tweenableTouchHitInfo.collider.GetComponent<TweenTouch>().Tween();
            }
            
            // test Children GO tween
            foreach (Transform child in tweenableTouchHitInfo.transform)
            {
                if (child != null && child.GetComponent<TweenTouch>() != null && child.CompareTag("TweenChild"))
                {
                    GameLogger.Log("tweening of " + transform.name + " is activated");
                    child.GetComponent<TweenTouch>().Tween();  
                }
            }
            // end test
        }

        if (ratMaskDetected)
        {
            detectedCollider = ratHitInfo.collider;
            detectedCollider.GetComponent<FreezeStateController>().InvertFreezeState();
            detectedCollider.GetComponent<TweenTouch>().Tween();
        }
        #endregion

        if (dioramaName == DioramaName.Tutorial)
        {
            #region Tutorial Casts
            tutorialButtonDetected = Physics.SphereCast(touchStart, CAST_RADIUS, (toucheEnd - touchStart), out RaycastHit tutorialButtonHitInfo, CAST_LENGTH, tutorialButtonMask);

            if (tutorialButtonDetected)
            {
                GameDrawDebugger.DrawRay(touchStart, (toucheEnd - touchStart) * CAST_LENGTH, Color.green, RAY_DEBUG_DURATION);
                tutorialButtonHitInfo.collider.GetComponent<TweenTouch>().Tween();
                OnTutorialButtonDetection();
            }
            #endregion
        }
        else if (dioramaName == DioramaName.Diorama1)
        {
            #region Diorama1 Casts
            tuyauDetected = Physics.SphereCast(touchStart, CAST_RADIUS, (toucheEnd - touchStart), out RaycastHit tuyauHitInfo, CAST_LENGTH, tuyauMask);
            carrouselBearDetected = Physics.SphereCast(touchStart, CAST_RADIUS, (toucheEnd - touchStart), out RaycastHit bearHitInfo, CAST_LENGTH, carrouselPropMask);

            if (LevelManager.IsPhase3)
            {
                if (carrouselBearDetected)
                {
                    GameLogger.Log("carrousel bear detected");
                    detectedCollider = bearHitInfo.collider;

                    detectedCollider.GetComponent<CarrouselProp>().SetActiveColor();
                    detectedCollider.GetComponent<Select_Ours>().enabled = true;
                }
            }

            if (tuyauDetected)
            {
                GameDrawDebugger.DrawRay(touchStart, (toucheEnd - touchStart) * CAST_LENGTH, Color.green, RAY_DEBUG_DURATION);
                StartCoroutine(CanCast());

                detectedCollider = tuyauHitInfo.collider;

                detectedCollider.GetComponent<TweenTouch>().Tween();
                OnTuyauDetected(detectedCollider); 
            }
            #endregion
        }
        else
        {
            #region Diorama2 Casts
            switchDetected = Physics.SphereCast(touchStart, CAST_RADIUS, (toucheEnd - touchStart), out RaycastHit switchHitInfo, CAST_LENGTH, switchMask);

            if (switchDetected)
            {
                GameDrawDebugger.DrawRay(touchStart, (toucheEnd - touchStart) * CAST_LENGTH, Color.green, RAY_DEBUG_DURATION);
                switchHitInfo.collider.GetComponent<Switcher>().InvertBoolAndDoSwitch(); 
            }
            #endregion
        }       
    }

    private readonly WaitForSeconds WFS = new (0.2f); 
    System.Collections.IEnumerator CanCast()
    {
        canCast = false; 
        yield return WFS;

        canCast = true; 
    }
}
