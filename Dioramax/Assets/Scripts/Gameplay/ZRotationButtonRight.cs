using UnityEngine;
using UnityEngine.EventSystems;

public class ZRotationButtonRight : ZRotationButton
{
    // this is called during OnPointerExit too..
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            RightIsSelected = true;
            LeftIsSelected = false;
            Debug.Log($"Enter right. Right is selected : {RightIsSelected}. Left is selected : {LeftIsSelected}.");
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            RightIsSelected = false;
            Debug.Log($"Exit right. Right is selected : {RightIsSelected}. Left is selected : {LeftIsSelected}.");
        }
    }
}
