using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TweenTouch")]
public class TweeningData : ScriptableObject
{
    [Header("Move")]
    [Range(0,1)]public float up_max_position;
    public float time_bounce;

    [Header("Scale")]
    public Vector3 stretch_squash = new Vector3(1,1,1);
    public float time_scale;

    [Header("Rotation")]
    public float rotation_degrees;
    public float time_rotation;
    [Header("Rotation Curve (only use one)")]
    public bool EaseOutCubic;
    public bool Punch;
    [Header("Vector.right = 1,0,0 | Vector.up = 0,1,0")]
    public Vector3 RotationAxis;


    //[Header("ParticleSystem")]
   // public GameObject ParticleSystem;

    // Test
    //[Header("Punch = 1 | Linear = 2 ")]
    //public float CurveType;
}
