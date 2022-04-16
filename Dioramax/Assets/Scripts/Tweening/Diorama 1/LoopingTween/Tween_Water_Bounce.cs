using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween_Water_Bounce : MonoBehaviour
{

[SerializeField] float rotation_degrees;
[SerializeField] float time_rotation;
[SerializeField] float up_max_position;
[SerializeField] float time_bounce;
 [SerializeField] float time_stretchsquash;
 [SerializeField] Vector3 stretch_squash;
    [SerializeField] float time_explosion;
    [SerializeField] Vector3 explosion_stretch;
    [SerializeField] float delay;

    // Start is called before the first frame update
    void Start()
{
        //explosion

        //stretch&squash
        LeanTween.scale(gameObject, explosion_stretch, time_explosion).setEasePunch();


        //rotation
    LeanTween.rotateAround(gameObject, Vector3.up, rotation_degrees, time_rotation).setLoopPingPong().setDelay(delay);

    //bounce
    LeanTween.moveLocalY(gameObject, up_max_position, time_bounce).setEaseShake().setLoopPingPong().setDelay(delay);

    //stretch&squash
    LeanTween.scale(gameObject, stretch_squash, time_stretchsquash).setLoopPingPong().setDelay(delay);
}

    // Update is called once per frame
    void Update()
    {
        
    }
}
