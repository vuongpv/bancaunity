using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class PayItem
{
	public string payID;
	public string productID;

	public PayItem(string payID, string productID)
	{
		this.payID = payID;
		this.productID = productID;
	}
}

public class UIRestore : MonoBehaviour {

	private UIShopHandler shopHander;

	[NonSerialized]
	public List<PayItem> smsPayIDs = new List<PayItem>();

	[NonSerialized]
	public List<PayItem> cardPayIDs = new List<PayItem>();

	public UILabel totalGold;

	void Awake()
	{
		shopHander = NGUITools.FindInParents<UIShopHandler>(gameObject);
	}

	public void AddSMS(string payID, string productID)
	{
		foreach (var item in smsPayIDs)
			if (item.payID == payID)
				return;

		smsPayIDs.Add(new PayItem(payID, productID));
	}

	public void AddCard(string payID, string productID)
	{
		foreach (var item in cardPayIDs)
			if (item.payID == payID)
				return;

		cardPayIDs.Add(new PayItem(payID, productID));
	}

	void OnClick()
	{
		GameObject obj = UICamera.selectedObject;

		switch (obj.name)
		{
			case "DoRestoreBtn":
				if (smsPayIDs.Count > 0)
				{
					shopHander.ChangeStatus(true);
					RestoreSMS();
				}
				else if (cardPayIDs.Count > 0)
				{
					shopHander.ChangeStatus(true);
					RestoreCard();
				}
				break;
		}
	}

	void RestoreSMS()
	{
		PayItem item = smsPayIDs[0];
		smsPayIDs.RemoveAt(0);
		FHSMSPayment.instance.onCloseTransaction = () =>
			{
				if (FHGoldHudPanel.instance != null)
					FHGoldHudPanel.instance.UpdateGold();

				Debug.Log("Restore SMS " + item.payID + ", remain SMS: " + smsPayIDs.Count + ", remain CARD: " + smsPayIDs.Count);
				if (smsPayIDs.Count == 0)
				{
					if (cardPayIDs.Count > 0)
						RestoreCard();
					else
					{
						shopHander.ChangeStatus(false);

						// Refresh
						shopHander.OnClickRestore();
					}
				}
				else
				{
					RestoreSMS();
				}
			};

		FHSMSPayment.instance.RestoreTransaction(item);
	}

	void RestoreCard()
	{
		PayItem item = cardPayIDs[0];
		cardPayIDs.RemoveAt(0);
		FHCardPayment.instance.onCloseTransaction = () =>
			{
				if (FHGoldHudPanel.instance != null)
					FHGoldHudPanel.instance.UpdateGold();

				Debug.Log("Restore CARD" + item.payID);
				if (cardPayIDs.Count == 0)
				{
					shopHander.ChangeStatus(false);
				
					// Refresh
					shopHander.OnClickRestore();
				}
				else
				{
					RestoreCard();
				}
			};
		FHCardPayment.instance.RestoreTransaction(item);
	}
}
