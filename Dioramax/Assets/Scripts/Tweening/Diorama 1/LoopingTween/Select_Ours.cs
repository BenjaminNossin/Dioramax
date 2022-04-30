using UnityEngine;



public class Select_Ours : MonoBehaviour
{
    [SerializeField] float up_max_position;
    [SerializeField] float time_bounce;

    // Start is called before the first frame update
    void Start()
    {
        LeanTween.moveLocalZ(gameObject, up_max_position, time_bounce).setEaseInOutSine().setLoopPingPong();
    }
}
