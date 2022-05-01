using UnityEngine;

public class Blocked_Fire_Hydrant_Cap : StoppableTween
{

    [SerializeField] float up_max_position;
    [SerializeField] float time_bounce;

    void Start()
    {
        //bounce
        LeanTween.moveLocalZ(gameObject, up_max_position, time_bounce).setEaseShake().setLoopCount(-1);
    }
}
