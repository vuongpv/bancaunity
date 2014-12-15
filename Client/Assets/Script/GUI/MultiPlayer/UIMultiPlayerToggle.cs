using UnityEngine;
using System.Collections;

public class UIMultiPlayerToggle : MonoBehaviour
{
    public FHMutliPlayerPanel container;

    public GameObject enableToggleOff;
    
    public GameObject toggleOff;

    public GameObject toggleOn;

    public FHFocusNotify focusNotify;

    const float ENABLE_HOLDING_TIME = 1.0f;

    bool isSelected = false;
    float holdingStartTime;
    float hoverStartTime;

    public void Hide()
    {
        gameObject.SetActiveRecursively(false);
    }

    public void ShowToggleOn()
    {
        gameObject.SetActiveRecursively(true);

        toggleOn.SetActiveRecursively(true);
        enableToggleOff.SetActiveRecursively(false);
        toggleOff.SetActiveRecursively(false);
        focusNotify.gameObject.SetActiveRecursively(false);

        isSelected = false;
    }

    public void ShowEnableToggleOff()
    {
        gameObject.SetActiveRecursively(true);

        toggleOn.SetActiveRecursively(false);
        enableToggleOff.SetActiveRecursively(true);
        toggleOff.SetActiveRecursively(false);
        focusNotify.gameObject.SetActiveRecursively(false);
    }

    public void ShowToggleOff()
    {
        gameObject.SetActiveRecursively(true);

        toggleOn.SetActiveRecursively(false);
        enableToggleOff.SetActiveRecursively(false);
        toggleOff.SetActiveRecursively(true);
        focusNotify.gameObject.SetActiveRecursively(true);
        focusNotify.Setup();

        toggleOff.GetComponent<UIMultiPlayerToggleOff>().Show();
    }

    void OnClick()
    {
        switch (UICamera.selectedObject.name)
        {
            case "ToggleOn":
                FHMultiPlayerManager.instance.Show(container.controller);
                break;

            case "ToggleOff":
                FHMultiPlayerManager.instance.Hide(container.controller);
                break;
        }
    }
}