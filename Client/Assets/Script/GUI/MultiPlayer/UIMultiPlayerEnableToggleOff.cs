using UnityEngine;
using System.Collections;

public class UIMultiPlayerEnableToggleOff : MonoBehaviour
{
    public UIMultiPlayerToggle container;

    const float ENABLE_HOLDING_TIME = 1.0f;

    bool isSelected = false;
    float holdingStartTime;

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            if (!isSelected)
            {
                isSelected = true;
                holdingStartTime = Time.time;
            }
        }
        else
            isSelected = false;
    }

    void Update()
    {
        if (!isSelected)
            return;

        if (Time.time - holdingStartTime >= ENABLE_HOLDING_TIME)
        {
            container.ShowToggleOff();

            isSelected = false;
            holdingStartTime = 0.0f;
        }
    }
}