using UnityEngine;

public class Turn_Star : MonoBehaviour
{


    [SerializeField] float rotation_degrees;
    [SerializeField] float time_rotation;
    [SerializeField] float up_max_position;
    [SerializeField] float time_hover;
    [SerializeField] Vector3 scale;

    // Start is called before the first frame update
    void Start()
    {

        // ! remove looping

        //rotation
        LeanTween.rotateAround(gameObject, Vector3.up, rotation_degrees, time_rotation).setLoopCount(-1);

        //hover
        LeanTween.moveLocalZ(gameObject, up_max_position, time_hover).setEaseOutSine().setLoopPingPong();

        //size
        LeanTween.scale(gameObject, scale, 1f).setEaseOutSine().setLoopPingPong();
    }
}
