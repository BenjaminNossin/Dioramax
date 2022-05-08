using UnityEngine;

public class TweenTouch : StoppableTween
{
    public TweeningData td;

    // TempPosition
    private Vector3 ObjectJumpPosition;
    private float ObjectMaxHeight;
    private float ObjectInitialHeight;
    private Vector3 originalScale;
    private ParticleSystem VFX;
    private float TimeScale;
    private float TimeBounce;

    public bool tweenOnDisable; 

    public void Start()
    {
        VFX = GetComponentInChildren<ParticleSystem>();

        ObjectInitialHeight = transform.position.y;
        ObjectMaxHeight = transform.position.y + td.up_max_position;
        originalScale = transform.localScale;
        ObjectJumpPosition = new Vector3(transform.position.x, ObjectMaxHeight, transform.position.z);
        TimeScale = td.time_scale * 0.3f;
        TimeBounce = td.time_bounce * 0.3f;

    }

    public void ScaleGood()
    {
        //retour
        LeanTween.scale(gameObject, originalScale, TimeScale + (TimeScale/2)).setEaseOutBack();
    }

    public void Tween2()
    {  //retour
        LeanTween.moveY(gameObject, ObjectInitialHeight, TimeBounce + (TimeBounce / 2)).setEaseOutBounce();
    }
    
    public void Tween()
    {
        // ! remove looping

        //bounce (aller)
        LeanTween.moveY(gameObject, ObjectMaxHeight, TimeBounce).setDelay(td.delay).setEaseOutExpo().setOnComplete(Tween2);

        //stretch&squash (aller)
        LeanTween.scale(gameObject, td.stretch_squash, TimeScale).setDelay(td.delay).setEaseOutExpo().setOnComplete(ScaleGood);//.setLoopCount(-1);

        //rotation

        if (td.EaseOutCubic)
            LeanTween.rotateAround(gameObject, td.RotationAxis, td.rotation_degrees, td.time_rotation).setDelay(td.delay).setEaseOutCubic();//.setLoopCount(-1);

        if (td.Punch)
            LeanTween.rotateAround(gameObject, td.RotationAxis, td.rotation_degrees, td.time_rotation).setDelay(td.delay).setEasePunch();//.setLoopCount(-1);
        
        //particlesystem
        if (VFX) // car pas de vfx sur certains objets.. (moulin, bouche d'incendie, bouton)
        {
            VFX.Play(); 
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
