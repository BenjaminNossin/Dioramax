using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrouselManager : MonoBehaviour
{
    [SerializeField] private List<Select_Ours> oursonTweens = new(); 
    public static List<CarrouselProp> carrouselProps = new List<CarrouselProp>();

    public static CarrouselManager Instance;

    private void Awake()
    {
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
        for (i = 0; i < carrouselProps.Count; i++)
        {
            if (carrouselProps[i].isActive)
            {
                carrouselProps[i].SetFinalColor();
                if (carrouselProps[i].IsValidProp)
                {
                    TouchDetection.ValidCarrouselPropAmount++;
                }
            }
        }

        if (TouchDetection.ValidCarrouselPropAmount != 3)
        {
            for (i = 0; i < carrouselProps.Count; i++)
            {
                if (carrouselProps[i].isActive)
                {
                    carrouselProps[i].BackToDefaultColor();
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
