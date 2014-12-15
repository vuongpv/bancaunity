using UnityEngine;
using System.Collections;

public class FHMutliPlayerPanel : MonoBehaviour
{
    public FHPlayerMultiController controller;
    public Transform container;
    public UIMultiPlayerToggle toggle;

    float containerStartY = -125.0f;
    float containerFinishY = 0.0f;
    float containerScrollingSpeed = 500.0f;

    Vector3 direction = Vector3.zero;
    bool isScrolling = false;
    bool isShowed = false;

    FHPlayerMultiController player;
    FHPlayerPanelToggleCallback callback = null;

    public void Setup(bool isActive)
    {
        container.localPosition = (isActive ? new Vector3(0.0f, containerFinishY, 0.0f) : new Vector3(0.0f, containerStartY, 0.0f));

        if (controller.isHostingPlayer)
            toggle.Hide();
        else
        {
            if (isActive)
                toggle.ShowEnableToggleOff();
            else
                toggle.ShowToggleOn();
        }

        isShowed = isActive;
        isScrolling = false;
    }

    public void Toggle(FHPlayerMultiController _player, FHPlayerPanelToggleCallback _callback)
    {
        if (isScrolling)
            return;

        player = _player;
        callback = _callback;

        toggle.Hide();

        if (isShowed)
            direction = -Vector3.up;
        else
            direction = Vector3.up;

        isScrolling = true;
    }

    void Update()
    {
        if (!isScrolling)
            return;

        container.localPosition += direction * containerScrollingSpeed * Time.deltaTime;

        if (container.localPosition.y >= containerFinishY)
        {
            container.localPosition = new Vector3(container.localPosition.x, containerFinishY, container.localPosition.z);

            isShowed = true;
            isScrolling = false;

            toggle.ShowEnableToggleOff();

            if (callback != null)
                callback(player, true);
        }
        else
        if (container.localPosition.y <= containerStartY)
        {
            container.localPosition = new Vector3(container.localPosition.x, containerStartY, container.localPosition.z);

            isShowed = false;
            isScrolling = false;

            toggle.ShowToggleOn();

            if (callback != null)
                callback(player, false);
        }
    }
}