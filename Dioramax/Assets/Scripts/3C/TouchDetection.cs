using UnityEngine;
using UnityEngine.Events;
using System; 

/// <summary>
/// This script is in charge of managing interactable objects state change on tap/hold/drag.
/// It does NOT perform any logic on the objects except state change.
/// </summary>
public class TouchDetection : MonoBehaviour
{
    [SerializeField] private LayerMask buttonMask;
    [SerializeField] private LayerMask carrouselPropMask;

    // [SerializeField] private InteractableEntity placeholderFeedback;

    private bool buttonDetected, carrouselBearDetected; 
    private ButtonProp previousButton, currentButton;
    private CarrouselProp detectedCarrouselProp; 

    private readonly UnityEvent<MeshRenderer[]> OnRequireSharedEvent = new();
    private UnityAction<MeshRenderer[]> OnRequireSharedCallback;

    private MeshRenderer[] currentMeshRendererArray;
    private MeshRenderer[] previousMeshRendererArray;
    private int[] equalityArray;

    public static Action<Vector3> OnDoubleTapDetection { get; set; } 
    public static int CarrouselPropActivated { get; set; }
    public static int ValidCarrouselPropAmount { get; set; }
    private int carrouselPropActivated; // DEBUG
    private bool doneOnce; 

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
        Debug.DrawRay(touchStart, (toucheEnd - touchStart) * 100f, Color.red, 0.5f);
        buttonDetected = Physics.Raycast(touchStart, (toucheEnd - touchStart), out RaycastHit buttonHitInfo, 100f, buttonMask);
        carrouselBearDetected = Physics.Raycast(touchStart, (toucheEnd - touchStart), out RaycastHit bearHitInfo, 100f, carrouselPropMask);

        if (buttonDetected)
        {
            Debug.DrawRay(touchStart, (toucheEnd - touchStart) * 100f, Color.green, 0.5f);

            if (doneOnce)
            {
                previousButton = currentButton;
                ButtonPropsManager.Instance.SetPreviousButtonProp(previousButton);
            }

            currentButton = buttonHitInfo.transform.GetComponent<ButtonProp>();
            ButtonPropsManager.Instance.SetCurrentButtonProp(currentButton);

            if (doubleTap && currentButton.CanOverrideCameraPositionOnDoubleTap())
            {
                Debug.Log("this was a double tap");
                OnDoubleTapDetection(currentButton.GetCameraPositionOverride());
            }

            if (previousButton)
            {
                if (previousButton == currentButton)
                {
                    currentButton.SetButtonState(true);
                }
                else
                {
                    currentButton.SetButtonState();
                    previousButton.SetButtonState(); 
                }
            }
            else
            {
                doneOnce = true; 
                currentButton.SetButtonState(); // only first time
            } 

        } 
        else if (carrouselBearDetected)
        {
            detectedCarrouselProp = bearHitInfo.transform.GetComponent<CarrouselProp>();
            detectedCarrouselProp.SetActiveColor(); 
        }

        return buttonDetected || carrouselBearDetected;
    }

    /* private void SetPlaceholderReference(InteractableEntity current)
    {
        if (!previousButton) return; 

        if (previousButton != current)
        {
            if (currentButton.InteractablesCanBeShared)
            {
                InteractableEntityRemote previousEntity = previousButton as InteractableEntityRemote;
                InteractableEntityRemote currentEntity = currentButton as InteractableEntityRemote;

                previousMeshRendererArray = previousEntity.entitiesMeshRenderers; 
                currentMeshRendererArray = currentEntity.entitiesMeshRenderers; 

                equalityArray = new int[previousMeshRendererArray.Length];
                for (int i = 0; i < equalityArray.Length; i++)
                {
                    equalityArray[i] = 0;
                }

                for (int i = 0; i < previousMeshRendererArray.Length; i++)
                {
                    if (previousMeshRendererArray.Length > currentMeshRendererArray.Length)
                    {
                        for (int j = 0; j < currentMeshRendererArray.Length; j++)
                        {
                            if (i <= j)
                            {
                                equalityArray[i] = currentMeshRendererArray[i] == previousMeshRendererArray[i] ? 1 : 0;
                            }
                        }
                    }
                    else if (previousMeshRendererArray.Length < currentMeshRendererArray.Length)
                    {
                        for (int j = 0; j < currentMeshRendererArray.Length; j++)
                        {
                            if (i <= j)
                            {
                                equalityArray[i] = currentMeshRendererArray[i] == previousMeshRendererArray[i] ? 1 : 0;
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < currentMeshRendererArray.Length; j++)
                        {
                            equalityArray[i] = currentMeshRendererArray[i] == previousMeshRendererArray[i] ? 1 : 0;                           
                        }
                    }
                }

                previousButton.SwapOrChangeBack(false, equalityArray); 
            }
        }
    }  */
}
