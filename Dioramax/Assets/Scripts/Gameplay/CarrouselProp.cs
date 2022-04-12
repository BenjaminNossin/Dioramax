using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class CarrouselProp : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    [Space, SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color activeColor = Color.yellow;
    [SerializeField] private Color goodColor = Color.green;
    [SerializeField] private Color badColor = Color.red;

    public bool IsValidProp;
    public bool isActive; 

    private void Start()
    {
        meshRenderer.material.color = defaultColor;
    }

    public void SetActiveColor()
    {
        if (isActive) return;

        TouchDetection.CarrouselPropActivated++;
        isActive = true; 
        meshRenderer.material.color = activeColor; 
    }

    public void SetFinalColor()
    {
        meshRenderer.material.color = IsValidProp ? goodColor : badColor; 
    }

    private void BackToDefaultColor()
    {
        TouchDetection.CarrouselPropActivated--;
        isActive = true; 
        meshRenderer.material.color = defaultColor;
    }
}
