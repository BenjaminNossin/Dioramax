using UnityEngine;
using DG.Tweening; 

public class Debug_Teleporter : MonoBehaviour
{
    public static System.Action<Transform> OnTeleportCallback { get; set; }

    public void CallTeleport()
    {
        OnTeleportCallback(transform);
        transform.DOScale(1.5f, 0.25f).SetEase(Ease.OutExpo).
            OnComplete(() => transform.DOScale(1, 0.25f).SetEase(Ease.OutBack));
    }
}
