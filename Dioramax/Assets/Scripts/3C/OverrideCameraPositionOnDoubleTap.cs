using UnityEngine;

public class OverrideCameraPositionOnDoubleTap : MonoBehaviour
{
    [SerializeField] private bool doOverride;
    [SerializeField] private Vector3 doubleTapCameraCraneFocusPosition; // hide this is previous bool is false
    public bool DoOverride { get; private set; }

    private void Awake()
    {
        DoOverride = doOverride;
    }

    public Vector3 GetNewPosition() => doubleTapCameraCraneFocusPosition;
}
