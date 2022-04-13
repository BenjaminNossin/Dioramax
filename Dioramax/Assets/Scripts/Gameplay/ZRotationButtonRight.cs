using UnityEngine.EventSystems;

public class ZRotationButtonRight : ZRotationButton
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (WaitingForNextFrames) return; 

        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            ButtonIsSelected = true; 

            RightIsSelected = true;
            LeftIsSelected = false;
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            ButtonIsSelected = false;

            RightIsSelected = false;
        }
    }
}
