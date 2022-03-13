using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaceholderFeedback : MonoBehaviour
{
    public MeshRenderer renderer;

    private void Start()
    {
        renderer.material.color = Color.red; 
    }

    public void ChangeColor(bool _changeBackAfterDelay = true)
    {
        renderer.material.color = Color.blue;

        if (_changeBackAfterDelay)
        {
            StartCoroutine((nameof(ChangeBackRoutine)));
        }
    }

    private readonly WaitForSeconds colorChangeWFS = new WaitForSeconds(0.5f); 
    IEnumerator ChangeBackRoutine()
    {
        yield return colorChangeWFS;
        ChangeBack(); 
    }

    public void ChangeBack()
    {
        renderer.material.color = Color.red;
    }
}
