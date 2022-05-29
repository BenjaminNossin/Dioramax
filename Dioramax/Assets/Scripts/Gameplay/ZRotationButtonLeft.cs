using UnityEngine;
using UnityEngine.EventSystems;

public class ZRotationButtonLeft : ZRotationButton
{
    [SerializeField] private AudioSource audiosource;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (WaitingForNextFrames) return;

        audiosource.Play();

        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            ButtonIsSelected = true;

            LeftIsSelected = true;
            RightIsSelected = false;
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        audiosource.Stop();

        base.OnPointerExit(eventData);

        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            ButtonIsSelected = false;

            LeftIsSelected = false;
        }
    } 
}
