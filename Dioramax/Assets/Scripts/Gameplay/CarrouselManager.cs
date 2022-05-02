using System.Collections.Generic;
using UnityEngine;

public class CarrouselManager : MonoBehaviour
{
    [SerializeField] private List<Select_Ours> oursonTweens = new();
    public static List<CarrouselProp> CarrouselProps { get; set; }

    public static CarrouselManager Instance;

    private void Awake()
    {
        CarrouselProps = new List<CarrouselProp>();
        if (Instance)
        {
            Destroy(Instance); 
        }

        Instance = this; 
    }

    public void SetFinalColorsDecorator()
    {
        Invoke(nameof(SetFinalColors), 1f); 
    }

    int i;
    private void SetFinalColors()
    {
        for (i = 0; i < CarrouselProps.Count; i++)
        {
            if (CarrouselProps[i].IsActive)
            {
                CarrouselProps[i].SetFinalColor();
                if (CarrouselProps[i].IsValidProp)
                {
                    TouchDetection.ValidCarrouselPropAmount++;
                }
            }
        }

        if (TouchDetection.ValidCarrouselPropAmount != 3)
        {
            for (i = 0; i < CarrouselProps.Count; i++)
            {
                if (CarrouselProps[i].IsActive)
                {
                    CarrouselProps[i].BackToDefaultColor();
                }
            }

            for (int i = 0; i < oursonTweens.Count; i++)
            {
                oursonTweens[i].enabled = false;
            }

            TouchDetection.CarrouselPropActivated = 0;
            TouchDetection.ValidCarrouselPropAmount = 0;
        }
        else
        {
            Debug.Log("Puzzle carrousel completed !");
        }
    }
}
