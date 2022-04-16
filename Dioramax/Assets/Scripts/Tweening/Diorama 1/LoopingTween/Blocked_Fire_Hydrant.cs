using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocked_Fire_Hydrant : MonoBehaviour
{


    [SerializeField] Vector3 orientation;
    [SerializeField] float time_rotation;
    [SerializeField] float up_max_position;
    [SerializeField] float time_bounce;

    // Start is called before the first frame update
    void Start()
    {
       
        LeanTween.rotateLocal(gameObject, orientation, time_rotation).setEaseShake().setLoopPingPong();

        //bounce
        LeanTween.moveLocalY(gameObject, up_max_position, time_bounce).setEaseShake().setLoopPingPong();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
