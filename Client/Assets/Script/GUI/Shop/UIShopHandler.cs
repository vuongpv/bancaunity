using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using com.soomla.unity;
using System.Linq;

public enum FHShopPackType
{
    Gold = 0,
    Diamond = 1,
}

// Mapping from id in ConfigPayport.txt
public enum FHPayPortIndex : int
{
	None = -1,
	SMS = 0,
	PlayStore = 1,
	PlayStore_HD = 101,
	AppleStore = 2,
	ZingCard = 3,
	MobiCard = 4,
	VinaCard = 5,
	ViettelCard = 6,
    Diamond = 1001,
}

public enum FHShopContentType
{
	SelectGoldPack = 0,
	SelectPayPort = 1,
	EnterCode = 2,
}

public class FHPayPortTab
{
	public string iconName;
	public List<ConfigPayPortRecord> payPorts;

	public FHPayPortTab(string data)
	{
		iconName = "";
		payPorts = null;

		string[] splitStrs = data.Split(';');

		if (splitStrs.Length < 2)
			return;

		iconName = splitStrs[0];
		payPorts = new List<ConfigPayPortRecord>();

		int payPortID;
		for (int i = 1; i < splitStrs.Length; i++)
			if (int.TryParse(splitStrs[i], out payPortID))
			{
				ConfigPayPortRecord payPort = ConfigManager.configPayPort.GetItemByID(payPortID);
				if (payPort != null)
					payPorts.Add(payPort);
			}
	}
}

public class UIShopHandler : UIBaseDialogHandler
{
	const int BUILD_ID = 0;
	const int NUMBER_TABS_PER_PAGE = 3;

	public GameObject main;
	public UISelectGoldPack selectGoldPack;
	public UISelectPayPort selectPayPort;
	public UIEnterCode enterCode;
	public UIRestore restorePage;
	public GameObject status;

	public UITextList listPaidTransations;
	public UILabel totalPaidTransationsGold;

	public GameObject tabs;
	public GameObject btnClose;
	public UILabel statusMsg;
	public UISprite tabSeperator;

	public GameObject restoreBtn;

	public UIShopStatus shopStatusController;

    Dictionary<FHShopPackType, List<FHPayPortTab>> payPortTabsAll = new Dictionary<FHShopPackType, List<FHPayPortTab>>();
	List<FHPayPortTab> payPortTabs;
	List<UIShopTabItem> tabControllers = new List<UIShopTabItem>();

    FHShopPackType currentPackType;
    ConfigPayPortRecord currentPayPort;
	FHShopContentType currentContentType;
	int currentTabStartID, currentTabSelectedID;
	string paymentDialogTitle;
    float tabHeight;

	public static UIShopHandler instance;

	public static void Show()
	{
#if UNITY_IPHONE
		if( !FHSystem.instance.enableLocalPayment )
		{
			GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.SHOP_UNAVAILABLE), FHLocalization.instance.GetString(FHStringConst.SHOP_UNAVAILABLE_TITLE), FH.MessageBox.MessageBoxButtons.OK);
			return;
		}
