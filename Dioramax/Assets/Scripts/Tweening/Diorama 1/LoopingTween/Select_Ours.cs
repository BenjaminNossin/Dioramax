using DG.Tweening;

public class Select_Ours : StoppableTween
{
  /*  [SerializeField] float up_max_position;
    [SerializeField] float time_bounce;
*/
    private void OnEnable()
    {
    //        ObjectMaxHeight = up_max_position + transform.position.y;
        // LeanTween.moveLocalZ(gameObject, up_max_position, time_bounce).setEaseInOutSine().setLoopPingPong();
        transform.DOMoveY(0.5f, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
