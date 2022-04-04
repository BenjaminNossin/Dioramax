using UnityEngine;
using UnityEngine.Events;
using System; 

/// <summary>
/// This script is in charge of managing interactable objects state change on tap/hold/drag.
/// It does NOT perform any logic on the objects except state change.
/// </summary>
public class TouchDetection : MonoBehaviour
{
    [SerializeField] private LayerMask interactableMask;
    // [SerializeField] private InteractableEntity placeholderFeedback;

    private Camera mainCam;
    private bool objectDetected;
    private static InteractableEntity previousTouched, currentTouched;

    private readonly UnityEvent<MeshRenderer[]> OnRequireSharedEvent = new();
    private UnityAction<MeshRenderer[]> OnRequireSharedCallback;

    private MeshRenderer[] currentMeshRendererArray;
    private MeshRenderer[] previousMeshRendererArray;
    private int[] equalityArray;

    public static Action<Vector3> OnDoubleTapDetection { get; set; } 

    void Start()
    {
        mainCam = Camera.main;
        OnRequireSharedEvent.AddListener(OnRequireSharedCallback); 
    }

    public bool TryCastToTarget(Vector3 touchStart, Vector3 toucheEnd, bool doubleTap)
    {
        Debug.DrawRay(touchStart, (toucheEnd - touchStart) * 100f, Color.red, 0.5f);
        objectDetected = Physics.Raycast(touchStart, (toucheEnd - touchStart), out RaycastHit hitInfo, 100f, interactableMask); 
        // use hit info

        if (objectDetected)
        {
            Debug.DrawRay(touchStart, (toucheEnd - touchStart) * 100f, Color.green, 0.5f);
            currentTouched = hitInfo.transform.GetComponent<InteractableEntity>();

            if (doubleTap && currentTouched.OverrideCameraPositionOnDoubleTap)
            {
                Debug.Log("this was a double tap");
                OnDoubleTapDetection(currentTouched.GetCameraCraneFocusPosition());
            }

            if (previousTouched)
            {
                if (previousTouched == currentTouched)
                {
                    previousTouched.SwapOrChangeBack(true); // swap
                }
                else
                {
                    currentTouched.ChangeColor();
                }
            }
            else
            {
                currentTouched.ChangeColor(); // will enter here only once
            }

            SetPlaceholderReference(currentTouched);
            previousTouched = currentTouched;
        }

        return objectDetected;
    }

    private void SetPlaceholderReference(InteractableEntity current)
    {
        if (!previousTouched) return; 

        if (previousTouched != current)
        {
            if (currentTouched.InteractablesCanBeShared)
            {
                InteractableEntityRemote previousEntity = previousTouched as InteractableEntityRemote;
                InteractableEntityRemote currentEntity = currentTouched as InteractableEntityRemote;

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

                previousTouched.SwapOrChangeBack(false, equalityArray); // change back
            }
        }
    }
}
