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
        CarrouselManager.carrouselProps.Add(this); 
    }

    public void SetActiveColor()
    {
        if (isActive || TouchDetection.CarrouselPropActivated == 3) return; 

        TouchDetection.CarrouselPropActivated++;
        isActive = true;
        meshRenderer.material.color = activeColor;

        if (TouchDetection.CarrouselPropActivated == 3)
        {
            CarrouselManager.Instance.SetFinalColorsDecorator();
        }
    }

    public void SetFinalColor()
    {
        meshRenderer.material.color = IsValidProp ? goodColor : badColor;
    }

    public void BackToDefaultColor()
    {
        Invoke(nameof(BackToDefault), 1f); 
    }

    private void BackToDefault()
    {
        isActive = false;
        meshRenderer.material.color = defaultColor;
    }
}
