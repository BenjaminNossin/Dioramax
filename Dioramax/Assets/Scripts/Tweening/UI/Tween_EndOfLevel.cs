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
        transform.DOScale(stretch_squash, TimeScale).SetEase(Ease.OutExpo).SetLoop(-1) ;
    }
}
