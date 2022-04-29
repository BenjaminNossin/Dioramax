using UnityEngine;

public class ButtonPropsManager : MonoBehaviour
{
    [SerializeField] private ButtonProp[] buttons = new ButtonProp[4];
    public static ButtonPropsManager Instance;

    public static ButtonProp[] Buttons;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
        }

        Instance = this;
    }

    private void Start()
    {
        Buttons = new ButtonProp[4];
        for (int i = 0; i < buttons.Length; i++)
        {
            Buttons[i] = buttons[i];
        }
    }

    public void SetCurrentButtonProp(ButtonProp _current)
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            if (Buttons[i] == _current)
            {
                Buttons[i].InverseButtonState();
            }
            else
            {
                Buttons[i].SetButtonOff();
            }
        }
    }
}
