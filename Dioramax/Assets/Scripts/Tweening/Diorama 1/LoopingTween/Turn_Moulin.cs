using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn_Moulin : MonoBehaviour
{


    [SerializeField] float rotation_degrees;
    [SerializeField] float time_rotation;

    // Start is called before the first frame update
    void Start()
    {


        //rotation
        LeanTween.rotateAround(gameObject, Vector3.right, rotation_degrees, time_rotation).setEaseOutBack().setLoopCount(-1);

        //hover
        //LeanTween.moveLocalZ(gameObject, up_max_position, time_hover).setEaseOutSine().setLoopPingPong();

        //size
        //LeanTween.scale(gameObject, scale, 1f).setEaseOutSine().setLoopPingPong();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
