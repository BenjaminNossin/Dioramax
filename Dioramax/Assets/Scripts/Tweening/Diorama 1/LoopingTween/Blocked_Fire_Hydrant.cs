using UnityEngine;

public class StoppableTween : MonoBehaviour
{
    private void OnDisable()
    {
        LeanTween.cancelAll();
    }
}

public class Blocked_Fire_Hydrant : StoppableTween
{
    [SerializeField] Vector3 orientation;
    [SerializeField] float time_rotation;
    [SerializeField] float up_max_position;
    [SerializeField] float time_bounce;

    void Start()
    {
       
        LeanTween.rotateLocal(gameObject, orientation, time_rotation).setEaseShake().setLoopPingPong();

        //bounce
        LeanTween.moveLocalY(gameObject, up_max_position, time_bounce).setEaseShake().setLoopPingPong();
    }
}
