using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISelectGoldPack : MonoBehaviour
{
	const int NUMBER_PACKS_PER_PAGE = 6;

	public UIShopHandler shopController;
	public ConfigPayPortRecord payPort;
    FHShopPackType packType;

	public GameObject back;
	public UILabel note;

	List<UIGoldPackItem> packControllers = new List<UIGoldPackItem>();

	public void Setup(ConfigPayPortRecord _payPort, FHShopPackType _packType)
	{
		gameObject.SetActiveRecursively(true);

		payPort = _payPort;
        packType = _packType;

		packControllers.Clear();
		for (int i = 0; i < NUMBER_PACKS_PER_PAGE; i++)
		{
			UIGoldPackItem pack = gameObject.transform.FindChild("Pack" + i.ToString()).gameObject.GetComponent<UIGoldPackItem>();
			packControllers.Add(pack);
		}

        List<ConfigGoldPackRecord> packs = ConfigManager.configGoldPack.GetPacksByPayPortID(payPort.id, packType);

		int count = 0;
		for (int i = 0; i < packs.Count; i++)
		{
			packControllers[i].Setup(this, packs[i]);
			count++;
		}

		for (int i = count; i < NUMBER_PACKS_PER_PAGE; i++)
			packControllers[i].Disable();

		if (!IsNeedEnterCode())
			back.SetActiveRecursively(false);

		DisableNote();
	}

	public void Reset()
	{
		for (int i = 0; i < packControllers.Count; i++)
		{
			if (packControllers[i].currentState != UIGoldPackItemState.Normal)
				packControllers[i].SwitchState(UIGoldPackItemState.Normal);
		}
	}

	public bool IsNeedConfirm()
	{
        return ((FHPayPortIndex)payPort.id == FHPayPortIndex.SMS || (FHPayPortIndex)payPort.id == FHPayPortIndex.Diamond);
	}

	public bool IsNeedEnterCode()
	{
		FHPayPortIndex payPortID = (FHPayPortIndex)payPort.id;

		return (payPortID == FHPayPortIndex.ZingCard ||
			payPortID == FHPayPortIndex.MobiCard ||
			payPortID == FHPayPortIndex.VinaCard ||
			payPortID == FHPayPortIndex.ViettelCard
		);
	}

	void OnClick()
	{
		GameObject obj = UICamera.selectedObject;

		switch (obj.name)
		{
			case "Back":
				shopController.SwitchContent(FHShopContentType.SelectPayPort);
				break;
		}
	}

	public void NotifySwitchContent(ConfigGoldPackRecord pack)
	{
		shopController.SwitchContent(FHShopContentType.EnterCode, pack);
	}

	public void RequestSMSPayment(ConfigGoldPackRecord pack)
	{
		shopController.RequestSMSPayment(pack);
	}

	public void RequestPlayStorePayment(ConfigGoldPackRecord pack)
	{
		shopController.RequestPlayStorePayment(pack);
	}

	public void RequestAppleStorePayment(ConfigGoldPackRecord pack)
	{
		shopController.RequestAppleStorePayment(pack);
	}

    public void RequestDiamondExchange(ConfigGoldPackRecord pack)
    {
        shopController.RequestDiamondExchange(pack);
    }
    
    public void DisableNote()
	{
		note.transform.parent.gameObject.SetActiveRecursively(false);
	}

	public void EnableNote(float cashValue)
	{
		if (!note.transform.parent.gameObject.active)
			note.transform.parent.gameObject.SetActiveRecursively(true);
		note.text = string.Format(FHLocalization.instance.GetString(FHStringConst.PAYMENT_SMS_NOTE), cashValue);
	}
}