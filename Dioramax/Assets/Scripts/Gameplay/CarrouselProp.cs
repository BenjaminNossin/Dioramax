using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Select_Ours))]
public class CarrouselProp : MonoBehaviour
{
    [SerializeField] private GameObject goodOursonVFX;

    [Header("Gameplay")]
    [SerializeField] private WinCondition winCondition;
    [SerializeField] private bool isValidProp;

    private Select_Ours selectOurs; 

    public bool IsValidProp { get; private set; }
    public bool IsActive { get; private set; }

    private void Start()
    {
        selectOurs = GetComponent<Select_Ours>();
        IsValidProp = isValidProp;

        CarrouselManager.CarrouselProps.Add(this); 
    }

    public void SetActiveColor()
    {
        if (IsActive || TouchDetection.CarrouselPropActivated == 3) return; 

        TouchDetection.CarrouselPropActivated++;
        IsActive = true;
        selectOurs.enabled = IsActive; 

        if (TouchDetection.CarrouselPropActivated == 3)
        {
            CarrouselManager.Instance.SetFinalColorsDecorator();
        }
    }

    public void IsValidPropFeedback()
    {
        if (IsValidProp)
        {
            SetGoodVFX(true); 
        }

        winCondition.UpdateWinCondition(isValidProp);
    }

    public void BackToDefaultState()
    {
        Invoke(nameof(BackToDefault), 2f);
        winCondition.UpdateWinCondition(false);
    }

    private void BackToDefault()
    {
        IsActive = false;
        selectOurs.enabled = IsActive; 

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
