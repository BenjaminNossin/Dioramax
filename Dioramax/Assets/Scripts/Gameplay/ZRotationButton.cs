using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections; 

public abstract class ZRotationButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool ButtonIsSelected { get; protected set; }
    public static bool LeftIsSelected { get; protected set; }
    public static bool RightIsSelected { get; protected set; }
    public static bool WaitingForNextFrames { get; private set; } // because somehow Unity calls OnPointerEnter again after OnPOinterExit..

    public static System.Action OnPointerExit_GamefeelCurve { get; set; }
    public static System.Action OnPointerEnter_StopPreviousGamefeelCurve { get; set; }

    private void Awake()
    {
        ButtonIsSelected = false;
        LeftIsSelected = false;
        RightIsSelected = false;
        WaitingForNextFrames = false; 
    }

    private void Update()
    {
        // because of logic errors when sliding super quickly from a button to the other
        if (WaitingForNextFrames || Input.touchCount == 0)
        {
            ButtonIsSelected = false; 
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnter_StopPreviousGamefeelCurve(); 
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExit_GamefeelCurve();
        StartCoroutine(Wait()); 
    }

    private readonly WaitForSeconds WFS = new(0.05f); 
    private IEnumerator Wait()
    {
        WaitingForNextFrames = true;
        yield return WFS; 

        WaitingForNextFrames = false; 
    }
}
