using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using com.soomla.unity;

public enum FHCardExchangeState
{
	SelectType = 0,
	EnterInfo = 1,
}

public class UICardExchange : UIBaseDialogHandler
{
	public GameObject main;
	public UICardSelectType selectType;
	public UICardEnterInfo enterInfo;

    public GameObject status;
    public UILabel statusMsg;
    public UIShopStatus shopStatusController;

	public GameObject btnClose;

    FHCardExchangeState currentState;

    string exchangeDialogTitle;

	public override void OnInit()
	{
        exchangeDialogTitle = FHLocalization.instance.GetString(FHStringConst.EXCHANGE_CARD_DIALOG_TITLE);
    }

    public override void OnBeginShow(object parameter)
	{
        currentState = FHCardExchangeState.SelectType;
        ChangeStatus(false);
	}

	public override void OnBeginHide (object parameter)
	{
		base.OnBeginHide (parameter);
	}

	public void SwitchState(FHCardExchangeState state)
	{
        SwitchState(state, null);
	}

    public void SwitchState(FHCardExchangeState state, object data)
	{
        switch (state)
		{
            case FHCardExchangeState.SelectType:
                enterInfo.gameObject.SetActiveRecursively(false);

                selectType.Setup();
                break;

            case FHCardExchangeState.EnterInfo:
				selectType.gameObject.SetActiveRecursively(false);
                
                ConfigCardTypeRecord cardType = (ConfigCardTypeRecord)data;
                List<ConfigCardRecord> cards = ConfigManager.configCard.GetCardByType(cardType.id);
                enterInfo.Setup(cards[0]);
				break;
		}

		currentState = state;
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
		}
	}

	void ResetGUI()
	{
		selectType.gameObject.SetActiveRecursively(true);
		enterInfo.gameObject.SetActiveRecursively(false);

		currentState = FHCardExchangeState.SelectType;
		selectType.Setup();
	}

	void OnClick()
	{
		GameObject obj = UICamera.selectedObject;

		switch (obj.name)
		{
			case "BtnClose":
				OnBtnClose();
				break;
		}
	}

	public void OnBtnClose()
	{
		GuiManager.HidePanel(GuiManager.instance.guiCardExchange);
	}

    public void RequestCardExchange(ConfigCardRecord card, string email, string phone)
    {
        Debug.LogError("[Card] Request exchange " + card.diamondValue + " diamonds for card " + card.cardValue);

        ChangeStatus(true);
        FHCardExchange.instance.RequestExchange(card, email, phone, CardExchangeCallback);
    }

    void CardExchangeCallback(int code, ConfigCardRecord card)
    {
        ChangeStatus(false);

        if (code == FHResultCode.NOT_CONNECT)
            GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.ENABLE_NET), exchangeDialogTitle, FH.MessageBox.MessageBoxButtons.OK);
        else
        if (code == FHResultCode.HTTP_ERROR)
            GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.CANNOT_CONNECT_PAYMENT_SERVER), exchangeDialogTitle, FH.MessageBox.MessageBoxButtons.OK);
        else
        if (code == FHResultCode.TIME_OUT)
            GUIMessageDialog.Show(null, FHLocalization.instance.GetString(FHStringConst.CANNOT_FINISH_PAYMENT), exchangeDialogTitle, FH.MessageBox.MessageBoxButtons.OK);
        else
        if (code == FHResultCode.OK)
        {
            string message = string.Format(FHLocalization.instance.GetString(FHStringConst.CARD_EXCHANGE_SUCCESS), card.diamondValue);

            GUIMessageDialog.Show(null, message, exchangeDialogTitle, FH.MessageBox.MessageBoxButtons.OK);

            if (FHDiamondHudPanel.instance != null)
            FHDiamondHudPanel.instance.UpdateDiamond();
        }
    }
}