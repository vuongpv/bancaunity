using UnityEngine;
using System.Collections;

public class UICardEnterInfo : MonoBehaviour
{
    public UICardExchange controller;
    public UISprite logo;
    public UILabel email;
    public UILabel phoneNumber;

    ConfigCardRecord card;

    public void Setup(ConfigCardRecord _card)
    {
        gameObject.SetActiveRecursively(true);

        card = _card;

        ConfigCardTypeRecord cardType = ConfigManager.configCardType.GetCardTypeByID(card.cardType);
        logo.spriteName = cardType.logoName;
        logo.MakePixelPerfect();

        email.text = "";
        phoneNumber.text = "";
    }

    void OnClick()
    {
        if (card == null)
            return;

        GameObject obj = UICamera.selectedObject;

        switch (obj.name)
        {
            case "Back":
                controller.SwitchState(FHCardExchangeState.SelectType);
                break;

            case "OK":
                controller.RequestCardExchange(card, email.text, phoneNumber.text);
                break;
        }
    }

}
