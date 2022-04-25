using UnityEngine;
using UnityEngine.EventSystems;

public class ZRotationButtonLeft : ZRotationButton
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (WaitingForNextFrames) return;

        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            ButtonIsSelected = true;

            LeftIsSelected = true;
            RightIsSelected = false;
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            ButtonIsSelected = false;

            LeftIsSelected = false;
        }
    } 
}