#endif

		GuiManager.ShowPanel(GuiManager.instance.guiShopHandler);

	}
	
	public override void OnInit()
	{
		// read config
		ConfigPayPortBuildRecord build = ConfigManager.configPayPortBuild.GetItemByID((int)(FHSystem.instance.payportShop));

        ParseDataForShopType(FHSystem.instance.shopConfig["gold"], FHShopPackType.Gold);
		ParseDataForShopType(FHSystem.instance.shopConfig["diamond"], FHShopPackType.Diamond);

		paymentDialogTitle = FHLocalization.instance.GetString(FHStringConst.PAYMENT_DIALOG_TITLE);

        tabControllers.Clear();
        for (int i = 0; i < NUMBER_TABS_PER_PAGE; i++)
        {
            UIShopTabItem tab = tabs.transform.FindChild("Tab" + i.ToString()).gameObject.GetComponent<UIShopTabItem>();
            tabControllers.Add(tab);
        }
        
        tabHeight = tabControllers[0].transform.FindChild("Background").localScale.y;
    }

    void ParseDataForShopType(string data, FHShopPackType type)
    {
        payPortTabsAll[type] = new List<FHPayPortTab>();

        if (data == null || data == "")
            return;

        string[] splitStrs = data.Split('|');
        for (int i = 0; i < splitStrs.Length; i++)
        {
            FHPayPortTab tab = new FHPayPortTab(splitStrs[i]);

#if UNITY_IPHONE
			if( !FHSystem.instance.enableLocalPayment )
			{
				tab.payPorts = tab.payPorts.Where(p => 
				                                  p.id != (int) FHPayPortIndex.SMS
				                                  && p.id != (int) FHPayPortIndex.ZingCard
				                                  && p.id != (int) FHPayPortIndex.MobiCard 
				                                  && p.id != (int) FHPayPortIndex.VinaCard 
				                                  && p.id != (int) FHPayPortIndex.ViettelCard).ToList(); 
			}
			if( tab.payPorts.Count == 0 )
				continue;
#endif
            payPortTabsAll[type].Add(tab);
        }
    }



	public override void OnBeginShow(object parameter)
	{
		if( instance == null )
			instance = this;

        currentPackType = FHShopPackType.Gold;
		if (parameter is FHShopPackType)
            currentPackType = (FHShopPackType)parameter;
        payPortTabs = payPortTabsAll[currentPackType];

		currentTabStartID = 0;
		currentTabSelectedID = 0;

		currentPayPort = null;
		currentContentType = FHShopContentType.SelectGoldPack;

		ChangeStatus(false);
		InAppBillingBinding.Init();

		if (restorePage != null)
		{
			restorePage.gameObject.SetActiveRecursively(false);
			restoreBtn.gameObject.SetActiveRecursively(true);
		}

		// Open store
#if UNITY_IPHONE
		StoreController.StoreOpening ();
#endif
	}

	public override void OnBeginHide (object parameter)
	{

		base.OnBeginHide (parameter);

		// Close store
#if UNITY_IPHONE
		StoreController.StoreClosing ();
#endif

		if (instance == this)
			instance = null;
	}

	public void SwitchContent(FHShopContentType type)
	{
		SwitchContent(type, null);
	}

	public void SwitchContent(FHShopContentType type, object data)
	{
		switch (type)
		{
			case FHShopContentType.SelectPayPort:
				selectGoldPack.gameObject.SetActiveRecursively(false);

				selectPayPort.Setup(payPortTabs[currentTabSelectedID].payPorts);
				break;

			case FHShopContentType.SelectGoldPack:
				if (currentContentType == FHShopContentType.EnterCode)
					enterCode.gameObject.SetActiveRecursively(false);
				else
					selectPayPort.gameObject.SetActiveRecursively(false);

				if (data != null && data is ConfigPayPortRecord)
					currentPayPort = (ConfigPayPortRecord)data;

				selectGoldPack.Setup(currentPayPort, currentPackType);
				break;

			case FHShopContentType.EnterCode:
				selectGoldPack.gameObject.SetActiveRecursively(false);

				if (data != null && data is ConfigGoldPackRecord)
					enterCode.Setup((ConfigGoldPackRecord)data);
				break;
		}

		currentContentType = type;
	}

	public void ChangeStatus(bool waiting)
	{
		ChangeStatus(waiting, FHGameConstant.SHOP_REQUEST_TIMEOUT);
	}

	public void ChangeStatus(bool waiting, float timeout)
	{
		if (waiting)
		{
			main.SetActiveRecursively(false);

			status.SetActiveRecursively(true);
			status.GetComponentInChildren<UIShopStatus>().Setup(timeout);
		}
		else
		{
			main.SetActiveRecursively(true);

			status.SetActiveRecursively(false);

			ResetGUI();

			if (restorePage != null)
			{
				restorePage.gameObject.SetActiveRecursively(false);
				restoreBtn.gameObject.SetActiveRecursively(true);
			}
		}
	}

	void ResetGUI()
	{
		tabSeperator.transform.localScale = new Vector3(tabSeperator.transform.localScale.x, GetSeperatorHeight(), tabSeperator.transform.localScale.z);

        for (int i = payPortTabs.Count; i < NUMBER_TABS_PER_PAGE; i++)
            tabControllers[i].SetEnable(false);

		OnTabChanged();
	}

	void OnTabChanged()
	{
		int endIndex = currentTabStartID + NUMBER_TABS_PER_PAGE - 1;
		if (endIndex >= payPortTabs.Count)
			endIndex = payPortTabs.Count - 1;

		int index = 0;
		for (int i = currentTabStartID; i <= endIndex; i++)
		{
            tabControllers[i].SetEnable(true);

			if (i == currentTabSelectedID)
				tabControllers[index].SetState(true);
			else
				tabControllers[index].SetState(false);

			tabControllers[index].dataIndex = i;
			tabControllers[index].SetIcon(payPortTabs[i].iconName);
			
			index++;
		}

		selectGoldPack.gameObject.SetActiveRecursively(false);
		selectPayPort.gameObject.SetActiveRecursively(false);
		enterCode.gameObject.SetActiveRecursively(false);

		if (currentTabSelectedID >= 0 && payPortTabs.Count > 0)
		{
			if (payPortTabs[currentTabSelectedID].payPorts.Count > 1)
			{
				currentContentType = FHShopContentType.SelectPayPort;
				selectPayPort.Setup(payPortTabs[currentTabSelectedID].payPorts);
			}
			else
			{
				currentContentType = FHShopContentType.SelectGoldPack;
				currentPayPort = payPortTabs[currentTabSelectedID].payPorts[0];
				selectGoldPack.Setup(currentPayPort, currentPackType);
			}
		}
	}

	void OnClick()
	{
		GameObject obj = UICamera.selectedObject;

		switch (obj.name)
		{
			case "BtnClose":
				OnBtnClose();
				break;

			case "Tab0":
			case "Tab1":
			case "Tab2":
				OnClickTab(obj);
				break;

			case "RestoreBtn":
				OnClickRestore();
				break;

		}
	}

	public void OnBtnClose()
	{
		GuiManager.HidePanel(GuiManager.instance.guiShopHandler);
	}

	void OnClickTab(GameObject tab)
	{
		if (restorePage != null)
		{
			restorePage.gameObject.SetActiveRecursively(false);
			restoreBtn.gameObject.SetActiveRecursively(true);
		}

		UIShopTabItem controller = tab.GetComponent<UIShopTabItem>();

		if (!(controller.dataIndex >= 0 && controller.dataIndex < payPortTabs.Count))
			return;

		if (currentTabSelectedID != controller.dataIndex)
		{
			currentTabSelectedID = controller.dataIndex;
			OnTabChanged();
		}
    }

    float GetSeperatorHeight()
    {
		if( payPortTabs.Count <= 0 )
			return 0.01f;
		return (Mathf.Abs(tabControllers[0].transform.localPosition.y - tabControllers[payPortTabs.Count - 1].transform.localPosition.y) + tabHeight);
    }

    #region Restore
    public void OnClickRestore()
	{
		currentTabSelectedID = -1;
		OnTabChanged();
		restorePage.gameObject.SetActiveRecursively(true);
		restoreBtn.gameObject.SetActiveRecursively(false);
		listPaidTransations.Clear();

		// Query transactions
		FHHttpClient.GetPaidTranstactions((r, json) =>
			{
				restorePage.totalGold.text = "0";
				if (r == FHResultCode.OK)
				{
					JSONArray payments = json["payments"].AsArray;
					if (payments.Count > 0)
					{
						int totalGold = 0;
						foreach (JSONNode payment in payments)
						{
							/*payID: payment._id,
							netAmount: payment.net_amount,
							grossAmount: payment.gross_amount,
							product: payment.product,
							type: payment.type*/


							if (payment["type"].Value == "sms")
							{
								ConfigGoldPackRecord goldPack = ConfigManager.configGoldPack.GetPackByID(payment["product"].Value);
								if (goldPack != null)
								{
									listPaidTransations.Add("SMS: " + payment["grossAmount"].Value + "   ->   Gold: [FFFF00]" + (goldPack.goldValue + goldPack.goldBonus) + "[-]");
									restorePage.AddSMS(payment["payID"].Value, payment["product"].Value);

									totalGold += goldPack.goldValue + goldPack.goldBonus;
								}
							}
							else if (payment["type"].Value == "card")
							{
								ConfigGoldPackRecord goldPack = ConfigManager.configGoldPack.GetPackByCashValue(payment["grossAmount"].AsInt);
								if (goldPack != null)
								{
									listPaidTransations.Add("CARD: " + payment["grossAmount"].Value + "   ->   Gold: [FFFF00]" + (goldPack.goldValue + goldPack.goldBonus) + "[-]");
									restorePage.AddCard(payment["payID"].Value, payment["product"].Value);

									totalGold += goldPack.goldValue + goldPack.goldBonus;
								}
							}


						}

						restorePage.totalGold.text = totalGold.ToString();
					}
					else
					{
						listPaidTransations.Add(FHLocalization.instance.GetString(FHStringConst.RESTORE_PURCHASE_NODATA));
					}
				}
				else
				{
					listPaidTransations.Add(FHLocalization.instance.GetString(FHStringConst.RESTORE_PURCHASE_ERROR));
				}
			});
    }
    #endregion

	#region SMS payment
	public void RequestSMSPayment(ConfigGoldPackRecord pack)
	{
		ChangeStatus(true);
		FHSMSPayment.instance.RequestPayment(pack, SMSPaymentCallback);
	}

	void SMSPaymentCallback(int code, ConfigGoldPackRecord goldpack)
	{
		ChangeStatus(false);

		if (code == FHResultCode.NOT_CONNECT)
			GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.ENABLE_NET), paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);
		else
		if (code == FHResultCode.HTTP_ERROR)
			GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.CANNOT_CONNECT_PAYMENT_SERVER), paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);
		else
		if (code == FHResultCode.TIME_OUT)
			GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.CANNOT_FINISH_PAYMENT), paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);
		else
		if (code == FHResultCode.OK)
		{
            if (goldpack.packType == (int)FHShopPackType.Gold)
            {
                // @STATISTIC : Pay first time
                if (!FHPlayerProfile.instance.stat_firstPay)
                {
                    FHPlayerProfile.instance.stat_firstPay = true;
                    FlurryBinding.SendEvent(StatisticDefine.PAY_1ST_TIME);
                }

                string message = "";
                if (goldpack.goldBonus > 0)
                    message = string.Format(FHLocalization.instance.GetString(FHStringConst.PAYMENT_SUCCESS), goldpack.goldValue, goldpack.goldBonus);
                else
                    message = string.Format(FHLocalization.instance.GetString(FHStringConst.PAYMENT_SUCCESS_NO_BONUS), goldpack.goldValue);

                GUIMessageDialog.Show(null, message, paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);

                if (FHGoldHudPanel.instance != null)
                    FHGoldHudPanel.instance.UpdateGold();
            }
            else
            {
                string message = "";
                if (goldpack.goldBonus > 0)
                    message = string.Format(FHLocalization.instance.GetString(FHStringConst.DIAMOND_PAYMENT_SUCCESS), goldpack.goldValue, goldpack.goldBonus);
                else
                    message = string.Format(FHLocalization.instance.GetString(FHStringConst.DIAMOND_PAYMENT_SUCCESS_NO_BONUS), goldpack.goldValue);

                GUIMessageDialog.Show(null, message, paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);

                if (FHDiamondHudPanel.instance != null)
                    FHDiamondHudPanel.instance.UpdateDiamond();
            }
		}
		else
			GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.PAYMENT_FAILED), paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);
	}
	#endregion

	#region Play store
	public void RequestPlayStorePayment(ConfigGoldPackRecord pack)
	{
		ChangeStatus(true);
		
		InAppBillingBinding.BuyItem(pack.id);

		StartCoroutine(UnblockUI());

	}

	IEnumerator UnblockUI()
	{
		yield return new WaitForSeconds(2.0f);

		ChangeStatus(false);
	}
	IEnumerator UnblockUITimeout(float _time)
	{
		yield return new WaitForSeconds(_time);

		ChangeStatus(false);
	}
	#endregion

	#region Apple store
	public void RequestAppleStorePayment(ConfigGoldPackRecord pack)
	{
		ChangeStatus(true);

		foreach (var item in StoreInfo.GetVirtualCurrencyPacks())
		{
			Debug.LogError ("Pack " + item.ItemId);
			if( item.ItemId == pack.id )
				Debug.LogError ("Match");
		}

		Debug.LogError ("Buy market item " + pack.id);
		StoreInventory.BuyItem (pack.id);

	
		StopCoroutine("UnblockUITimeout");
		StartCoroutine(UnblockUITimeout(60));

	}
	#endregion

	#region Card payment
	public void RequestCardPayment(ConfigGoldPackRecord pack, string cardSerial, string cardCode)
	{
		ChangeStatus(true);
		FHCardPayment.instance.RequestPayment(pack, cardSerial, cardCode, CardPaymentCallback);
	}

	void CardPaymentCallback(int code, ConfigGoldPackRecord goldpack)
	{
		ChangeStatus(false);

		if (code == FHResultCode.NOT_CONNECT)
			GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.ENABLE_NET), paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);
		else
		if (code == FHResultCode.HTTP_ERROR)
			GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.CANNOT_CONNECT_PAYMENT_SERVER), paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);
		else
		if (code == FHResultCode.TIME_OUT)
			GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.CANNOT_FINISH_PAYMENT), paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);
		else
		if (code == FHResultCode.OK)
		{
            if (goldpack.packType == (int)FHShopPackType.Gold)
            {
                // @STATISTIC : Pay first time
                if (!FHPlayerProfile.instance.stat_firstPay)
                {
                    FHPlayerProfile.instance.stat_firstPay = true;
                    FlurryBinding.SendEvent(StatisticDefine.PAY_1ST_TIME);
                }

                string message = "";
                if (goldpack.goldBonus > 0)
                    message = string.Format(FHLocalization.instance.GetString(FHStringConst.PAYMENT_SUCCESS), goldpack.goldValue, goldpack.goldBonus);
                else
                    message = string.Format(FHLocalization.instance.GetString(FHStringConst.PAYMENT_SUCCESS_NO_BONUS), goldpack.goldValue);

                GUIMessageDialog.Show(null, message, paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);

                if (FHGoldHudPanel.instance != null)
                    FHGoldHudPanel.instance.UpdateGold();
            }
            else
            {
                string message = "";
                if (goldpack.goldBonus > 0)
                    message = string.Format(FHLocalization.instance.GetString(FHStringConst.DIAMOND_PAYMENT_SUCCESS), goldpack.goldValue, goldpack.goldBonus);
                else
                    message = string.Format(FHLocalization.instance.GetString(FHStringConst.DIAMOND_PAYMENT_SUCCESS_NO_BONUS), goldpack.goldValue);

                GUIMessageDialog.Show(null, message, paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);

                if (FHDiamondHudPanel.instance != null)
                    FHDiamondHudPanel.instance.UpdateDiamond();
            }
		}
		else
		{
			int errorCodeStrID = FHStringConst.PAYMENT_FAILED;

			switch (code)
			{
				case FHCardPaymentErrorCode.PAYCARD_CODE_5000:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_5000;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_N100:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_N100;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_N101:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_N101;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_6000:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_6000;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_6100:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_6100;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_6200:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_6200;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_6206:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_6206;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_7400:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_7400;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_7500:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_7500;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_7501:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_7501;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_7502:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_7502;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_7503:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_7503;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_7504:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_7504;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_7505:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_7505;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_7506:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_7506;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_7507:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_7507;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_7508:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_7508;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_7509:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_7509;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_7510:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_7510;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_7511:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_7511;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_7800:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_7800;
					break;

				case FHCardPaymentErrorCode.PAYCARD_CODE_7899:
					errorCodeStrID = FHStringConst.PAYCARD_CODE_7899;
					break;
			}

			GUIMessageDialog.Show(null, FHLocalization.instance.GetString(errorCodeStrID), paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);
		}
	}
	#endregion

    #region Diamond exchange
    public void RequestDiamondExchange(ConfigGoldPackRecord pack)
    {
        ChangeStatus(true);

        FHCoinExchange.instance.RequestPayment(pack, DiamondExchangeCallback);
    }

    void DiamondExchangeCallback(int code, ConfigGoldPackRecord goldpack)
    {
        ChangeStatus(false);

        if (code == FHResultCode.NOT_CONNECT)
            GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.ENABLE_NET), paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);
        else
        if (code == FHResultCode.HTTP_ERROR)
            GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.CANNOT_CONNECT_PAYMENT_SERVER), paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);
        else
        if (code == FHResultCode.TIME_OUT)
            GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.CANNOT_FINISH_PAYMENT), paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);
        else
        if (code == FHResultCode.OK)
        {
            string message = "";
            if (goldpack.goldBonus > 0)
                message = string.Format(FHLocalization.instance.GetString(FHStringConst.COIN_EXCHANGE_SUCCESS), goldpack.cashValue, goldpack.goldValue, goldpack.goldBonus);
            else
                message = string.Format(FHLocalization.instance.GetString(FHStringConst.COIN_EXCHANGE_SUCCESS_NO_BONUS), goldpack.cashValue, goldpack.goldValue);

            GUIMessageDialog.Show(null, message, paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);

            if (FHGoldHudPanel.instance != null)
                FHGoldHudPanel.instance.UpdateGold();

            if (FHDiamondHudPanel.instance != null)
                FHDiamondHudPanel.instance.UpdateDiamond();
        }
        else
            GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.PAYMENT_FAILED), paymentDialogTitle, FH.MessageBox.MessageBoxButtons.OK);
    }
    #endregion
}