using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween_RailSwitch : MonoBehaviour
{
    //[SerializeField]
    Vector3 rotateAxis;
    [SerializeField] float rotateDegree;
    [SerializeField] float Delay;
    [SerializeField] float timeRotation;


    Quaternion InitialRot;
    Quaternion altrotation;

    void Start()
    {
        //altrotation = new Quaternion(Setalrotation.x, Setalrotation.y, Setalrotation.z, Setalrotation.w);
        //transform.rotation = altrotation;
        //transform.rotation = new Quaternion(Setalrotation.x, Setalrotation.y, Setalrotation.z, Setalrotation.w);
        //InitialRot = transform.rotation;

        rotateAxis = Vector3.up;
    }

    private void swapPosition()
    {
        LeanTween.rotateAround(gameObject, rotateAxis, rotateDegree, timeRotation).setDelay(Delay).setEaseOutCubic();//.setLoopCount(-1);
        //altrotation = new Quaternion(Setalrotation.x, Setalrotation.y, Setalrotation.z, Setalrotation.w);
        //transform.rotation = altrotation;
    }

}
