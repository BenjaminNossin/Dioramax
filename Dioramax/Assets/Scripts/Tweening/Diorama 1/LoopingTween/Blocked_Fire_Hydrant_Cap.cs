using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocked_Fire_Hydrant_Cap : MonoBehaviour
{

    [SerializeField] float up_max_position;
    [SerializeField] float time_bounce;
    // Start is called before the first frame update
    void Start()
    {
        //bounce
        LeanTween.moveLocalZ(gameObject, up_max_position, time_bounce).setEaseShake().setLoopCount(-1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
