using UnityEngine.EventSystems;
using UnityEngine;

public abstract class ZRotationButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool LeftIsSelected { get; protected set; }
    public static bool RightIsSelected { get; protected set; }


    public abstract void OnPointerEnter(PointerEventData eventData);

    public abstract void OnPointerExit(PointerEventData eventData);
}
