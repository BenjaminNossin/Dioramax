using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tween_EndOfLevel : MonoBehaviour
{

    public float stretch_squash;
    public float TimeScale;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        transform.DOScale(stretch_squash, TimeScale).SetEase(Ease.OutBack).SetLoops(-1, LoopType.Restart) ;
    }
}
