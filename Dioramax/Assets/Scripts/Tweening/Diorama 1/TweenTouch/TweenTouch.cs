using UnityEngine;
using DG.Tweening;

public class TweenTouch : StoppableTween
{
    public TweeningData td;

    // TempPosition
    [SerializeField] private float ObjectMaxHeight;
    [SerializeField] private float ObjectInitialHeight;
    [SerializeField] private Vector3 originalScale;
    private ParticleSystem VFX;
    private float TimeScale;
    private float TimeBounce;
    private Vector3 initialRotation;

    public bool tweenOnDisable;


    // Swap test 
    private bool swapState;

    public void Start()
    {
        initialRotation = transform.localRotation.eulerAngles;
        VFX = GetComponentInChildren<ParticleSystem>();
        ObjectInitialHeight = transform.position.y;
        ObjectMaxHeight = td.up_max_position + transform.position.y;
        originalScale = transform.localScale;
        TimeScale = td.time_scale * 0.3f;
        TimeBounce = td.time_bounce * 0.3f;

        //Swap test
        swapState = true;
    }

    public void ScaleGood()
    {
        //retour
        LeanTween.scale(gameObject, originalScale, TimeScale + (TimeScale/2)).setEaseOutBack();
    }

    public void Tween2()
    {  //retour
        //LeanTween.moveY(gameObject, ObjectInitialHeight, TimeBounce + (TimeBounce / 2)).setEaseOutBounce();
    }
    
    public void Swap()
    {
        
    }
    
    public void Tween()
    {
       
            //particlesystem
            if (VFX) // car pas de vfx sur certains objets.. (moulin, bouche d'incendie, bouton)
            {
                VFX.Play();
            }



        //bounce
        //transform.DOPunchPosition(ObjectMaxHeight, TimeBounce).SetDelay(td.delay);
        //transform.DOPunchScale(td.stretch_squash, TimeScale).SetDelay(td.delay);

            transform.DOMoveY(ObjectMaxHeight, TimeBounce).SetDelay(td.delay).OnComplete(() => transform.DOMoveY(ObjectInitialHeight, TimeBounce));
            transform.DOScale(td.stretch_squash, TimeScale).SetDelay(td.delay).OnComplete(() => transform.DOScale(originalScale, TimeScale));
        //LeanTween.moveY(gameObject, ObjectMaxHeight, TimeBounce).setDelay(td.delay).setEaseOutExpo().setOnComplete(Tween2);

        //stretch&squash (aller)

        //LeanTween.scale(gameObject, td.stretch_squash, TimeScale).setDelay(td.delay).setEaseOutExpo().setOnComplete(ScaleGood);//.setLoopCount(-1);



        if (transform.CompareTag("SwapRail"))
            {
                Debug.Log("Object is a Swap rail");

                if (swapState)
                {
                transform.DORotate((transform.rotation.eulerAngles + (td.RotationAxis * td.rotation_degrees)), td.time_rotation);
                    //LeanTween.rotateAroundLocal(gameObject, td.RotationAxis, td.rotation_degrees, td.time_rotation);//.setDelay(td.delay).setEaseOutCubic();//.setLoopCount(-1);
                    swapState = false;
                }
                else
                {
                transform.DORotate((transform.rotation.eulerAngles + (td.RotationAxis * -td.rotation_degrees)), td.time_rotation);
                    //LeanTween.rotateAroundLocal(gameObject, td.RotationAxis, -td.rotation_degrees, td.time_rotation);//.setDelay(td.delay).setEaseOutCubic();//.setLoopCount(-1);
                    swapState = true;
                }

            }
            else
            {
                //Rotation
                if (td.EaseOutCubic) {
                    //LeanTween.rotateAroundLocal(gameObject, td.RotationAxis, td.rotation_degrees, td.time_rotation);//.setDelay(td.delay).setEaseOutCubic();//.setLoopCount(-1);
                    Debug.Log("Test");
                //transform.DORotate(transform.rotation.eulerAngles + (td.RotationAxis * (td.rotation_degrees)), td.time_rotation, RotateMode.LocalAxisAdd);
                transform.DOLocalRotate(initialRotation + (td.RotationAxis * (td.rotation_degrees)), td.time_rotation, RotateMode.FastBeyond360);

                }
                else if (td.Punch) {
                    //LeanTween.rotateAroundLocal(gameObject, td.RotationAxis, td.rotation_degrees, td.time_rotation);//.setDelay(td.delay).setEasePunch();//.setLoopCount(-1);
                    transform.DORotate((transform.rotation.eulerAngles + (td.RotationAxis * td.rotation_degrees)), td.time_rotation).SetDelay(td.delay).SetEase(Ease.OutBounce);
                }
           }
        
        
    }

    private void OnDisable()
    {
        if (tweenOnDisable)
        {
            LeanTween.scale(gameObject, Vector3.one, TimeScale).setEaseOutBounce(); 
        }
    }
}
