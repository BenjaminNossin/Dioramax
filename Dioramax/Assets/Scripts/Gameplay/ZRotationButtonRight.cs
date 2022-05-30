using UnityEngine.EventSystems;
using UnityEngine;

public class ZRotationButtonRight : ZRotationButton
{
    [SerializeField] private AudioSource audiosource; 

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (WaitingForNextFrames) return;

        base.OnPointerEnter(eventData);

        audiosource.Play();

        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            ButtonIsSelected = true; 

            RightIsSelected = true;
            LeftIsSelected = false;
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        audiosource.Stop();

        base.OnPointerExit(eventData);

        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            ButtonIsSelected = false;

            RightIsSelected = false;
        }
    }
}
