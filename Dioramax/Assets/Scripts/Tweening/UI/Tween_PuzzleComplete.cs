using UnityEngine;
using DG.Tweening;

public class Tween_PuzzleComplete : MonoBehaviour
{
    public float stretch_squash = 1f;
    public float TimeScale = 1f;

    public void PuzzleCompleteTween()
    {
        GameLogger.Log("doing puzzle complete tween");
        transform.DOScale(stretch_squash * 1.5f, TimeScale * 0.5f).SetEase(Ease.OutExpo).OnComplete(() => transform.DOScale(stretch_squash, TimeScale * 0.5f).SetEase(Ease.OutBack));
    }
}
