using UnityEngine;

public class Water_Exploding_Cap : MonoBehaviour
{
    [SerializeField] float up_max_position;
    [SerializeField] float time_exploding;
    [SerializeField] float up_bouncing_position;
    [SerializeField] float time_bounce;
    [SerializeField] float delay;

    void Start()
    {
        //exploding
        LeanTween.moveLocalZ(gameObject, up_max_position, time_exploding).setEaseOutElastic();

        //bouncing
        LeanTween.moveLocalZ(gameObject, up_bouncing_position, time_bounce).setEaseShake().setLoopCount(-1).setDelay(delay);
    }
}
