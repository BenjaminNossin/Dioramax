using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractableEntity : MonoBehaviour
{
    [SerializeField] protected MeshRenderer renderer;
    protected bool isActive; 

    private void Start()
    {
        renderer.material.color = Color.red; 
    }

    public void ChangeColor(bool _changeBackAfterDelay = true)
    {
        isActive = true; 
        renderer.material.color = Color.blue;

        if (_changeBackAfterDelay)
        {
            StartCoroutine((nameof(ChangeColorRoutine)));
        }
    }

    private readonly WaitForSeconds colorChangeWFS = new WaitForSeconds(0.5f); 
    IEnumerator ChangeColorRoutine()
    {
        yield return colorChangeWFS;
        SwapOrChangeBack(false); 
    }

    public void SwapOrChangeBack(bool swap)
    {
        isActive = swap ? !isActive : false;
        renderer.material.color = isActive ? Color.blue : Color.red;
    }
}
