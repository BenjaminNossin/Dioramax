using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TweenTouch : MonoBehaviour
{
    public TweeningData td;

    //public AnimationCurve animationCurve;

    void Start()
    {

        // ! remove looping


         //bounce
        LeanTween.moveLocalY(gameObject, td.up_max_position, td.time_bounce).setEasePunch().setLoopCount(-1);


        //stretch&squash
        LeanTween.scale(gameObject, td.stretch_squash, 1f).setEasePunch().setLoopCount(-1);

        //rotation

        if (td.EaseOutCubic)
            LeanTween.rotateAround(gameObject, td.RotationAxis, td.rotation_degrees, td.time_rotation).setEaseOutCubic().setLoopCount(-1);

        if (td.Punch)
            LeanTween.rotateAround(gameObject, td.RotationAxis, td.rotation_degrees, td.time_rotation).setEasePunch().setLoopCount(-1);


    }
}
