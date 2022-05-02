using UnityEngine;

public class Select_Ours : StoppableTween
{
    [SerializeField] float up_max_position;
    [SerializeField] float time_bounce;

    private void OnEnable()
    {
        LeanTween.moveLocalZ(gameObject, up_max_position, time_bounce).setEaseInOutSine().setLoopPingPong();
    } 
}
