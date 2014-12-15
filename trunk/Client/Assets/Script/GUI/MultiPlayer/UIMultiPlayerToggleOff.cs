using UnityEngine;
using System.Collections;

public class UIMultiPlayerToggleOff : MonoBehaviour
{
    public UIMultiPlayerToggle container;

    const float HIDING_TIME = 3.0f;

    float outStartTime;
    bool isShowed = false;

    public void Show()
    {
        isShowed = true;
        outStartTime = Time.time;
    }

    void Update()
    {
        if (!isShowed)
            return;

        if (Time.time - outStartTime >= HIDING_TIME)
        {
            isShowed = false;
            container.ShowEnableToggleOff();
        }
    }
}