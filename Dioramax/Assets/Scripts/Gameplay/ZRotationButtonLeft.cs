using UnityEngine;
using UnityEngine.EventSystems;

public class ZRotationButtonLeft : ZRotationButton
{
    // this is called during OnPointerExit too..
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            LeftIsSelected = true;
            RightIsSelected = false;
            Debug.Log($"Enter left. Left is selected : {LeftIsSelected}. Right is selected : {RightIsSelected}.");
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            LeftIsSelected = false;
            Debug.Log($"Exit left. Left is selected : {LeftIsSelected}. Right is selected : {RightIsSelected}.");
        }
    } 
}
