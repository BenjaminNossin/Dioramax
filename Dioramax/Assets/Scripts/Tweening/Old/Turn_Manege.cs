using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn_Manege : MonoBehaviour
{

    [SerializeField] float rotation_degrees;
    [SerializeField] float time_rotation;
    [SerializeField] float up_max_position;
    [SerializeField] float time_bounce;
    [SerializeField] Vector3 stretch_squash;

    // Start is called before the first frame update
    void Start()
    {

        // ! remove looping

        //rotation
        LeanTween.rotateAround(gameObject,Vector3.up, rotation_degrees, time_rotation).setEaseOutCubic().setLoopCount(-1);
        
        //bounce
        LeanTween.moveLocalY(gameObject, up_max_position, time_bounce).setEasePunch().setLoopCount(-1);

        //stretch&squash
        LeanTween.scale(gameObject,stretch_squash, 1f).setEasePunch().setLoopCount(-1) ;
    }

    // Update is called once per frame
    void Update()
    {


    }
}
