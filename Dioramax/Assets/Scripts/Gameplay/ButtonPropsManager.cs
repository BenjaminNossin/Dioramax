using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPropsManager : MonoBehaviour
{
    public static ButtonPropsManager Instance;
    private static ButtonProp currentButtonProp;
    private static ButtonProp previousButtonProp;

    // DEBUG
    private ButtonProp current;
    private ButtonProp previous; 

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
        }

        Instance = this;
        currentButtonProp = previousButtonProp = null; 
    }

    public void SetCurrentButtonProp(ButtonProp _current)
    {
        currentButtonProp = _current;
        current = currentButtonProp;
    }

    public void SetPreviousButtonProp(ButtonProp _previous)
    {
        previousButtonProp = _previous;
        previous = previousButtonProp; 
    }

    public bool CheckPropEquality()
    {
        if (!currentButtonProp || !previousButtonProp) return default;
        return currentButtonProp == previousButtonProp;
    }
}
