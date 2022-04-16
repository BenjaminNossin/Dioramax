using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TweenTouch : MonoBehaviour
{
    public TweeningData td;

    // TempPosition
    private Vector3 ObjectJumpPosition;

    //public void Tween()
    void Update()

    {

            // ! remove looping
            ObjectJumpPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + td.up_max_position, gameObject.transform.position.z);

            //bounce
            LeanTween.moveLocal(gameObject, ObjectJumpPosition, td.time_bounce).setEasePunch().setLoopCount(-1);

            //stretch&squash
            LeanTween.scale(gameObject, td.stretch_squash, 1f).setEasePunch().setLoopCount(-1);

            //rotation

            if (td.EaseOutCubic)
                LeanTween.rotateAround(gameObject, td.RotationAxis, td.rotation_degrees, td.time_rotation).setEaseOutCubic().setLoopCount(-1);

            if (td.Punch)
                LeanTween.rotateAround(gameObject, td.RotationAxis, td.rotation_degrees, td.time_rotation).setEasePunch().setLoopCount(-1);

    }
}
