using DG.Tweening;
using UnityEngine;

public class Select_Ours : StoppableTween 
{
    private Vector3 initialPosition, currentPosition; 

    private void Awake()
    {
        initialPosition = transform.position;
    }

    private void OnEnable()
    {
        transform.DOMoveY(0.5f, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        Debug.Log("select ourson on disable");
        currentPosition = transform.position; 
        BackToInitial(); 
    }

    private void BackToInitial()
    {
        transform.DOKill();
        transform.DOMove(initialPosition, Mathf.Abs(initialPosition.y - currentPosition.y));
    }
}
