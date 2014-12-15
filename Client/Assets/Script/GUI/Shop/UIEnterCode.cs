using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEnterCode : MonoBehaviour
{
    public UIShopHandler shopController;
    public UISprite logo;
    public UILabel serial;
    public UILabel code;

    ConfigGoldPackRecord pack;

    public void Setup(ConfigGoldPackRecord _pack)
    {
        gameObject.SetActiveRecursively(true);

        pack = _pack;

        ConfigPayPortRecord payPort = ConfigManager.configPayPort.GetItemByID(pack.payPortID);

        logo.spriteName = payPort.logoName;
        logo.MakePixelPerfect();

        serial.text = "";
        code.text = "";
    }

    void OnClick()
    {
        if (pack == null)
            return;

        GameObject obj = UICamera.selectedObject;

        switch (obj.name)
        {
            case "Back":
                shopController.SwitchContent(FHShopContentType.SelectGoldPack);
                break;

            case "OK":
                shopController.RequestCardPayment(pack, serial.text, code.text);
                break;
        }
    }
}