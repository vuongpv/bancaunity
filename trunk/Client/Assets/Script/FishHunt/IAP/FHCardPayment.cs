using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;

public static class FHCardPaymentErrorCode
{
	public const int PAYCARD_CODE_1 = 1;
	public const int PAYCARD_CODE_N100 = -100;
	public const int PAYCARD_CODE_N101 = -101;
	public const int PAYCARD_CODE_5000 = 5000;
	public const int PAYCARD_CODE_6000 = 6000;
	public const int PAYCARD_CODE_6100 = 6100;
	public const int PAYCARD_CODE_6200 = 6200;
	public const int PAYCARD_CODE_6206 = 6206;
	public const int PAYCARD_CODE_7400 = 7400;
	public const int PAYCARD_CODE_7500 = 7500;
	public const int PAYCARD_CODE_7501 = 7501;
	public const int PAYCARD_CODE_7502 = 7502;
	public const int PAYCARD_CODE_7503 = 7503;
	public const int PAYCARD_CODE_7504 = 7504;
	public const int PAYCARD_CODE_7505 = 7505;
	public const int PAYCARD_CODE_7506 = 7506;
	public const int PAYCARD_CODE_7507 = 7507;
	public const int PAYCARD_CODE_7508 = 7508;
	public const int PAYCARD_CODE_7509 = 7509;
	public const int PAYCARD_CODE_7510 = 7510;
	public const int PAYCARD_CODE_7511 = 7511;
	public const int PAYCARD_CODE_7800 = 7800;
	public const int PAYCARD_CODE_7899 = 7899;
}

public delegate void FHCardPaymentCallback(int code, ConfigGoldPackRecord goldpack);

public class FHCardPayment : SingletonMono<FHCardPayment>
{
	private enum TransactionState
	{
		None = 0,
		Request = 1,
		SendCard = 2,
		Poll = 3,
		Close = 4,
	}

	private const float POLL_TIME_CYCLE = 2.0f;
	private const float FAKE_RESULT_TIME = 3.0f;

	private const string GAME_ID = "FH";

	private TransactionState state;
	private string payID = "";
	private FHCardPaymentCallback paymentCallback = null;
	public Action onCloseTransaction;
	private ConfigGoldPackRecord goldPack;

	private string cardSerial, cardCode;
	
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

		if (state == TransactionState.Request || state == TransactionState.Close || state == TransactionState.SendCard)
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

	public void RequestPayment(ConfigGoldPackRecord pack, string _cardSerial, string _cardCode, FHCardPaymentCallback callback)
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
		cardSerial = _cardSerial;
		cardCode = _cardCode;

		string cardType = SO6PaymentCard.MOBIFONE;
		if( goldPack.cardType == "MBF" )
		{
			cardType = SO6PaymentCard.MOBIFONE;
		}
		else if( goldPack.cardType == "VNP" )
		{
			cardType = SO6PaymentCard.VINAFONE;
		}
		else if( goldPack.cardType == "VTL" )
		{
			cardType = SO6PaymentCard.VIETTEL;
		}
		else if (goldPack.cardType == "ZIN")
		{
			cardType = SO6PaymentCard.ZINGCARD;
		}
		else
		{
			return;
		}

		// Make request params
		Dictionary<string, object> @params = new Dictionary<string, object>();
		@params["deviceID"] = SystemHelper.deviceUniqueID;
		@params["product"] = goldPack.id;
		@params["money"] = goldPack.cashValue;

#if UNITY_EDITOR
		@params["sandbox"] = true;
#endif

#if UNITY_IPHONE
		@params["cardType"] = cardType; //"VMS"; //cardType;
		@params["cardSerialNo"] = _cardSerial; // "123456789156";//_cardSerial;
		@params["cardCode"] = _cardCode; //"1234567892222"; //_cardCode;
#endif

		requestStartTime = Time.time;
		FHHttpClient.RequestTransaction(@params, "card", (code, json) =>
		{
			OnReceivedPayID(code, json);
		});
	}


	void OnReceivedPayID(int code, JSONNode json)
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

		state = TransactionState.SendCard;
#if UNITY_ANDROID
		SendCard();
#elif UNITY_IPHONE
		int paymentCode = json["paymentCode"].AsInt;
		OnCardSent(paymentCode);
#endif
	}

	void SendCard()
	{
		Debug.LogError("@@@@@ PayID = " + payID);

		int card = 1;
		if( goldPack.cardType == "MBF" )
		{
			card = 1;
		}
		else if( goldPack.cardType == "VNP" )
		{
			card = 2;
		}
		else if( goldPack.cardType == "VTL" )
		{
			card = 3;
		}
		else if (goldPack.cardType == "ZIN")
		{
			card = 4;
		}
		else
		{
			return;
		}

#if UNITY_EDITOR || UNITY_IPHONE
		OnCardSent(FHCardPaymentErrorCode.PAYCARD_CODE_1);
#elif UNITY_ANDROID
		Debug.LogError("@@@@@ Type = " + goldPack.cardType);
		Debug.LogError("@@@@@ Serial = " + cardSerial);
		Debug.LogError("@@@@@ Code = " + cardCode);

		SO6PaymentBinding.e_card_result = OnCardSent;
		SO6PaymentBinding.SendCard(SystemHelper.deviceUniqueID, GAME_ID, card, cardSerial, cardCode, payID);
#endif
	}

	void OnCardSent(int errorCode)
	{
		Debug.LogError("SO6Payment return errorCode = " + errorCode);

		if (errorCode != FHCardPaymentErrorCode.PAYCARD_CODE_1)
		{
			CompleteTransaction(errorCode);
			return;
		}

		state = TransactionState.Poll;
		PollTransaction();
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
			int cashValue = json["grossAmount"].AsInt;
			goldPack = ConfigManager.configGoldPack.GetPackByCashValue(cashValue);
			if (goldPack == null)
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
                    Debug.LogError("FH Error updategold diamond ");
                }
            });
        }

		return true;
	}
}