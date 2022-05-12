using UnityEngine;

public class Tween_Star_Finish : MonoBehaviour
{
    [SerializeField] Vector3 explosion_stretch;
    [SerializeField] float time_explosion;
    [SerializeField] float up_max_position_start;

    [SerializeField] float delay;

    [SerializeField] float rotation_degrees;
    [SerializeField] float time_rotation;
    [SerializeField] float up_max_position;

    [SerializeField] float time_bounce;

    [SerializeField] Vector3 stretch_squash;
    [SerializeField] float time_stretchsquash;




    // Start is called before the first frame update
    void Start()
    {
        //explosion

        //stretch&squash
        LeanTween.scale(gameObject, explosion_stretch, time_explosion).setEasePunch();
        LeanTween.moveY(gameObject, up_max_position_start, time_explosion).setEaseInBounce().setLoopPingPong();


        //rotation
        LeanTween.rotateAround(gameObject, Vector3.up, rotation_degrees, time_rotation).setLoopPingPong();

        //bounce
        LeanTween.moveY(gameObject, up_max_position, time_bounce).setEaseShake().setLoopPingPong().setDelay(delay);

        //stretch&squash
        LeanTween.scale(gameObject, stretch_squash, time_stretchsquash).setLoopPingPong().setDelay(delay);
        // Update is called once per frame


    }
}
