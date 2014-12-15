using UnityEngine;
using System.Collections;

public class UICardTypeItem : MonoBehaviour
{
    public UISprite logo;

    UICardSelectType manager = null;
    ConfigCardTypeRecord data = null;

    public void Disable()
    {
        gameObject.SetActiveRecursively(false);
    }

    public void Setup(UICardSelectType _manager, ConfigCardTypeRecord _data)
    {
        manager = _manager;
        data = _data;

        gameObject.SetActiveRecursively(true);

        logo.spriteName = data.logoName;
        logo.MakePixelPerfect();
    }

    void OnClick()
    {
        if (data == null)
            return;

        manager.NotifySwitchContent(data);
    }
}