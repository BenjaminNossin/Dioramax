using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TweenTouch : MonoBehaviour
{
    public TweeningData td;

    // TempPosition
    private Vector3 ObjectJumpPosition;
    private float ObjectMaxHeight;
    private float ObjectInitialHeight;
    private Vector3 originalScale;
    private ParticleSystem VFX;
    private float TimeScale;
    private float TimeBounce;
    public void Start()
    {
        VFX = GetComponentInChildren<ParticleSystem>();

        ObjectInitialHeight = transform.position.y;
        ObjectMaxHeight = transform.position.y + td.up_max_position;
        originalScale = transform.localScale;
        ObjectJumpPosition = new Vector3(transform.position.x, ObjectMaxHeight, transform.position.z);
        TimeScale = td.time_scale * 0.3f;
        TimeBounce = td.time_bounce * 0.3f;

    }

    public void ScaleGood()
    {
        //retour
        LeanTween.scale(gameObject, originalScale, TimeScale + (TimeScale/2)).setEaseOutBack();
    }

    public void Tween2()
    {  //retour
        LeanTween.moveY(gameObject, ObjectInitialHeight, TimeBounce + (TimeBounce / 2)).setEaseOutBounce();

    }
    
    public void Tween()
    {

        // ! remove looping

        //bounce (aller)
        LeanTween.moveY(gameObject, ObjectMaxHeight, TimeBounce).setEaseOutExpo().setOnComplete(Tween2);
        Debug.Log("Prout");

        //stretch&squash (aller)
        LeanTween.scale(gameObject, td.stretch_squash, TimeScale).setEaseOutExpo().setOnComplete(ScaleGood);//.setLoopCount(-1);

        //rotation

        if (td.EaseOutCubic)
            LeanTween.rotateAround(gameObject, td.RotationAxis, td.rotation_degrees, td.time_rotation).setEaseOutCubic();//.setLoopCount(-1);

        if (td.Punch)
            LeanTween.rotateAround(gameObject, td.RotationAxis, td.rotation_degrees, td.time_rotation).setEasePunch();//.setLoopCount(-1);
        
        //particlesystem
        VFX.Play();
    }

}
