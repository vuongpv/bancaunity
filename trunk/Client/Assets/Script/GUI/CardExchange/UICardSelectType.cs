using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICardSelectType : MonoBehaviour
{
    const int NUMBER_CARD_TYPE_PER_PAGE = 3;

    public UICardExchange controller;

    List<UICardTypeItem> cardTypeControllers = new List<UICardTypeItem>();

    public void Setup()
    {
        gameObject.SetActiveRecursively(true);

        cardTypeControllers.Clear();
        for (int i = 0; i < NUMBER_CARD_TYPE_PER_PAGE; i++)
        {
            UICardTypeItem payPort = gameObject.transform.FindChild("CardType" + i.ToString()).gameObject.GetComponent<UICardTypeItem>();
            cardTypeControllers.Add(payPort);
        }

        int count = 0;
        foreach (ConfigCardTypeRecord record in ConfigManager.configCardType.records)
        {
            cardTypeControllers[count].Setup(this, record);
            count++;
        }

        for (int i = count; i < NUMBER_CARD_TYPE_PER_PAGE; i++)
            cardTypeControllers[i].Disable();
    }

    void OnClick()
    {
        GameObject obj = UICamera.selectedObject;
    }

    public void NotifySwitchContent(ConfigCardTypeRecord cardType)
    {
        controller.SwitchState(FHCardExchangeState.EnterInfo, cardType);
    }
}