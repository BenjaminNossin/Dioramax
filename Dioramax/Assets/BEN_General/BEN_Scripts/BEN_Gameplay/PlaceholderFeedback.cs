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

    public void ChangeColor()
    {
        renderer.material.color = Color.blue;
        StartCoroutine((nameof(ChangeBack)));
    }

    IEnumerator ChangeBack()
    {
        yield return new WaitForSeconds(0.5f);
        renderer.material.color = Color.red; 
    }
}
