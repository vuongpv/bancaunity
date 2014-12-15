using UnityEngine;
using System.Collections;

public class UIShopTabItem : MonoBehaviour
{
    public UISprite icon;

    public GameObject tabOn;

    public GameObject tabOff;

    public int dataIndex;

    public void SetEnable(bool enable)
    {
        if (enable)
            UIHelper.EnableWidget(gameObject);
        else
            UIHelper.DisableWidget(gameObject);
    }

    public void SetState(bool active)
    {
        if (active)
        {
            UIHelper.EnableWidget(tabOn);
            UIHelper.DisableWidget(tabOff);
        }
        else
        {
            UIHelper.DisableWidget(tabOn);
            UIHelper.EnableWidget(tabOff);
        }
    }

    public void SetIcon(string iconName)
    {
        icon.spriteName = iconName;
        icon.MakePixelPerfect();
    }
}