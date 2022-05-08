using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class CarrouselProp : MonoBehaviour
{
    [SerializeField] private GameObject goodOursonVFX;

    [Header("Gameplay")]
    [SerializeField] private WinCondition winCondition;
    [SerializeField] private bool isValidProp;

    public bool IsValidProp { get; private set; }
    public bool IsActive { get; private set; }

    private void Start()
    {
        IsValidProp = isValidProp;

        CarrouselManager.CarrouselProps.Add(this); 
    }

    public void SetActiveColor()
    {
        if (IsActive || TouchDetection.CarrouselPropActivated == 3) return; 

        TouchDetection.CarrouselPropActivated++;
        IsActive = true;

        if (TouchDetection.CarrouselPropActivated == 3)
        {
            CarrouselManager.Instance.SetFinalColorsDecorator();
        }
    }

    public void SetFinalColor()
    {
        if (IsValidProp)
        {
            SetGoodVFX(true); 
        }

        winCondition.UpdateWinCondition(isValidProp);
    }

    public void BackToDefaultColor()
    {
        Invoke(nameof(BackToDefault), 2f);
        winCondition.UpdateWinCondition(false);
    }

    private void BackToDefault()
    {
        IsActive = false;

        if (IsValidProp)
        {
            SetGoodVFX(false);
        }
    }

    private void SetGoodVFX(bool setActive)
    {
        goodOursonVFX.SetActive(setActive); 
    }
}
