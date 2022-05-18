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
    private Renderer rend;
    // public bool tweenOnDisable;
    private float FrozenState;

    // Swap test 
    private bool swapState;

    [Header("--DEBUG--")]
    [SerializeField] private bool doTween = true; 

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

        //Freeze on Start
        FrozenState = 0;
        rend = transform.GetComponent<MeshRenderer>();
        
        if (gameObject.CompareTag("Freezable"))
        {
            rend.material = td.FreezeMaterial;
            rend.material.SetFloat("Freezed", FrozenState);
            // gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);

            // renderer
        }

    }

    private void SwapFreeze()
    {

    }
    public void Tween()
    {
        if (!doTween) return;
        //Particle effects Active selon le tag 
        if (VFX)
        {
            if (!VFX.CompareTag("NoVFX") && !VFX.CompareTag("StopVFX"))
            {
                VFX.Play();
            }
            else if (VFX.CompareTag("StopVFX") && VFXPlaying)
            {
                VFXPlaying = false;
                StartCoroutine(StopDelay());
            }
        }
        // end Particle Effetcs
        
        //Move Object
        transform.DOMoveY(ObjectMaxHeight, TimeBounce).SetDelay(td.delay).SetEase(Ease.OutExpo).OnComplete(() => transform.DOMoveY(ObjectInitialHeight, TimeBounce).SetEase(Ease.OutBounce));

        //stretch&squash_Scale
        transform.DOScale(td.stretch_squash, TimeScale).SetDelay(td.delay).SetEase(Ease.OutExpo).OnComplete(() => transform.DOScale(originalScale, TimeScale).SetEase(Ease.OutBack));

        // Freeze Objects 
        //SwapFreeze();
        // Creat a function with the swap freeze named : SwapFreez then call it if the transfrom parent has a rend else call it for each childrens
        if (gameObject.CompareTag("Freezable") && rend != null)
        {
            if (FrozenState == 1)
            {
                FrozenState = 0;
                rend.material.SetFloat("Freezed", FrozenState);
            }

           else if (FrozenState == 0 && rend != null) 
            {
                FrozenState = 1;
               rend.material.SetFloat("Freezed", FrozenState);
            }
        }
        else if(rend != null)
        {
            foreach(Renderer Childrend in transform)
            {

            }
        }
        // End Freez Object
        
        // Swap Rail Logic
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
        Debug.Log(VFX);
        VFX.gameObject.SetActive(false);
        yield return new WaitForSeconds(td.DelayStopVFX);
        VFXPlaying = true;
        VFX.gameObject.SetActive(true);
    }

}
