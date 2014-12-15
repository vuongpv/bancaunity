using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;

public delegate void FHSMSPaymentCallback(int code, ConfigGoldPackRecord goldpack);

public class FHSMSPayment : SingletonMono<FHSMSPayment>
{
	private enum TransactionState
	{
		None = 0,
		Request = 1,
		SendSMS = 2,
		Poll = 3,
		Close = 4,
	}

	private const float POLL_TIME_CYCLE = 2.0f;
	private const float FAKE_RESULT_TIME = 3.0f;

	private const string SERVICE_SMS_FORMAT = "MXU FH";

	private TransactionState state;
	private string payID = "";
	private FHSMSPaymentCallback paymentCallback = null;
	public Action onCloseTransaction;
	private ConfigGoldPackRecord goldPack;
	
	private float requestStartTime;
	private float requestElapsedTime;
	private bool pollCycleFinished;

	void Reset()
	{
		state = TransactionState.None;
		payID = "";
		paymentCallback = null;
		goldPack = null;
	}

	void Update()
	{
		if (state == TransactionState.None)
			return;

		if (state == TransactionState.Request || state == TransactionState.SendSMS || state == TransactionState.Close)
		{
			if (Time.time - requestStartTime > FHGameConstant.SHOP_REQUEST_TIMEOUT)
				CompleteTransaction(FHResultCode.TIME_OUT);

			return;
		}

		if (state == TransactionState.Poll)
		{
			requestElapsedTime += Time.deltaTime;

			if (Time.time - requestStartTime > FHGameConstant.SHOP_REQUEST_TIMEOUT)
				CompleteTransaction(FHResultCode.TIME_OUT);
			else
			if (pollCycleFinished && requestElapsedTime > POLL_TIME_CYCLE)
				MakeAPollRequest();

			return;
		}
	}

	public void RequestPayment(ConfigGoldPackRecord pack, FHSMSPaymentCallback callback)
	{
		if (!FHHttpClient.isInternetReachable)
		{
			paymentCallback = callback;
			CompleteTransaction(FHResultCode.NOT_CONNECT);
			return;
		}

		// Reset transaction info
		Reset();

		state = TransactionState.Request;

		paymentCallback = callback;
		goldPack = pack;

		// Make request params
		Dictionary<string, object> @params = new Dictionary<string, object>();
		@params["deviceID"] = SystemHelper.deviceUniqueID;
		@params["product"] = pack.id;
		@params["money"] = pack.cashValue;
#if UNITY_EDITOR
		@params["sandbox"] = true;
#endif

		requestStartTime = Time.time;
		FHHttpClient.RequestTransaction(@params, "sms", (code, json) =>
		{
			OnReceivedPayID((int)pack.cashValue, code, json);

			// @STATISTIC: Pay SMS 
			switch((int)pack.cashValue)
			{
				case 5000: FlurryBinding.SendEvent(StatisticDefine.PAY, "price", "5000"); break;
				case 10000: FlurryBinding.SendEvent(StatisticDefine.PAY, "price", "10000"); break;
				case 15000: FlurryBinding.SendEvent(StatisticDefine.PAY, "price", "15000"); break;
			}
		});
	}

	void OnReceivedPayID(int price, int code, JSONNode json)
	{
		if (code != FHResultCode.OK)
		{
			CompleteTransaction(code);
			return;
		}

		try
		{
			payID = (string)json["payID"];
		}
		catch (System.Exception ex)
		{
			CompleteTransaction(FHResultCode.FAILED);
			return;
		}

		state = TransactionState.SendSMS;
		OpenSMSWindow(price);
	}

	void OpenSMSWindow(int price)
	{
		Debug.LogError("@@@@@ PayID = " + payID);
		
		string phoneNumber = ExternalServices.SMSServiceNumberFromPrice(price);
		Debug.LogError("@@@@@ Number = " + phoneNumber);

#if UNITY_EDITOR
		OnBackFromSMSWindow(UnityEventStatus.OK);
#else
		if (phoneNumber != "")
		{
			PhoneUtilityBinding.e_phone_sendsms = OnBackFromSMSWindow;
			PhoneUtilityBinding.SendSMS(phoneNumber, GetSMSContent());
		}
		else
			CompleteTransaction(FHResultCode.FAILED);
#endif
	}

