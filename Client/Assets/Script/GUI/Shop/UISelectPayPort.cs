using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISelectPayPort : MonoBehaviour
{
    const int NUMBER_PAYPORTS_PER_PAGE = 4;

    public UIShopHandler shopController;

    List<UIPayPortItem> payPortControllers = new List<UIPayPortItem>();

    public void Setup(List<ConfigPayPortRecord> payPorts)
    {
        gameObject.SetActiveRecursively(true);

        payPortControllers.Clear();
        for (int i = 0; i < NUMBER_PAYPORTS_PER_PAGE; i++)
        {
            UIPayPortItem payPort = gameObject.transform.FindChild("PayPort" + i.ToString()).gameObject.GetComponent<UIPayPortItem>();
            payPortControllers.Add(payPort);
        }

        int count = 0;
        for (int i = 0; i < payPorts.Count; i++)
        {
            payPortControllers[i].Setup(this, payPorts[i]);
            count++;
        }

        for (int i = count; i < NUMBER_PAYPORTS_PER_PAGE; i++)
            payPortControllers[i].Disable();
    }

    void OnClick()
    {
        GameObject obj = UICamera.selectedObject;
    }

    public void NotifySwitchContent(ConfigPayPortRecord payPort)
    {
        shopController.SwitchContent(FHShopContentType.SelectGoldPack, payPort);
    }
}