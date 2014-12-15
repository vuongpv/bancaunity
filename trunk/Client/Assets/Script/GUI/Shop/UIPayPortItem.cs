using UnityEngine;
using System.Collections;

public class UIPayPortItem : MonoBehaviour
{
    public UISprite logo;

    UISelectPayPort manager = null;
    ConfigPayPortRecord data = null;

    public void Disable()
    {
        gameObject.SetActiveRecursively(false);
    }

    public void Setup(UISelectPayPort _manager, ConfigPayPortRecord _data)
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