	void OnBackFromSMSWindow(string status)
	{
		Debug.LogError("SMS return status = " + status);

		if (status != "ok")
		{
			CompleteTransaction(FHResultCode.FAILED);
			return;
		}

		state = TransactionState.Poll;
		PollTransaction();

		// for testing, remove when release!!!
		//FakeResult();
	}

	public void RestoreTransaction(PayItem item)
	{
		this.payID = item.payID;
		this.goldPack = ConfigManager.configGoldPack.GetPackByID(item.productID);
		PollTransaction();
	}

	void PollTransaction()
	{
		if (payID == "")
		{
			CompleteTransaction(FHResultCode.FAILED);
			return;
		}

		MakeAPollRequest();
	}

	void MakeAPollRequest()
	{
		requestElapsedTime = 0.0f;
		pollCycleFinished = false;

		FHHttpClient.PollTranstaction(payID, (code, json) =>
		{
			OnReceivedPollTransaction(payID, code, json);
		});
	}

	void OnReceivedPollTransaction(string payID, int code, JSONNode json)
	{
		if (code != FHResultCode.OK)
		{
			pollCycleFinished = true;
			return;
		}

		try
		{
			if ( goldPack != null && (string)json["product"] != goldPack.id)
			{
				CompleteTransaction(FHResultCode.FAILED);
				return;
			}
		}
		catch (System.Exception ex)
		{
			CompleteTransaction(FHResultCode.FAILED);
			return;
		}

		if (!UpdateGold())
		{
			CompleteTransaction(FHResultCode.FAILED);
			return;
		}

		state = TransactionState.Close;
		CloseTransaction();                
	}

	void CloseTransaction()
	{
		FHHttpClient.CloseTranstaction(payID, (code, json) =>
		{
			OnReceivedCloseTransaction(code, json);
			if (onCloseTransaction != null)
				onCloseTransaction();
		});
	}
	
	void OnReceivedCloseTransaction(int code, JSONNode json)
	{
		CompleteTransaction(code);
	}

	void CompleteTransaction(int code)
	{
		if (paymentCallback != null)
			paymentCallback(code, goldPack);

		Reset();
	}

	string GetSMSContent()
	{
		return SERVICE_SMS_FORMAT + " " + SystemHelper.deviceUniqueID + " " + payID;
	}

	bool UpdateGold()
	{
		if (goldPack == null)
			return false;

        if (goldPack.packType == (int)FHShopPackType.Gold)
        {
            int goldTotal = goldPack.goldValue + goldPack.goldBonus;

            if (FHSystem.instance.GetCurrentPlayerMode() == FHPlayerMode.Multi)
                FHMultiPlayerManager.instance.GetMainPlayer().AddCoin(goldTotal);
            else
                FHPlayerProfile.instance.gold += goldTotal;

            FHPlayerProfile.instance.ForceSave();
        }
        else
        {
            //int goldTotal = goldPack.goldValue + goldPack.goldBonus;
            //FHPlayerProfile.instance.diamond += goldTotal;
            //FHPlayerProfile.instance.ForceSave();
            FHHttpClient.GetCurrentDiamond((code, json) =>
            {
                if (code == FHResultCode.OK)
                {
                    Debug.LogError(json);
                    int diamond = int.Parse((string)json["diamond"]);
                    FHPlayerProfile.instance.diamond = diamond;
                    FHDiamondHudPanel.instance.UpdateDiamond();
                }
                else
                {
                    Debug.LogError("FH Error diamond restore");
                }
            });
        }

		return true;
	}

	#region Testing
	void FakeResult()
	{
		StartCoroutine(_FakeResult());
	}

	IEnumerator _FakeResult()
	{
		yield return new WaitForSeconds(FAKE_RESULT_TIME);

		Hashtable header = new Hashtable();
		header.Add("Content-Type", "application/json");

		string body = "{\"data\":\"" + payID.ToString() + "\",\"money\":\"" + goldPack.cashValue + "\",\"transactionid\":\"" + System.DateTime.Now.Ticks.ToString() + "\"}";

		Debug.LogError("@@@@@ Fake body: " + body);

		FHHttpClient.FakeResult(header, body, (code, json) =>
		{
			OnReceivedFakeResult(code, json);            
		});
	}

	void OnReceivedFakeResult(int code, JSONNode json)
	{
	}
	#endregion
}