using UnityEngine;
using DG.Tweening;

public class Turn_Star : MonoBehaviour
{
    private float ObjectMaxHeight;
        //private Vector3 initialRotation;

    public Vector3 RotationAxis = Vector3.up;
    [SerializeField] float rotation_degrees = -360f;
    [SerializeField] float time_rotation = 2f;
    [SerializeField] float up_max_position = 0.17f;
    [SerializeField] float TimeUp = 2f;
    [SerializeField] Vector3 scale = Vector3.one;
    public float time_scale = 1f;
    [SerializeField] Vector3 UpAxis = Vector3.up;// = new Vector3 (0,0,1);

    // Start is called before the first frame update
    void Start()
    {

        //init 
            //initialRotation = transform.localRotation.eulerAngles;
        ObjectMaxHeight = up_max_position + transform.position.y;


        //rotation
            //LeanTween.rotateAround(gameObject, Vector3.up, rotation_degrees, time_rotation).setLoopCount(-1);
        transform.DOLocalRotate((RotationAxis * (rotation_degrees)), time_rotation, RotateMode.Fast).SetLoops(-1).SetEase(Ease.Linear).SetRelative();


        //hover
        transform.DOMoveY(ObjectMaxHeight, TimeUp).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);

        //size
            // LeanTween.scale(gameObject, scale, 1f).setEaseOutSine().setLoopPingPong();
        transform.DOScale(scale, time_scale).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
    }
}
