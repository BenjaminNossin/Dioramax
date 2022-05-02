using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class CarrouselProp : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private GameObject goodOursonVFX;

    [Space, SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color activeColor = Color.yellow;
    [SerializeField] private Color goodColor = Color.green;
    [SerializeField] private Color badColor = Color.red;

    [Header("Gameplay")]
    [SerializeField] private WinCondition winCondition;
    [SerializeField] private bool isValidProp;

    public bool IsValidProp { get; private set; }
    public bool IsActive { get; private set; }

    private void Start()
    {
        IsValidProp = isValidProp;

        meshRenderer.material.color = defaultColor;
        CarrouselManager.CarrouselProps.Add(this); 
    }

    public void SetActiveColor()
    {
        if (IsActive || TouchDetection.CarrouselPropActivated == 3) return; 

        TouchDetection.CarrouselPropActivated++;
        IsActive = true;
        meshRenderer.material.color = activeColor;

        if (TouchDetection.CarrouselPropActivated == 3)
        {
            CarrouselManager.Instance.SetFinalColorsDecorator();
        }
    }

    public void SetFinalColor()
    {
        meshRenderer.material.color = isValidProp ? goodColor : badColor;
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

        meshRenderer.material.color = defaultColor;
    }

    private void SetGoodVFX(bool setActive)
    {
        goodOursonVFX.SetActive(setActive); 
    }
}
