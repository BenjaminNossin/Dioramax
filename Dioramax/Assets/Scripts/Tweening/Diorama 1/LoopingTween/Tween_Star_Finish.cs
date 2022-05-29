using UnityEngine;
using DG.Tweening;

public class Tween_Star_Finish : MonoBehaviour
{
    [SerializeField] private AudioSource starFreeAudioSource;

    private float ObjectMaxHeight;
    private float ObjectInitialHeight;
    private Vector3 initialRotation;

    [Header("Move")]
    public float TimeUp;
    public float TimeFall;
    public float up_max_position;


    [Header("Scale")]
    public float time_scale;
    public Vector3 ScaleUp;


    [Header("Rotation")]
    public Vector3 RotationAxis;
    public float rotation_degrees_1;
    public float rotation_degrees_2;
    public float time_rotation_1;
    public float time_rotation_2;

    [Header("VFX Sparkle Finish")]
    public ParticleSystem VFX;




    public void Start()
    {
        starFreeAudioSource.Play();

        //init 
        initialRotation = transform.localRotation.eulerAngles;
        ObjectInitialHeight = transform.position.y;
        ObjectMaxHeight = up_max_position + transform.position.y;
        

        //Animation
        //vfx sparkle finish
        VFX.Play();

        //rotation
        transform.DOLocalRotate(initialRotation + (RotationAxis * (rotation_degrees_1)), time_rotation_1, RotateMode.LocalAxisAdd).SetEase(Ease.InOutQuad).OnComplete(()
            => transform.DOLocalRotate((RotationAxis * (rotation_degrees_2)), time_rotation_2, RotateMode.Fast).SetLoops(-1).SetEase(Ease.Linear).SetRelative());

        //move
        transform.DOMoveY(ObjectMaxHeight, TimeUp).SetEase(Ease.OutBounce).OnComplete(()
            => transform.DOMoveY(ObjectInitialHeight, TimeFall).SetEase(Ease.InBack));

        //scale
        transform.DOScale(ScaleUp, time_scale).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);

    }
}
