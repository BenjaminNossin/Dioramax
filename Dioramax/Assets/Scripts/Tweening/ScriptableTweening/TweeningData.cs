using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TweenTouch")]
public class TweeningData : ScriptableObject
{
    [Header("Rotation")]
     public float rotation_degrees;
    public float time_rotation;

    [Header("Move")]
    public float up_max_position;
    public float time_bounce;

    [Header("Scale")]
    public Vector3 stretch_squash;
    public float time_scale;
}
