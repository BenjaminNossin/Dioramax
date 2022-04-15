using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenTouch : MonoBehaviour
{

    public TweeningData td;

    void Start()
    {

        // ! remove looping

        //rotation
        LeanTween.rotateAround(gameObject, Vector3.up, td.rotation_degrees, td.time_rotation).setEaseOutCubic().setLoopCount(-1);

        //bounce
        LeanTween.moveLocalY(gameObject, td.up_max_position, td.time_bounce).setEasePunch().setLoopCount(-1);

        //stretch&squash
        LeanTween.scale(gameObject, td.stretch_squash, 1f).setEasePunch().setLoopCount(-1);

    }
}
