using UnityEngine;
using System.Collections;
using DG.Tweening;

public class TweenTouch : StoppableTween
{
    public TweeningData td;

    // TempPosition
   
    private float ObjectMaxHeight;
    private float ObjectInitialHeight;
    private Vector3 originalScale;
    private ParticleSystem VFX;
    private float TimeScale;
    private float TimeBounce;
    private Vector3 initialRotation;
    private bool VFXPlaying;

   // public bool tweenOnDisable;


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
        
        // Deactivate VFX
        VFXPlaying = true;
    }

    
    public void Tween()
    {
       
            //particlesystem
            
            //Active selon le tag 
            if(!VFX.CompareTag("NoVFX") && !VFX.CompareTag("StopVFX"))
            {
                 if (VFX) // car pas de vfx sur certains objets.. (moulin, bouche d'incendie, bouton)
                  {
                        VFX.Play();
                  }
            }
            // deactivate a VFX when touched
            else if(VFX.CompareTag("StopVFX") && VFXPlaying)
            {
                VFXPlaying = false;

                if(VFX)
                {
                    VFX.Stop();
                StartCoroutine(StopDelay());
                }
            }

        //Move

        transform.DOMoveY(ObjectMaxHeight, TimeBounce).SetDelay(td.delay).SetEase(Ease.OutExpo).OnComplete(() => transform.DOMoveY(ObjectInitialHeight, TimeBounce).SetEase(Ease.OutBounce));

        //stretch&squash_Scale
        transform.DOScale(td.stretch_squash, TimeScale).SetDelay(td.delay).SetEase(Ease.OutExpo).OnComplete(() => transform.DOScale(originalScale, TimeScale).SetEase(Ease.OutBack));



        if (transform.CompareTag("SwapRail"))
            {

                if (swapState)
                {
                transform.DORotate((transform.rotation.eulerAngles + (td.RotationAxis * td.rotation_degrees)), td.time_rotation).SetEase(Ease.OutCubic);
                    swapState = false;
                }
                else
                {
                transform.DORotate((transform.rotation.eulerAngles + (td.RotationAxis * -td.rotation_degrees)), td.time_rotation).SetEase(Ease.OutCubic);
                    swapState = true;
                }

            }
            else
            {
                //Rotation
                if (td.EaseOutCubic) {
                 
                transform.DOLocalRotate(initialRotation + (td.RotationAxis * (td.rotation_degrees)), td.time_rotation, RotateMode.FastBeyond360).SetDelay(td.delay).SetEase(Ease.OutCubic);

                }
                else if (td.EaseOutCirc) {
           
                transform.DOLocalRotate(initialRotation + (td.RotationAxis * (td.rotation_degrees)), td.time_rotation, RotateMode.FastBeyond360).SetDelay(td.delay).SetEase(Ease.OutCirc);
            } 
            
                else if (td.EaseInBack) {
 
                transform.DORotate((transform.rotation.eulerAngles + (td.RotationAxis * td.rotation_degrees)), td.time_rotation).SetDelay(td.delay).SetEase(Ease.OutBack);

            }
           }


    }

    //Couroutine Wait delay before activating VFX again
    IEnumerator StopDelay()
    {

        yield return new WaitForSeconds(td.DelayStopVFX);
        VFXPlaying = true;
        VFX.Play();
    }

}