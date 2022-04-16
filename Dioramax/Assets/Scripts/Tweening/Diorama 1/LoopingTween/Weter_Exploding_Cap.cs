using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weter_Exploding_Cap : MonoBehaviour
{

    [SerializeField] float up_max_position;
    [SerializeField] float time_exploding;
    [SerializeField] float up_bouncing_position;
    [SerializeField] float time_bounce;
    [SerializeField] float delay;
    // Start is called before the first frame update
    void Start()
    {
        //exploding
        LeanTween.moveLocalZ(gameObject, up_max_position, time_exploding).setEaseOutElastic();

        //bouncing
        LeanTween.moveLocalZ(gameObject, up_bouncing_position, time_bounce).setEaseShake().setLoopCount(-1).setDelay(delay) ;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
